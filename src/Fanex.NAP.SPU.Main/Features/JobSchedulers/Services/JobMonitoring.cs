using Fanex.NAP.SPU.CTS.Shares;
using Fanex.NAP.SPU.Main.Middleware;
using Fanex.NAP.SPU.Notification.Services;
using ONEbook.UM.Services;
using Quartz;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.Services
{
    [DisallowConcurrentExecution, PersistJobDataAfterExecution]
    public class JobMonitoring : IJob
    {
        private readonly IScheduler _scheduler;
        private readonly IMaintenanceService _maintenanceService;
        private readonly IAppSettings _appSettings;

        public JobMonitoring(IAppSettings appSettings, ISchedulerFactory schedulerFactory, IMaintenanceService maintenanceService)
        {
            _scheduler = schedulerFactory.GetScheduler().Result;
            _maintenanceService = maintenanceService;
            _appSettings = appSettings;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var isUM = _maintenanceService.GetUnderMaintenanceTime().IsUnderMaintenanceTime;

                if (!isUM)
                {
                    var allTriggerKeys = _scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup());
                    var result = new List<string>();

                    foreach (var triggerKey in allTriggerKeys.Result)
                    {
                        var triggerdetails = await _scheduler.GetTrigger(triggerKey);
                        var state = await _scheduler.GetTriggerState(triggerdetails.Key);

                        if (state == TriggerState.Paused)
                        {
                            result.Add(triggerdetails.JobKey.Name);
                        }
                    }

                    if (result.Any())
                    {
                        var jobName = string.Join("", result.Select(x => x + "\r\n"));

                        NotifyTelegramMessage("MainService Monitoring", $"The <b>{result.Count}</b> jobs are PAUSED.", $"The List of jobs: \n\r {jobName}", _appSettings.EnvironmentString, _appSettings.Proxy);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogError();
            }
        }

        private static void NotifyTelegramMessage(string systemName, string message, string moreInfo, string environment, string proxy)
        {
            if (environment == "PROD" || environment == "KLDR")
            {
                var notificationService = new NotificationService();

                notificationService
                   .Config("0A40aOc7CtpzjbBSJu8KNWQRoXyjRvf9", "B08GPPK88MS:T08ACMHMMFS:C091AGH3G04", "https://bot.nexdev.net/skynex/api/messages/forwardwithtoken", proxy)
                   .Message(new MessageModel(systemName, DateTime.Now, environment, message, moreInfo))
                   .SendWithoutDelay();
            }
        }
    }
}
