namespace Fanex.NAP.SPU.Main.Features.SchedulerManager.ViewModel
{
    public class AddJobViewModel
    {
        public string JobName { get; set; }

        public string GroupName { get; set; }

        public bool IsActive { get; set; }

        public bool IsNew { get; set; }

        public string JobType { get; set; }

        public string ApiUrl { get; set; }

        public string MethodName { get; set; }

        public string Controller { get; set; }

        public string TriggerName { get; set; }

        public string TriggerType { get; set; }

        public string CronExpression { get; set; }

        public string IntervalInSecond { get; set; }           
       
    }
}
