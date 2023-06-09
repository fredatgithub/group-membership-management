// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using Models;
using Repositories.Contracts;
using Services.Contracts;
using Services.Messages.Requests;
using Services.Messages.Responses;
using SyncJobDetailsDTO = WebApi.Models.DTOs.SyncJobDetails;

namespace Services{
    public class GetJobDetailsHandler : RequestHandlerBase<GetJobDetailsRequest, GetJobDetailsResponse>
    {
        private readonly ISyncJobRepository _syncJobRepository;
        private readonly IGraphGroupRepository _graphGroupRepository;

        public GetJobDetailsHandler(ILoggingRepository loggingRepository,
                              ISyncJobRepository syncJobRepository,
                              IGraphGroupRepository graphGroupRepository) : base(loggingRepository)
        {
            _syncJobRepository = syncJobRepository ?? throw new ArgumentNullException(nameof(syncJobRepository));
            _graphGroupRepository = graphGroupRepository ?? throw new ArgumentNullException(nameof(graphGroupRepository));
        }

        protected override async Task<GetJobDetailsResponse> ExecuteCoreAsync(GetJobDetailsRequest request)
        {
            var response = new GetJobDetailsResponse();
            SyncJob job = await _syncJobRepository.GetSyncJobAsync(request.PartitionKey, request.RowKey);

            bool isRequestorOwner = await _graphGroupRepository.IsEmailRecipientOwnerOfGroupAsync(job.Requestor, job.TargetOfficeGroupId);
            string requestor = isRequestorOwner ? job.Requestor : job.Requestor + " (Not an Owner)";

            var dto = new SyncJobDetailsDTO
                (
                    startDate: job.StartDate,
                    lastSuccessfulStartTime: job.LastSuccessfulStartTime,
                    source: job.Query,
                    period: job.Period,
                    requestor: requestor,
                    thresholdViolations: job.ThresholdViolations,
                    thresholdPercentageForAdditions: job.ThresholdPercentageForAdditions,
                    thresholdPercentageForRemovals: job.ThresholdPercentageForRemovals
                );

            response.Model = dto;

            return response;
        }
    }
}