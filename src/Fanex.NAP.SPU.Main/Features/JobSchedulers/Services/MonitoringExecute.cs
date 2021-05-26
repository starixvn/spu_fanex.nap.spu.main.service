using Quartz;
using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.Services
{
    [DisallowConcurrentExecutionAttribute]
    [PersistJobDataAfterExecution]
    public class MonitoringExecute : IJob
    {
        private readonly ISentryClient sentryClient;
        private readonly IJobService jobService;
        private readonly IScheduler scheduler;
        public MonitoringExecute(ISentryClient sentryClient, IJobService jobService, ISchedulerFactory schedulerFactory)
        {
            this.sentryClient = sentryClient;
            this.jobService = jobService;
            scheduler = schedulerFactory.GetScheduler().Result;
        }

        public Task Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
