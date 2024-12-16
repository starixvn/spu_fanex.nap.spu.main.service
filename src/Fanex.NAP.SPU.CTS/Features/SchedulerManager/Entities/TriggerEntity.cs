namespace Fanex.NAP.SPU.CTS.Features.SchedulerManager
{

    public class TriggerEntity
    {
        public string TriggerType { get; set; }

        public string JobName { get; set; }

        public string TriggerName { get; set; }

        public string CronExpression { get; set; }

        public int IntervalInSecond { get; set; }

        public string TimeZone { get; set; }
    }
}
