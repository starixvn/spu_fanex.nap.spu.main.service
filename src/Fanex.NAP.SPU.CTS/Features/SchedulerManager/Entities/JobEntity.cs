namespace Fanex.NAP.SPU.CTS.Features.SchedulerManager
{
    public class JobEntity
    {
        public string JobName { get; set; }

        public string GroupName { get; set; }

        public string JobType { get; set; }

        public string APIUrl { get; set; }

        public string MethodName { get; set; }

        public string Controller { get; set; }

        public bool IsActive { get; set; }
    }
}
