namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.ViewModels.Job
{
    using Fanex.NAP.SPU.CTS.Features.JobSchedulers.Model;
    using System;

    public class JobScheduleViewModel
    {
        public JobScheduleViewModel(GroupJob groupJob, Job jobModel, Trigger triggerModel)
        {
            Group = groupJob;

            JobName = jobModel.JobName;
            JobType = Type.GetType(jobModel.JobType);
            APIUrl = jobModel.APIUrl;            
            IsActive = jobModel.IsStartByDefault;
            MethodName = jobModel.MethodName;
            Controller = jobModel.Controller;
            TriggerType = triggerModel.TriggerType;
            TriggerName = triggerModel.TriggerName;           
            CronExpression = triggerModel.CronExpression;
        }

        public GroupJob Group { get; set; }

        public string JobName { get; set; }

        public Type JobType { get; }       

        public string APIUrl { get; set; }

        public string MethodName { get; set; }

        public string Controller { get; set; }

        public bool IsActive { get; set; }

        public string TriggerType { get; set; }

        public string TriggerName { get; set; }

        public string CronExpression { get; }

        public CronTriggerViewModel Cron { get; set; } = new CronTriggerViewModel();

        public bool Validate()
        {
            if (string.IsNullOrEmpty(JobName))
            {
                return false;
            }

            return Cron.Validate(this);
        }

    }
}
