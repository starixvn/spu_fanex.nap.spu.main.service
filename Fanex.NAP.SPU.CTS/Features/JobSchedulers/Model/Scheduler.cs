namespace Fanex.NAP.SPU.CTS.Features.JobSchedulers.Model
{
    using System.Collections.Generic;
    using System.Linq;

    public class Scheduler
    {
        public IEnumerable<GroupJob> GroupJobs { get; set; }

        //public override bool Equals(object obj)
        //{
        //    var other = obj as Scheduler;

        //    if (other == null) return false;

        //    if (other.GroupJobs.Count() != GroupJobs.Count())
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return EqualGroupJob(GroupJobs, other.GroupJobs);

        //    }
        //}

        //private bool EqualGroupJob(IEnumerable<GroupJob> groupJobs1, IEnumerable<GroupJob> groupJobs2)
        //{
        //    return groupJobs1.Count() == groupJobs2.Count() && groupJobs1.All(groupJobs2.Contains);
        //}

        //public override int GetHashCode() => (GroupJobs).GetHashCode();
    }
}
