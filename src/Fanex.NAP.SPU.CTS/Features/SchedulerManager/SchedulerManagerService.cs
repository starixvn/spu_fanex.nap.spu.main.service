namespace Fanex.NAP.SPU.CTS.Features.SchedulerManager
{
    using Fanex.NAP.SPU.CTS.Features.JobSchedulers.Model;
    using Fanex.NAP.SPU.CTS.Features.SchedulerManager.Model;
    using System.Collections.Generic;

    public interface ISchedulerManagerService
    {
        IEnumerable<GroupJobEntity> GetGroupJobs();

        IEnumerable<GroupJob> GetGroupJobTriggers();

        bool ClearedJobNew(string jobName, string groupName);

        bool UpdateFlagJobUpdate(bool isUpdate);

        bool DeleteJobToDb(string jobName, string groupName);

        bool AddJob(string jobTriggerJson);

        Job? GetSingleJob(string jobName, string groupName);
    }

    public class SchedulerManagerService : ISchedulerManagerService 
    {
        private readonly ISchedulerManagerDataAccess _dataAccess;

        public SchedulerManagerService(ISchedulerManagerDataAccess dataAccess) 
        {
            _dataAccess = dataAccess;
        }

        public IEnumerable<GroupJobEntity> GetGroupJobs()
        {
            return _dataAccess.GetGroupJobs();
        }

        public IEnumerable<GroupJob> GetGroupJobTriggers()
        {
            var groupJobs = new List<GroupJob>();

            var jobInfoDb = _dataAccess.GetJobs();

            if (jobInfoDb != null)
            {
                var groupedData = jobInfoDb.Jobs.GroupBy(x => x.GroupName);

                foreach (var groupJob in groupedData)
                {
                    groupJobs.Add(new GroupJob
                    {
                        Name = groupJob.Key,
                        Jobs = groupJob.Select(x =>
                        {
                            var job = new Job(x);
                            job.MappingTrigger(jobInfoDb.Triggers);

                            return job;
                        })
                    });
                }
            }

            return groupJobs;
        }
     
        public bool ClearedJobNew(string jobName, string groupName)
        {
            return _dataAccess.ClearedJobNew(jobName, groupName);
        }

        public bool UpdateFlagJobUpdate(bool isUpdate)
        {
            return _dataAccess.UpdateFlagJobUpdate(isUpdate);
        }

        public bool DeleteJobToDb(string jobName, string groupName) 
        {
            return _dataAccess.DeleteJobToDb(jobName, groupName);
        }

        public bool AddJob(string jobTriggerJson)
        {
            return _dataAccess.AddJob(jobTriggerJson);
        }

        public Job? GetSingleJob(string jobName, string groupName)
        {            
            var jobData = _dataAccess.GetSingleJob(jobName, groupName);

            if (jobData.Jobs.Any())
            {
                var job = new Job(jobData.Jobs.First());

                job.MappingTrigger(jobData.Triggers);

                return job;
            }

            return null;
        }
    }
}
