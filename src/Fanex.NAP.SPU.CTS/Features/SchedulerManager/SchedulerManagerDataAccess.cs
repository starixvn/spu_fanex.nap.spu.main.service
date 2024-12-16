namespace Fanex.NAP.SPU.CTS.Features.SchedulerManager
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.Features.SchedulerManager.Model;

    public interface ISchedulerManagerDataAccess
    {
        JobInfoModel GetJobs(bool isUpdate);

        bool UpdateFlagJobUpdate(bool isUpdate);
    }

    public class SchedulerManagerDataAccess : ISchedulerManagerDataAccess   
    {
        private readonly IDynamicRepository _repository;

        public SchedulerManagerDataAccess() 
        {
            _repository = new DynamicRepository();
        }

        public JobInfoModel GetJobs(bool isUpdate)
        {
            var criteria = new GetJobsCriteria { IsUpdate = isUpdate };

            (var jobData, var triggerData) = _repository.FetchMultiple<JobEntity, TriggerEntity>(criteria);

            return new JobInfoModel
            {
                Jobs = jobData,
                Triggers = triggerData
            };
        }

        public bool UpdateFlagJobUpdate(bool isUpdate)
        {
            var criteria = new UpdateFlagJobUpdateCriteria { IsUpdate = isUpdate };

            _repository.Execute(criteria);

            return true;
        }
    }
}
