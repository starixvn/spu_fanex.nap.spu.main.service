namespace Fanex.NAP.SPU.CTS.Features.SchedulerManager
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.Features.SchedulerManager.Model;

    public interface ISchedulerManagerDataAccess
    {
        IEnumerable<GroupJobEntity> GetGroupJobs();

        JobInfoModel GetJobs();

        JobInfoModel GetSingleJob(string jobName, string groupName);

        bool ClearedJobNew(string jobName, string groupName);

        bool UpdateFlagJobUpdate(bool isUpdate);

        bool DeleteJobToDb(string jobName, string groupName);

        bool AddJob(string jobTriggerJson);
    }

    public class SchedulerManagerDataAccess : ISchedulerManagerDataAccess   
    {
        private readonly IDynamicRepository _repository;

        public SchedulerManagerDataAccess() 
        {
            _repository = new DynamicRepository();
        }

        public IEnumerable<GroupJobEntity> GetGroupJobs()
        {
            var criteria = new GetGroupJobCriteria();

            return _repository.Fetch<GroupJobEntity>(criteria);
        }

        public JobInfoModel GetJobs()
        {
            var criteria = new GetJobsCriteria();

            (var jobData, var triggerData) = _repository.FetchMultiple<JobEntity, TriggerEntity>(criteria);

            return new JobInfoModel
            {
                Jobs = jobData,
                Triggers = triggerData
            };
        }

        public JobInfoModel GetSingleJob(string jobName, string groupName)
        {
            var criteria = new GetSingleJobCriteria()
            {
                JobName = jobName,
                GroupName = groupName
            };

            (var jobData, var triggerData) = _repository.FetchMultiple<JobEntity, TriggerEntity>(criteria);

            return new JobInfoModel
            {
                Jobs = jobData,
                Triggers = triggerData
            };
        }

        public bool ClearedJobNew(string jobName, string groupName)
        {
            var criteria = new ClearedJobNewCriteria
            {
                JobName = jobName,
                GroupName = groupName
            };

            _repository.Execute(criteria);

            return true;
        }

        public bool UpdateFlagJobUpdate(bool isUpdate)
        {
            var criteria = new UpdateFlagJobUpdateCriteria { IsUpdate = isUpdate };

            _repository.Execute(criteria);

            return true;
        }

        public bool DeleteJobToDb(string jobName, string groupName)
        {
            var criteria = new DeleteJobCriteria { JobName = jobName, GroupName = groupName };

            var errorCode = _repository.Execute(criteria);

            return true;
        }

        public bool AddJob(string jobTriggerJson)
        {
            var criteria = new AddJobCriteria() { JobTriggerJson = jobTriggerJson };

            return _repository.Execute(criteria) == 1;
        }
    }
}
