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
       
    }
}