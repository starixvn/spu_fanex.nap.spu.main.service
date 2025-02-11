namespace Fanex.NAP.SPU.CTS.Shares
{
    using System.Collections.Generic;
    using System.Linq;

    public class JobComparison
    {
        public static (List<string> newJobs, List<string> removeJobs) CompareJobs(HashSet<string> schedulerJobs, List<string> databaseJobs)
        {
            var databaseJobSet = new HashSet<string>(databaseJobs);             

            // Jobs in the database but not in the scheduler
            var newJobs = databaseJobSet.Except(schedulerJobs).ToList();
           
            // Jobs in the scheduler but not in the database
            var removedJobs = schedulerJobs.Except(databaseJobSet).ToList();

            return (newJobs, removedJobs);
        }
    }
}
