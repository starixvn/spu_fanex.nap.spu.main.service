namespace Fanex.NAP.SPU.CTS.Features.SchedulerManager
{
    using Fanex.NAP.SPU.CTS.Features.JobSchedulers.Model;
    using System.Collections.Generic;

    public interface ISchedulerManagerService
    {
        IEnumerable<GroupJob> GetGroupJobs(bool isUpdate);

        bool UpdateFlagJobUpdate(bool isUpdate);
    }

    public class SchedulerManagerService : ISchedulerManagerService 
    {
        private readonly ISchedulerManagerDataAccess _dataAccess;

        public SchedulerManagerService(ISchedulerManagerDataAccess dataAccess) 
        {
            _dataAccess = dataAccess;
        }

        public IEnumerable<GroupJob> GetGroupJobs(bool isUpdate)
        {
            var groupJobs = new List<GroupJob>();

            var jobInfoDb = _dataAccess.GetJobs(isUpdate);

            if (jobInfoDb != null) 
            {
                var groupedData = jobInfoDb.Jobs.GroupBy(x => x.GroupName);

                foreach ( var groupJob in groupedData)
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
     
        public bool UpdateFlagJobUpdate(bool isUpdate)
        {
            return _dataAccess.UpdateFlagJobUpdate(isUpdate);
        }
    }
}
