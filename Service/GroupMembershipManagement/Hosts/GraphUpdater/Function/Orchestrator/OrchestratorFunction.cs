// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using Entities;
using Entities.ServiceBus;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Graph;
using Newtonsoft.Json;
using Repositories.Contracts;
using Repositories.Contracts.InjectConfig;
using Services.Contracts;
using Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hosts.GraphUpdater
{
    public class OrchestratorFunction
    {
        private const string SyncCompletedEmailBody = "SyncCompletedEmailBody";
        private readonly TelemetryClient _telemetryClient;
        private readonly IGraphUpdaterService _graphUpdaterService = null;
        private readonly IEmailSenderRecipient _emailSenderAndRecipients = null;
        private readonly IThresholdConfig _thresholdConfig = null;
        private readonly IGMMResources _gmmResources = null;
        private readonly bool _isDryRunEnabled;
        enum Metric
        {
            SyncComplete,
            Result,
            SyncJobTimeElapsedSeconds,
            ProjectedMemberCount
        }

        public OrchestratorFunction(
            ILoggingRepository loggingRepository,
            TelemetryClient telemetryClient,
            IGraphUpdaterService graphUpdaterService,
            IDryRunValue dryRun,
            IEmailSenderRecipient emailSenderAndRecipients,
            IThresholdConfig thresholdConfig,
            IGMMResources gmmResources)
        {
            _telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
            _graphUpdaterService = graphUpdaterService ?? throw new ArgumentNullException(nameof(graphUpdaterService));
            _isDryRunEnabled = loggingRepository.DryRun = dryRun != null ? dryRun.DryRunEnabled : throw new ArgumentNullException(nameof(dryRun));
            _emailSenderAndRecipients = emailSenderAndRecipients ?? throw new ArgumentNullException(nameof(emailSenderAndRecipients));
            _thresholdConfig = thresholdConfig ?? throw new ArgumentNullException(nameof(thresholdConfig));
            _gmmResources = gmmResources ?? throw new ArgumentNullException(nameof(gmmResources));
        }

        [FunctionName(nameof(OrchestratorFunction))]
        public async Task<OrchestrationRuntimeStatus> RunOrchestratorAsync([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            GroupMembership groupMembership = null;
            GraphUpdaterHttpRequest graphRequest = null;
            SyncJob syncJob = null;

            graphRequest = context.GetInput<GraphUpdaterHttpRequest>();

            try
            {
                syncJob = await context.CallActivityAsync<SyncJob>(nameof(JobReaderFunction),
                                                       new JobReaderRequest
                                                       {
                                                           JobPartitionKey = graphRequest.JobPartitionKey,
                                                           JobRowKey = graphRequest.JobRowKey,
                                                           RunId = graphRequest.RunId
                                                       });

                var fileContent = await context.CallActivityAsync<string>(nameof(FileDownloaderFunction),
                                                                            new FileDownloaderRequest
                                                                            {
                                                                                FilePath = graphRequest.FilePath,
                                                                                RunId = graphRequest.RunId,
                                                                                SyncJob = syncJob
                                                                            });

                groupMembership = JsonConvert.DeserializeObject<GroupMembership>(fileContent);
                var updatedSourceMembers = groupMembership.SourceMembers.Distinct().ToList();

                await context.CallActivityAsync(nameof(LoggerFunction), new LoggerRequest { Message = $"{nameof(OrchestratorFunction)} function started", SyncJob = syncJob });

                await context.CallActivityAsync(nameof(LoggerFunction), new LoggerRequest { Message = $"Received membership from StarterFunction and will sync the obtained {updatedSourceMembers.Count} distinct members", SyncJob = syncJob });

                var isValidGroup = await context.CallActivityAsync<bool>(nameof(GroupValidatorFunction),
                                           new GroupValidatorRequest
                                           {
                                               RunId = groupMembership.RunId,
                                               GroupId = groupMembership.Destination.ObjectId,
                                               JobPartitionKey = groupMembership.SyncJobPartitionKey,
                                               JobRowKey = groupMembership.SyncJobRowKey
                                           });

                if (!isValidGroup)
                {
                    await context.CallActivityAsync(nameof(JobStatusUpdaterFunction),
                                    CreateJobStatusUpdaterRequest(groupMembership.SyncJobPartitionKey, groupMembership.SyncJobRowKey,
                                                                    SyncStatus.DestinationGroupNotFound, groupMembership.MembershipObtainerDryRunEnabled, syncJob.ThresholdViolations, groupMembership.RunId));

                    await context.CallActivityAsync(nameof(LoggerFunction), new LoggerRequest { Message = $"{nameof(OrchestratorFunction)} function did not complete", SyncJob = syncJob });

                    return OrchestrationRuntimeStatus.Completed;
                }

                var destinationGroupMembers = await context.CallSubOrchestratorAsync<List<AzureADUser>>(nameof(UsersReaderSubOrchestratorFunction),
                                                                                                        new UsersReaderRequest { SyncJob = syncJob });

                var fullMembership = new GroupMembership
                {
                    Destination = groupMembership.Destination,
                    IsLastMessage = groupMembership.IsLastMessage,
                    RunId = groupMembership.RunId,
                    SourceMembers = updatedSourceMembers,
                    SyncJobPartitionKey = groupMembership.SyncJobPartitionKey,
                    SyncJobRowKey = groupMembership.SyncJobRowKey
                };

                var deltaResponse = await context.CallActivityAsync<DeltaResponse>(nameof(DeltaCalculatorFunction),
                                                new DeltaCalculatorRequest
                                                {
                                                    RunId = groupMembership.RunId,
                                                    GroupMembership = fullMembership,
                                                    MembersFromDestinationGroup = destinationGroupMembers,
                                                });

                if (deltaResponse.GraphUpdaterStatus == GraphUpdaterStatus.Error ||
                    deltaResponse.GraphUpdaterStatus == GraphUpdaterStatus.ThresholdExceeded)
                {
                    var updateRequest = CreateJobStatusUpdaterRequest(groupMembership.SyncJobPartitionKey, groupMembership.SyncJobRowKey,
                                                                    deltaResponse.SyncStatus, groupMembership.MembershipObtainerDryRunEnabled, syncJob.ThresholdViolations, groupMembership.RunId);

                    if (deltaResponse.GraphUpdaterStatus == GraphUpdaterStatus.ThresholdExceeded)
                    {
                        updateRequest.ThresholdViolations++;

                        if (updateRequest.ThresholdViolations >= _thresholdConfig.NumberOfThresholdViolationsToDisableJob)
                            updateRequest.Status = SyncStatus.ThresholdExceeded;
                    }

                    await context.CallActivityAsync(nameof(JobStatusUpdaterFunction), updateRequest);
                }

                if (deltaResponse.GraphUpdaterStatus != GraphUpdaterStatus.Ok)
                {
                    await context.CallActivityAsync(nameof(LoggerFunction), new LoggerRequest { Message = $"{nameof(OrchestratorFunction)} function did not complete", SyncJob = syncJob });

                    return OrchestrationRuntimeStatus.Terminated;
                }

                if (!deltaResponse.IsDryRunSync)
                {
                    await context.CallSubOrchestratorAsync<GraphUpdaterStatus>(nameof(GroupUpdaterSubOrchestratorFunction),
                                    CreateGroupUpdaterRequest(syncJob, deltaResponse.MembersToAdd, RequestType.Add, deltaResponse.IsInitialSync));

                    await context.CallSubOrchestratorAsync<GraphUpdaterStatus>(nameof(GroupUpdaterSubOrchestratorFunction),
                                    CreateGroupUpdaterRequest(syncJob, deltaResponse.MembersToRemove, RequestType.Remove, deltaResponse.IsInitialSync));

                    if (deltaResponse.IsInitialSync)
                    {
                        var groupName = await context.CallActivityAsync<string>(nameof(GroupNameReaderFunction),
                                                        new GroupNameReaderRequest { RunId = groupMembership.RunId, GroupId = groupMembership.Destination.ObjectId });

                        var groupOwners = await context.CallActivityAsync<List<User>>(nameof(GroupOwnersReaderFunction),
                                                        new GroupOwnersReaderRequest { RunId = groupMembership.RunId, GroupId = groupMembership.Destination.ObjectId });

                        var ownerEmails = string.Join(";", groupOwners.Where(x => !string.IsNullOrWhiteSpace(x.Mail)).Select(x => x.Mail));

                        var additonalContent = new[]
                        {
                                groupName,
                                groupMembership.Destination.ObjectId.ToString(),
                                deltaResponse.MembersToAdd.Count.ToString(),
                                deltaResponse.MembersToRemove.Count.ToString(),
                                deltaResponse.Requestor,
                                _gmmResources.LearnMoreAboutGMMUrl,
                                _emailSenderAndRecipients.SupportEmailAddresses
                        };

                        await context.CallActivityAsync(nameof(EmailSenderFunction),
                                        new EmailSenderRequest
                                        {
                                            ToEmail = ownerEmails,
                                            CcEmail = _emailSenderAndRecipients.SyncCompletedCCAddresses,
                                            ContentTemplate = SyncCompletedEmailBody,
                                            AdditionalContentParams = additonalContent,
                                            RunId = groupMembership.RunId
                                        });
                    }
                }

                var message = GetUsersDataMessage(groupMembership.Destination.ObjectId, deltaResponse.MembersToAdd.Count, deltaResponse.MembersToRemove.Count);
                await context.CallActivityAsync(nameof(LoggerFunction), new LoggerRequest { Message = message });

                await context.CallActivityAsync(nameof(JobStatusUpdaterFunction),
                                    CreateJobStatusUpdaterRequest(groupMembership.SyncJobPartitionKey, groupMembership.SyncJobRowKey,
                                                                    SyncStatus.Idle, groupMembership.MembershipObtainerDryRunEnabled, 0, groupMembership.RunId));

                if (!context.IsReplaying)
                {
                    var timeElapsedForJob = (context.CurrentUtcDateTime - deltaResponse.Timestamp).TotalSeconds;
                    _telemetryClient.TrackMetric(nameof(Metric.SyncJobTimeElapsedSeconds), timeElapsedForJob);

                    var syncCompleteEvent = new Dictionary<string, string>
                    {
                        { nameof(SyncJob.TargetOfficeGroupId), groupMembership.Destination.ObjectId.ToString() },
                        { nameof(SyncJob.Type), deltaResponse.SyncJobType },
                        { nameof(groupMembership.RunId), groupMembership.RunId.ToString() },
                        { nameof(Metric.Result), deltaResponse.SyncStatus == SyncStatus.Idle ? "Success": "Failure" },
                        { nameof(SyncJob.IsDryRunEnabled), deltaResponse.IsDryRunSync.ToString() },
                        { nameof(Metric.SyncJobTimeElapsedSeconds), timeElapsedForJob.ToString() },
                        { nameof(DeltaResponse.MembersToAdd), deltaResponse.MembersToAdd.Count.ToString() },
                        { nameof(DeltaResponse.MembersToRemove), deltaResponse.MembersToRemove.Count.ToString() },
                        { nameof(Metric.ProjectedMemberCount), fullMembership.SourceMembers.Count.ToString() },
                        { nameof(DeltaResponse.IsInitialSync), deltaResponse.IsInitialSync.ToString() }
                    };

                    _telemetryClient.TrackEvent(nameof(Metric.SyncComplete), syncCompleteEvent);
                }

                await context.CallActivityAsync(nameof(LoggerFunction), new LoggerRequest { Message = $"{nameof(OrchestratorFunction)} function completed", SyncJob = syncJob });

                return OrchestrationRuntimeStatus.Completed;
            }
            catch (Exception ex)
            {
                await context.CallActivityAsync(nameof(LoggerFunction), new LoggerRequest { Message = $"Caught unexpected exception, marking sync job as errored. Exception:\n{ex}", SyncJob = syncJob });

                if (syncJob == null)
                {
                    await context.CallActivityAsync(nameof(LoggerFunction), new LoggerRequest { Message = "SyncJob is null. Removing the message from the queue..." });
                    return OrchestrationRuntimeStatus.Failed;
                }

                if (syncJob != null && groupMembership != null && !string.IsNullOrWhiteSpace(groupMembership.SyncJobPartitionKey) && !string.IsNullOrWhiteSpace(groupMembership.SyncJobRowKey))
                {
                    await context.CallActivityAsync(nameof(JobStatusUpdaterFunction),
                                    CreateJobStatusUpdaterRequest(groupMembership.SyncJobPartitionKey, groupMembership.SyncJobRowKey,
                                                                    SyncStatus.Error, groupMembership.MembershipObtainerDryRunEnabled, syncJob.ThresholdViolations, groupMembership.RunId));
                }

                throw;
            }
        }

        private JobStatusUpdaterRequest CreateJobStatusUpdaterRequest(string partitionKey, string rowKey, SyncStatus syncStatus, bool isDryRun, int thresholdViolations, Guid runId)
        {
            return new JobStatusUpdaterRequest
            {
                RunId = runId,
                JobPartitionKey = partitionKey,
                JobRowKey = rowKey,
                Status = syncStatus,
                IsDryRun = isDryRun,
                ThresholdViolations = thresholdViolations
            };
        }

        private GroupUpdaterRequest CreateGroupUpdaterRequest(SyncJob syncJob, ICollection<AzureADUser> members, RequestType type, bool isInitialSync)
        {
            return new GroupUpdaterRequest
            {
                SyncJob = syncJob,
                Members = members,
                Type = type,
                IsInitialSync = isInitialSync
            };
        }

        private string GetUsersDataMessage(Guid targetGroupId, int membersToAdd, int membersToRemove)
        {
            string message;
            if (_isDryRunEnabled)
                message = $"A Dry Run Synchronization for {targetGroupId} is now complete. " +
                          $"{membersToAdd} users would have been added. " +
                          $"{membersToRemove} users would have been removed.";
            else
                message = $"Synchronization for {targetGroupId} is now complete. " +
                          $"{membersToAdd} users have been added. " +
                          $"{membersToRemove} users have been removed.";

            return message;
        }
    }
}
