namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.Models
{
    public class Trigger
    {
        public int TriggerId { get; set; }

        public string TriggerType { get; set; }

        public string TriggerName { get; set; }

        public string CronExpression { get; set; }

        public int IntervalInSecond { get; set; }


        public override bool Equals(object obj)
        {
            var other = obj as Trigger;
            
            if (other == null) return false;

            if (other.TriggerName != TriggerName ||
                other.TriggerType != TriggerType ||               
                other.CronExpression != CronExpression ||
                other.IntervalInSecond != IntervalInSecond)
            {               

                return false;
            }

            return true;
        }

        public override int GetHashCode() => (TriggerType, TriggerName, CronExpression).GetHashCode();        
    }
}
