using Fanex.NAP.SPU.Main.Features.JobSchedulers.Services;
using Fanex.NAP.SPU.Main.Middleware;
using ONEbook.UM.Services;
using Quartz;
using Sentry;
using Sentry.Protocol;
using System;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.Main.JobSchedulers.Services
{
    [DisallowConcurrentExecution, PersistJobDataAfterExecution]
    public class JobExecute : IJob
    {
        private readonly ISentryClient sentryClient;
        private readonly IJobService jobService;
        private readonly IScheduler scheduler;
        private readonly IMaintenanceService _maintenanceService;
        private readonly IAppSettings _appSettings;

        public JobExecute(ISentryClient sentryClient
            , IJobService jobService
            , ISchedulerFactory schedulerFactory
            , IMaintenanceService maintenanceService
            , IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.jobService = jobService;
            scheduler = schedulerFactory.GetScheduler().Result;
            _maintenanceService = maintenanceService;
            _appSettings = appSettings;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var isUM = _maintenanceService.GetUnderMaintenanceTime().IsUnderMaintenanceTime;

            if (!isUM)
            {
                var dataMap = context.JobDetail.JobDataMap;
                var jobName = context.JobDetail.Key.Name;
                var apiUrl = dataMap.GetString("apiUrl");
                var count = dataMap.GetInt("count");
                var methodName = dataMap.GetString("methodName");
                var controller = dataMap.GetString("controller");


                var flagError = false;
                var messageError = string.Empty;

                if (count >= 5)
                {
                    var e = new JobExecutionException("Retries exceeded");

                    await scheduler.PauseTrigger(context.Trigger.Key);

                    dataMap.PutAsString("count", 0);

                    sentryClient.CaptureMessage($"{_appSettings.EnvironmentString} - {jobName} is STOPPED. Cause of RETRIED EXECEEDED.", SentryLevel.Error);

                    throw e;
                }

                try
                {
                    var queryString = JobHelper.GetQueryData(apiUrl);

                    var url = JobHelper.GetUrl(apiUrl);

                    var result = await jobService.Execute(jobName, url, methodName, controller, queryString);

                    if (!result)
                    {
                        flagError = true;
                        messageError = $"{_appSettings.EnvironmentString} - {jobName} is failed at {DateTime.Now}.";
                    }
                    else
                    {
                        dataMap.PutAsString("count", 0);
                    }
                }
                catch (Exception ex)
                {
                    flagError = true;
                    messageError = $"{_appSettings.EnvironmentString} - " + ex.Message;
                }

                if (flagError)
                {
                    count++;
                    dataMap.PutAsString("count", count);
                    throw JobHelper.JobExecuteFailed(messageError, sentryClient);
                }
            }
        }
    }
}