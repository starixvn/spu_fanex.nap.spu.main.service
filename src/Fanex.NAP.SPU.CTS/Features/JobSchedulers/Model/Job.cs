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
            EnvironmentID = data.EnvironmentID;
            APIUrl= data.APIUrl;    
            MethodName = data.MethodName;   
            Controller = data.Controller;
            // TODO: hardcode for test -> will rollback            
            //IsStartByDefault = data.IsStartByDefault;
            IsStartByDefault = false; 
            // TODO: hardcode for test -> will rollback end
            IsAdjust = data.IsAdjust;
        }

        //public int JobId { get; set; }

        public string JobName { get; set; }

        public string JobType { get; set; }

        public int EnvironmentID { get; set; }

        public string APIUrl { get; set; }

        public string MethodName { get; set; }

        public string Controller { get; set; }

        public bool IsStartByDefault { get; set; }

        public bool IsAdjust { get; set; }

        public IEnumerable<Trigger> Triggers { get; set; }               

        public void MappingTrigger(IEnumerable<TriggerEntity> triggerEntites)
        {
            this.Triggers = triggerEntites
                .Where(x => x.JobName == this.JobName)
                .Select(x=>new Trigger(x));
        }
       
    }
}