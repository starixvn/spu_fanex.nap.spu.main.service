namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.Models
{
    using System.Collections.Generic;
    using System.Linq;    

    public class GroupJob
    {     
        public string GroupName { get; set; }

        public IEnumerable<Job> Jobs { get; set; }
    }
}
