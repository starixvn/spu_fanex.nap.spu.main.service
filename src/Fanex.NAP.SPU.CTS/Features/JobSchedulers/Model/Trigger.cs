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

        //public int TriggerId { get; set; }        

        public string TriggerName { get; set; }

        public string TriggerType { get; set; }

        public string CronExpression { get; set; }

        public int IntervalInSecond { get; set; }

        public string TimeZone { get; set; }


        //public override bool Equals(object obj)
        //{
        //    var other = obj as Trigger;
            
        //    if (other == null) return false;

        //    if (other.TriggerName != TriggerName ||
        //        other.TriggerType != TriggerType ||               
        //        other.CronExpression != CronExpression ||
        //        other.IntervalInSecond != IntervalInSecond ||
        //        other.TimeZone != TimeZone)
        //    {               

        //        return false;
        //    }

        //    return true;
        //}

        //public override int GetHashCode() => (TriggerType, TriggerName, CronExpression).GetHashCode();        
    }
}
