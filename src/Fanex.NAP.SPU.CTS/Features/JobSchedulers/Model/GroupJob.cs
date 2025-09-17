namespace Fanex.NAP.SPU.CTS.Features.JobSchedulers.Model
{
    using System.Collections.Generic;

    public class GroupJob
    {
        public string Name { get; set; }

        public IEnumerable<Job> Jobs { get; set; }
    }
}
