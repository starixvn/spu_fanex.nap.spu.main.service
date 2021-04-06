namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.Models
{
    public class Trigger
    {

        public int TriggerId { get; set; }

        public string TriggerType { get; set; }

        public string TriggerName { get; set; }

        public bool RepeatForever { get; set; }

        public string CronExpression { get; set; }
              

        public override bool Equals(object obj)
        {
            var other = obj as Trigger;
            
            if (other == null) return false;

            if (other.TriggerName != TriggerName ||
                other.TriggerType != TriggerType ||
                other.RepeatForever != RepeatForever ||
                other.CronExpression != CronExpression)
            {               

                return false;
            }

            return true;
        }

        public override int GetHashCode() => (TriggerType, TriggerName, RepeatForever, CronExpression).GetHashCode();        
    }
}
