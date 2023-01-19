// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Repositories.Contracts;
using System;
using System.Threading.Tasks;

namespace Hosts.JobTrigger
{
    public class JobTrackerFunction
    {
        private readonly ILoggingRepository _loggingRepository = null;

        public JobTrackerFunction(ILoggingRepository loggingRepository)
        {
            _loggingRepository = loggingRepository ?? throw new ArgumentNullException(nameof(loggingRepository));
        }

        [FunctionName(nameof(JobTrackerFunction))]
        public async Task<int> TrackJobFrequencyAsync([ActivityTrigger] SyncJob syncJob)
        {
            await _loggingRepository.LogMessageAsync(new LogMessage { Message = $"{nameof(JobTrackerFunction)} function started", RunId = syncJob.RunId }, VerbosityLevel.DEBUG);
            var frequency = 0;
            if (syncJob != null)
            {
                if (syncJob.LastSuccessfulRunTime == DateTime.FromFileTimeUtc(0)) return frequency;
                var timeDifference = (int)(DateTime.UtcNow - syncJob.LastSuccessfulRunTime).TotalHours;
                frequency = timeDifference / syncJob.Period;
            }
            await _loggingRepository.LogMessageAsync(new LogMessage { Message = $"{nameof(JobTrackerFunction)} function completed", RunId = syncJob.RunId }, VerbosityLevel.DEBUG);
            return frequency;
        }
    }
}