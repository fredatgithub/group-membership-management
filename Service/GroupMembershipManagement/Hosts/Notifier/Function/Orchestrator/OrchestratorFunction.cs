// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Repositories.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models.ThresholdNotifications;
using Newtonsoft.Json;
using Models.Notifications;

namespace Hosts.Notifier
{
    public class OrchestratorFunction
    {
        public OrchestratorFunction()
        {
        }

        [FunctionName(nameof(OrchestratorFunction))]
        public async Task RunOrchestratorAsync(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var runId = context.NewGuid();

            await context.CallActivityAsync(nameof(LoggerFunction),
                new LoggerRequest
                {
                    RunId = runId,
                    Message = $"{nameof(OrchestratorFunction)} function started at: {context.CurrentUtcDateTime}",
                    Verbosity = VerbosityLevel.DEBUG
                });

            var (messageBody, messageType) = context.GetInput<(string, string)>();
            var messageContent = JsonConvert.DeserializeObject<Dictionary<string, object>>(messageBody);

            switch (messageType)
            {
                case nameof(NotificationMessageType.ThresholdNotification):
                    var notification = await context.CallActivityAsync<ThresholdNotification>(nameof(CreateActionableNotificationFromContentFunction), messageContent);
                    await context.CallActivityAsync(nameof(SendNotificationFunction), notification);
                    await context.CallActivityAsync(nameof(UpdateNotificationStatusFunction), new UpdateNotificationStatusRequest { Notification = notification, Status = ThresholdNotificationStatus.AwaitingResponse });
                    break;
                // Todo: Add other cases for different message types
                default:
                    await context.CallActivityAsync(nameof(LoggerFunction),
                    new LoggerRequest
                    {
                        RunId = runId,
                        Message = $"{messageType} is not a valid message type",
                        Verbosity = VerbosityLevel.DEBUG
                    });
                    break;
            }

            await context.CallActivityAsync(nameof(LoggerFunction),
                new LoggerRequest
                {
                    RunId = runId,
                    Message = $"{nameof(OrchestratorFunction)} function completed at: {context.CurrentUtcDateTime}",
                    Verbosity = VerbosityLevel.DEBUG
                });
        }
    }
}