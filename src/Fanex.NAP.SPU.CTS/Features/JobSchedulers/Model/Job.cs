namespace Fanex.NAP.SPU.CTS.Features.JobSchedulers.Model
{
    using Fanex.NAP.SPU.CTS.Features.SchedulerManager;
    using System.Collections.Generic;
    using System.Linq;

    public class Job
    {
        public Job() { }

        public Job(JobEntity data)
        {
            JobName = data.JobName; 
            JobType = data.JobType; 
            APIUrl= data.APIUrl;    
            MethodName = data.MethodName;   
            Controller = data.Controller;
            Active = data.IsActive;
            IsNew = data.IsNew;
        }

        //public int JobId { get; set; }

        public string JobName { get; set; }

        public string JobType { get; set; }

        public string APIUrl { get; set; }

        public string MethodName { get; set; }

        public string Controller { get; set; }

        public bool Active { get; set; }

        public bool IsNew { get; set; }

        public IEnumerable<Trigger> Triggers { get; set; }               

        public void MappingTrigger(IEnumerable<TriggerEntity> triggerEntites)
        {
            this.Triggers = triggerEntites
                .Where(x => x.JobName == this.JobName)
                .Select(x=>new Trigger(x));
        }

        //public override bool Equals(object obj)
        //{
        //    var other = obj as Job;

        //    if (other == null) return false;

        //    if (other.JobName != JobName ||
        //        other.JobType != JobType ||
        //        other.APIUrl != APIUrl ||
        //        other.MethodName != MethodName ||
        //        other.Controller != Controller ||
        //        other.Active != Active ||
        //        !EqualTrigger(Triggers, other.Triggers)                )
        //    {

        //        return false;
        //    }

        //    return true;
        //}

        //private bool EqualTrigger(IEnumerable<Trigger> triggers1, IEnumerable<Trigger> triggers2)
        //{            
        //    return triggers1.Count() == triggers2.Count() &&  triggers1.All(triggers2.Contains);
        //}

        //public override int GetHashCode() => (JobName, JobType, APIUrl, MethodName, Controller, Active, Triggers).GetHashCode();

    }
}