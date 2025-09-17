namespace Fanex.NAP.SPU.CTS.Features.JobSchedulers.Model
{
    using System.Collections.Generic;

    public class Scheduler
    {
        public IEnumerable<GroupJob> GroupJobs { get; set; }
    }
}
