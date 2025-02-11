namespace Fanex.NAP.SPU.Main.Features.SchedulerManager.ViewModel
{
    public class AddJobViewModel
    {
        public string JobName { get; set; }

        public string GroupName { get; set; }

        public bool IsStartByDefault { get; set; }

        public int EnvironmentID { get; set; }     

        public bool IsAdjust { get; set; }

        public string JobType { get; set; }

        public string ApiUrl { get; set; }

        public string MethodName { get; set; }

        public string Controller { get; set; }

        public string TriggerName { get; set; }

        public string TriggerType { get; set; }

        public string CronExpression { get; set; }

        public string IntervalInSecond { get; set; } 
        
        public AddJobViewModel Clone()
        {
            return new AddJobViewModel
            {
                JobName = this.JobName,
                GroupName= this.GroupName,
                IsStartByDefault = this.IsStartByDefault,
                EnvironmentID = this.EnvironmentID,
                IsAdjust = this.IsAdjust,
                JobType = this.JobType,
                ApiUrl = this.ApiUrl,
                MethodName = this.MethodName,
                Controller = this.Controller,
                TriggerName = this.TriggerName,
                TriggerType = this.TriggerType,
                CronExpression = this.CronExpression,
                IntervalInSecond = this.IntervalInSecond
            };
        }
       
    }
}
