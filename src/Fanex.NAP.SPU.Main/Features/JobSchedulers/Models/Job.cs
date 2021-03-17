namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.Models
{
    using System.Collections.Generic;

    public class Job
    {
        public string JobName { get; set; }

        public string JobType { get; set; }

        public string APIUrl { get; set; }

        public string HealthCheckUrl { get; set; }

        public bool Active { get; set; }

        public UpdateStatus UpdateStatus { get; set; }

        public IEnumerable<Trigger> Triggers { get; set; }

    }
}