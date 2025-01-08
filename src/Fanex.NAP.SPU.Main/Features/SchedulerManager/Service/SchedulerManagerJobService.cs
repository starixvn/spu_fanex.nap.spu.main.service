namespace Fanex.NAP.SPU.Main.Features.SchedulerManager.Service
{
    using Fanex.NAP.SPU.CTS.Features.JobSchedulers.Model;
    using Fanex.NAP.SPU.CTS.Features.SchedulerManager;
    using Fanex.NAP.SPU.CTS.Features.SchedulerManager.Model;
    using Fanex.NAP.SPU.CTS.Shares;
    using Fanex.NAP.SPU.Main.Features.SchedulerManager.ViewModel;
    using Microsoft.AspNetCore.Mvc;
    using Quartz;
    using Quartz.Impl.Matchers;
    using Refit;
    using SilkierQuartz.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    public interface ISchedulerManagerJobService
    {
        IJobDetail BuildJob(Job job, string groupName);

        ITrigger BuildTrigger(Trigger trigger, string groupName);

        Task UpdateJobData(string jobId, string key, string value);

        Task ReplaceJob(Job job, string groupName);

        Task AddJob(string jobTriggerJson);

        Task<bool> DeleteJobToDb(string jobName, string groupName);

        Task DeleteAllJobs();

        IEnumerable<GroupJobEntity> GetGroupJobs();

        Job GetSingleJob(string jobName, string groupName);

        IEnumerable<GroupJob> GetGroupJobTriggers();

        Task<HashSet<string>> GetJobsFromScheduler();

        bool UpdateFlagJobUpdate(bool isUpdate);

        bool ClearedJobNew(string jobName, string groupName);
    }

    public class SchedulerManagerJobService : ISchedulerManagerJobService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IScheduler _scheduler;
        private readonly ISchedulerManagerService _schedulerManagerService;

        public SchedulerManagerJobService(ISchedulerFactory schedulerFactory,
            ISchedulerManagerService schedulerManagerService)
        {
            _schedulerFactory = schedulerFactory;
            _schedulerManagerService = schedulerManagerService; 
            _scheduler = schedulerFactory.GetScheduler().Result;
        }

        public IJobDetail BuildJob(Job job, string groupName)
        {
            return JobBuilder
                .Create(Type.GetType(job.JobType))
                .WithIdentity(job.JobName, groupName)
                .WithDescription(job.JobName)
                .UsingJobData("apiUrl", job.APIUrl)
                .UsingJobData("methodName", job.MethodName)
                .UsingJobData("controller", job.Controller)
                .Build();
        }

        public ITrigger BuildTrigger(Trigger trigger, string groupName)
        {
            var triggerBuilder = TriggerBuilder
                .Create()
                .WithIdentity(trigger.TriggerName, groupName)
                .WithDescription(trigger.TriggerName)
                .StartNow();

            if (trigger.TriggerType != "Cron")
            {
                triggerBuilder.WithSimpleSchedule(x => x.WithIntervalInSeconds(trigger.IntervalInSecond).RepeatForever());
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(trigger.TimeZone))
                {
                    triggerBuilder.WithCronSchedule(trigger.CronExpression, x => x.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById(trigger.TimeZone)));
                }
                else
                {
                    triggerBuilder.WithCronSchedule(trigger.CronExpression);
                }
            }

            return triggerBuilder.Build();
        }

        public async Task UpdateJobData(string jobId, string key, string value)
        {
            await UpdateJobDataAsync(_scheduler, jobId, key, value);
        }

        public async Task ReplaceJob(Job job, string groupName)
        {
            await ReplaceJobAsync(job, groupName);
        }

        public async Task AddJob(string jobTriggerJson)
        {
            _schedulerManagerService.AddJob(jobTriggerJson);
        }

        public async Task UpdateJobDataAsync(IScheduler scheduler, string jobID, string newDataKey, string newDataValue)
        {
            var jobKey = new JobKey(jobID, "Group1");
           
            var jobDetail = await scheduler.GetJobDetail(jobKey);

            if (jobDetail != null)
            {               
                jobDetail.JobDataMap[newDataKey] = newDataValue;
               
                await scheduler.AddJob(jobDetail, true);
            }
            else
            {
                Console.WriteLine($"Job {jobID} not found.");
            }
        }

        public async Task ReplaceJobAsync(Job job, string groupName)
        {
            var jobKey = new JobKey(job.JobName, groupName);

            await _scheduler.DeleteJob(jobKey);

            var jobDetail = BuildJob(job, groupName);

            job.Triggers.ToList().ForEach(async trigger =>
            {
                var triggerDetail = BuildTrigger(trigger, groupName);

                await _scheduler.ScheduleJob(jobDetail, triggerDetail);

                if (!job.Active)
                {
                    await _scheduler.PauseTrigger(new TriggerKey(trigger.TriggerName, groupName));
                }
            });
        }

        public async Task DeleteAllJobs()
        {
            var jobKeys = await _scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());

            foreach (var jobKey in jobKeys)
            {
                await _scheduler.DeleteJob(jobKey);

                Console.WriteLine($"Deleted job: {jobKey.Name} in group: {jobKey.Group}");
            }

            Console.WriteLine("All jobs have been deleted.");
        }

        public async Task<bool> DeleteJobToDb(string jobName, string groupName)
        {
            var isSuccess = _schedulerManagerService.DeleteJobToDb(jobName, groupName);

            if (isSuccess) 
                return await _scheduler.DeleteJob(new JobKey(jobName, groupName));

            return true;
        }

        public IEnumerable<GroupJobEntity> GetGroupJobs()
        {
            return _schedulerManagerService.GetGroupJobs();
        }

        public Job GetSingleJob(string jobName, string groupName)
        {          
            return _schedulerManagerService.GetSingleJob(jobName, groupName);
        }

        public IEnumerable<GroupJob> GetGroupJobTriggers()
        {
            return _schedulerManagerService.GetGroupJobTriggers();
        }      

        public async Task<HashSet<string>> GetJobsFromScheduler()
        {
            var jobKeys = await _scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());

            var jobSet = new HashSet<string>();

            foreach (var jobKey in jobKeys)
            {
                string jobIdentifier = $"{jobKey.Name}:{jobKey.Group}";

                jobSet.Add(jobIdentifier);
            }

            return jobSet;
        }

        public bool ClearedJobNew(string jobName, string groupName)
        {
            return _schedulerManagerService.ClearedJobNew(jobName, groupName);
        }

        public bool UpdateFlagJobUpdate(bool isUpdate)
        {
            return _schedulerManagerService.UpdateFlagJobUpdate(isUpdate);
        }
    }
}
