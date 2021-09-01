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
        private readonly IScheduler scheduler;
        private readonly IMaintenanceService _maintenanceService;
        private readonly IAppSettings _appSettings;
        public JobMonitoring(IAppSettings appSettings, ISchedulerFactory schedulerFactory, IMaintenanceService maintenanceService)
        {     
            scheduler = schedulerFactory.GetScheduler().Result;
            _maintenanceService = maintenanceService;
            _appSettings = appSettings;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var isUM = _maintenanceService.GetUnderMaintenanceTime().IsUnderMaintenanceTime;

            if(!isUM)
            {
                var allTriggerKeys = scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup());
                var result = new List<string>();

                foreach (var triggerKey in allTriggerKeys.Result)
                {
                    var triggerdetails = await scheduler.GetTrigger(triggerKey);
                    var state = await scheduler.GetTriggerState(triggerdetails.Key);                                        
                    
                    if(state == TriggerState.Paused)
                    {
                        result.Add(triggerdetails.JobKey.Name);                                               
                    }                                       
                }

                if(result.Any())
                {
                    var jobName = string.Join("\n\r - ", result.ToArray());

                    Notification("Jobs State Monitoring", "The jobs are PAUSED. Please check again.", jobName);
                }
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
