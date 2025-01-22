namespace Fanex.NAP.SPU.CTS.Features.SchedulerManager
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.Features.SchedulerManager.Criteria;
    using Fanex.NAP.SPU.CTS.Features.SchedulerManager.Model;

    public interface ISchedulerManagerDataAccess
    {
        bool AddJobToDb(string jobTriggerJson);

        bool UpdateJobToDb(string jobTriggerJson);

        IEnumerable<GroupJobEntity> GetGroupJobs();

        JobInfoModel GetJobs(bool isGetAdjust, int dbEnvironmentID);

        JobInfoModel GetSingleJob(string jobName, string groupName, int dbEnvironmentID);

        IEnumerable<JobEntity> CheckExistJob(string jobName, string groupName, string dbEnvironmentIDs);

        bool ClearedJobNew(string jobName, string groupName, int dbEnvironmentID);     

        bool DeleteJobToDb(string jobName, string groupName, int dbEnvironmentID);        
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

        public JobInfoModel GetJobs(bool isGetAdjust, int dbEnvironmentID)
        {
            var criteria = new GetJobsCriteria() { IsGetAdjust = isGetAdjust, DbEnvironmentID = dbEnvironmentID };

            (var jobData, var triggerData) = _repository.FetchMultiple<JobEntity, TriggerEntity>(criteria);

            return new JobInfoModel
            {
                Jobs = jobData,
                Triggers = triggerData
            };
        }

        public JobInfoModel GetSingleJob(string jobName, string groupName, int dbEnvironmentID)
        {
            var criteria = new GetSingleJobCriteria()
            {
                JobName = jobName,
                GroupName = groupName,
                DbEnvironmentID = dbEnvironmentID
            };

            (var jobData, var triggerData) = _repository.FetchMultiple<JobEntity, TriggerEntity>(criteria);

            return new JobInfoModel
            {
                Jobs = jobData,
                Triggers = triggerData
            };
        }

        public IEnumerable<JobEntity> CheckExistJob(string jobName, string groupName, string dbEnvironmentIDs)
        {
            var criteria = new CheckExistJobCriteria()
            {
                JobName = jobName,
                GroupName = groupName,
                EnvironmentIDs = dbEnvironmentIDs
            };

            return _repository.Fetch<JobEntity>(criteria);
        }

        public bool ClearedJobNew(string jobName, string groupName, int dbEnvironmentID)
        {
            var criteria = new ClearedJobNewCriteria
            {
                JobName = jobName,
                GroupName = groupName,
                DbEnvironmentID = dbEnvironmentID
            };

            _repository.Execute(criteria);

            return true;
        }      

        public bool DeleteJobToDb(string jobName, string groupName, int dbEnvironmentID)
        {
            var criteria = new DeleteJobCriteria 
            { 
                JobName = jobName, 
                GroupName = groupName, 
                DbEnvironmentID = dbEnvironmentID
            };

            return _repository.Execute(criteria) == 1;
        }
       
    }
}
