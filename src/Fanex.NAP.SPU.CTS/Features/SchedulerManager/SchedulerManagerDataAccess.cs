namespace Fanex.NAP.SPU.CTS.Features.SchedulerManager
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.Features.SchedulerManager.Model;

    public interface ISchedulerManagerDataAccess
    {
        bool AddJobToDb(string jobTriggerJson);

        bool UpdateJobToDb(string jobTriggerJson);

        IEnumerable<GroupJobEntity> GetGroupJobs();

        JobInfoModel GetJobs(bool isGetNew);

        JobInfoModel GetSingleJob(string jobName, string groupName);

        bool ClearedJobNew(string jobName, string groupName);     

        bool DeleteJobToDb(string jobName, string groupName);        
    }

    public class SchedulerManagerDataAccess : ISchedulerManagerDataAccess   
    {
        private readonly IDynamicRepository _repository;

        public SchedulerManagerDataAccess() 
        {
            _repository = new DynamicRepository();
        }

        public bool AddJobToDb(string jobTriggerJson)
        {
            var criteria = new AddJobCriteria() { JobTriggerJson = jobTriggerJson };

            return _repository.Execute(criteria) == 1;
        }

        public bool UpdateJobToDb(string jobTriggerJson)
        {
            var criteria = new UpdateJobCriteria() { JobTriggerJson = jobTriggerJson };

            return _repository.Execute(criteria) == 1;
        }

        public IEnumerable<GroupJobEntity> GetGroupJobs()
        {
            var criteria = new GetGroupJobCriteria();

            return _repository.Fetch<GroupJobEntity>(criteria);
        }

        public JobInfoModel GetJobs(bool isGetNew)
        {
            var criteria = new GetJobsCriteria() { IsGetNew = isGetNew };

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

        public bool DeleteJobToDb(string jobName, string groupName)
        {
            var criteria = new DeleteJobCriteria { JobName = jobName, GroupName = groupName };

            return _repository.Execute(criteria) == 1;
        }
       
    }
}
