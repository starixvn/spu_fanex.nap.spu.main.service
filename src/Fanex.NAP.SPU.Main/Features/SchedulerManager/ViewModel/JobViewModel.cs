namespace Fanex.NAP.SPU.Main.Features.SchedulerManager.ViewModel
{
    using System.Collections.Generic;

    public class JobViewModel
    {
        public JobViewModel() { }

        public AddJobViewModel AddJob { get; set; }

        public string EnvironmentDbIDs { get; set; }

        public List<string> EnvironmentApiUrls { get; set; }
    }
}
