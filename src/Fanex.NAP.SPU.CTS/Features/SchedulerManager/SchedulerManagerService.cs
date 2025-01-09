namespace Fanex.NAP.SPU.CTS.Features.SchedulerManager
{
    using Fanex.NAP.SPU.CTS.Features.JobSchedulers.Model;
    using Fanex.NAP.SPU.CTS.Features.SchedulerManager.Model;
    using System.Collections.Generic;

    public interface ISchedulerManagerService
    {
        bool AddJobToDb(string jobTriggerJson);

        bool UpdateJobToDb(string jobTriggerJson);

        IEnumerable<GroupJobEntity> GetGroupJobs();

        IEnumerable<GroupJob> GetGroupJobTriggers(bool isGetNew);

        bool ClearedJobNew(string jobName, string groupName);      

        bool DeleteJobToDb(string jobName, string groupName);        

        Job? GetSingleJob(string jobName, string groupName);
    }

    public class SchedulerManagerService : ISchedulerManagerService 
    {
        private readonly ISchedulerManagerDataAccess _dataAccess;

        public SchedulerManagerService(ISchedulerManagerDataAccess dataAccess) 
        {
            _dataAccess = dataAccess;
        }

        public bool AddJobToDb(string jobTriggerJson)
        {
            return _dataAccess.AddJobToDb(jobTriggerJson);
        }

        public bool UpdateJobToDb(string jobTriggerJson)
        {
            return _dataAccess.UpdateJobToDb(jobTriggerJson);
        }

        public IEnumerable<GroupJobEntity> GetGroupJobs()
        {
            return _dataAccess.GetGroupJobs();
        }

        public IEnumerable<GroupJob> GetGroupJobTriggers(bool isGetNew)
        {
            var groupJobs = new List<GroupJob>();

            var jobInfoDb = _dataAccess.GetJobs(isGetNew);

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

        public bool DeleteJobToDb(string jobName, string groupName) 
        {
            return _dataAccess.DeleteJobToDb(jobName, groupName);
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
