using Fanex.NAP.SPU.Main.Middleware;
using Fanex.NAP.SPU.Notification.Services;
using ONEbook.UM.Services;
using Quartz;
using Quartz.Impl.Matchers;
using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.Services
{
    [DisallowConcurrentExecution, PersistJobDataAfterExecution]
    public class JobMonitoring : IJob
    {
        private readonly ISentryClient _sentryClient;
        private readonly IScheduler _scheduler;
        private readonly IMaintenanceService _maintenanceService;
        private readonly IAppSettings _appSettings;
        private DateTime lastTime;

        public JobMonitoring(ISentryClient sentryClient, IAppSettings appSettings, ISchedulerFactory schedulerFactory, IMaintenanceService maintenanceService)
        {
            _scheduler = schedulerFactory.GetScheduler().Result;
            _maintenanceService = maintenanceService;
            _appSettings = appSettings;
            _sentryClient = sentryClient;
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
                        var totalSecond = (DateTime.Now - lastTime).TotalSeconds;

                        if (totalSecond >= 300)
                        {
                            var message = "The jobs are PAUSED.\n\r - " + string.Join("\n\r - ", result.ToArray());

                            _sentryClient.CaptureMessage(message, Sentry.Protocol.SentryLevel.Error);

                            lastTime = DateTime.Now;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _sentryClient.CaptureException(ex);
            }
        }

        private void Notification(string jobName, string message, string moreInfo)
        {
            var notification = new NotificationService();

            notification
                      .Config("0A40aOc7CtpzjbBSJu8KNWQRoXyjRvf9", "-1001571051228")
                      .Message(new MessageModel(jobName, DateTime.UtcNow.AddHours(7), _appSettings.EnvironmentString, message, moreInfo))
                      .SendWithDelay(300);
        }
    }
}
