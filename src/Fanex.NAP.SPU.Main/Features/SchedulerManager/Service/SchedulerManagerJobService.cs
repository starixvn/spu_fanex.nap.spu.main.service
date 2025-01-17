namespace Fanex.NAP.SPU.Main.Features.SchedulerManager.Service
{
    using Fanex.NAP.SPU.CTS.Features.JobSchedulers.Model;
    using Fanex.NAP.SPU.CTS.Features.SchedulerManager;
    using Fanex.NAP.SPU.CTS.Features.SchedulerManager.Model;
    using Fanex.NAP.SPU.CTS.Shares;
    using Fanex.NAP.SPU.Main.Features.SchedulerManager.ViewModel;
    using Fanex.NAP.SPU.Main.Middleware;
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

        Task ReplaceJob(Job job, string groupName);

        Task AddJobToDb(string jobTriggerJson);

        Task UpdateJobToDb(string jobTriggerJson);

        Task<bool> DeleteJobToDb(string jobName, string groupName);     

        IEnumerable<GroupJobEntity> GetGroupJobs();

        Job GetSingleJob(string jobName, string groupName);

        IEnumerable<GroupJob> GetGroupJobTriggers(bool isGetAdjust);

        Task<HashSet<string>> GetJobsFromScheduler();    

        bool ClearedJobNew(string jobName, string groupName);
    }

    public class SchedulerManagerJobService : ISchedulerManagerJobService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IScheduler _scheduler;
        private readonly ISchedulerManagerService _schedulerManagerService;
        private readonly IAppSettings _appSettings;

        public SchedulerManagerJobService(ISchedulerFactory schedulerFactory
            , ISchedulerManagerService schedulerManagerService
            , IAppSettings appSettings
            )
        {
            _schedulerFactory = schedulerFactory;
            _schedulerManagerService = schedulerManagerService; 
            _scheduler = schedulerFactory.GetScheduler().Result;
            _appSettings = appSettings;
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

        public async Task ReplaceJob(Job job, string groupName)
        {
            var jobKey = new JobKey(job.JobName, groupName);

            await _scheduler.DeleteJob(jobKey);

            var newJobDetail = BuildJob(job, groupName);

            foreach (var trigger in job.Triggers)
            {
                var newTriggerDetail = BuildTrigger(trigger, groupName);

                var triggerKey = new TriggerKey(trigger.TriggerName, groupName);

                await _scheduler.ScheduleJob(newJobDetail, newTriggerDetail);

                if (!job.IsStartByDefault)
                {
                    await _scheduler.PauseTrigger(new TriggerKey(trigger.TriggerName, groupName));
                }
                else
                {                    
                    await _scheduler.ResumeJob(jobKey);
                }
            }           
        }

        public async Task AddJobToDb(string jobTriggerJson)
        {
            _schedulerManagerService.AddJobToDb(jobTriggerJson);
        }

        public async Task UpdateJobToDb(string jobTriggerJson)
        {
            _schedulerManagerService.UpdateJobToDb(jobTriggerJson);
        }        

        public async Task<bool> DeleteJobToDb(string jobName, string groupName)
        {
            var isSuccess = _schedulerManagerService.DeleteJobToDb(jobName, groupName, _appSettings.DbEnvironmentID);

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
            return _schedulerManagerService.GetSingleJob(jobName, groupName, _appSettings.DbEnvironmentID);
        }

        public IEnumerable<GroupJob> GetGroupJobTriggers(bool isGetAdjust)
        {
            return _schedulerManagerService.GetGroupJobTriggers(isGetAdjust, _appSettings.DbEnvironmentID);
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
            return _schedulerManagerService.ClearedJobNew(jobName, groupName, _appSettings.DbEnvironmentID);
        }       
    }
}
