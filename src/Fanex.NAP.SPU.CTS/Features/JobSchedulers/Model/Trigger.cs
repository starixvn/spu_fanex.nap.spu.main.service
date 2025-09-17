using Fanex.NAP.SPU.CTS.Features.SchedulerManager;

namespace Fanex.NAP.SPU.CTS.Features.JobSchedulers.Model
{
    public class Trigger
    {
        public Trigger() { }

        public Trigger(TriggerEntity triggerEntity) 
        {
            TriggerName = triggerEntity.TriggerName;
            TriggerType = triggerEntity.TriggerType;    
            CronExpression = triggerEntity.CronExpression; 
            IntervalInSecond = triggerEntity.IntervalInSecond;  
            TimeZone = triggerEntity.TimeZone; 
        }

        public string TriggerName { get; set; }

        public string TriggerType { get; set; }

        public string CronExpression { get; set; }

        public int IntervalInSecond { get; set; }

        public string TimeZone { get; set; }
      
    }
}
