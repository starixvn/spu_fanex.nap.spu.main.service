namespace Fanex.NAP.SPU.Main.JobSchedulers.Services
{
    using Fanex.NAP.SPU.CTS.Features.JobSchedulers.Model;
    using Fanex.NAP.SPU.CTS.Features.SchedulerManager;
    using Fanex.NAP.SPU.CTS.Shares;
    using Fanex.NAP.SPU.Main.Features.SchedulerManager.Service;
    using Fanex.NAP.SPU.Main.Features.SchedulerManager.ViewModel;
    using Fanex.NAP.SPU.Main.Middleware;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Newtonsoft.Json;
    using Quartz;
    using Quartz.Impl;
    using Quartz.Impl.Matchers;
    using Quartz.Spi;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    public class QuartzHostedService : IHostedService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;
        private readonly IConfiguration _configuration;
        private readonly IAppSettings _appSettings;
        private readonly IScheduler _scheduler;
        private readonly ISentryClient _sentryClient;
        private readonly ISchedulerManagerJobService _schedulerManagerJobService;

        public QuartzHostedService(
            ISchedulerFactory schedulerFactory,
            IJobFactory jobFactory,
            IConfiguration configuration,
            IAppSettings appSettings,
            ISentryClient sentryClient,
            ISchedulerManagerJobService schedulerManagerJobService)
        {
            _sentryClient = sentryClient;
            _schedulerFactory = schedulerFactory;
            _jobFactory = jobFactory;
            _configuration = configuration;
            _appSettings = appSettings;
            _schedulerManagerJobService = schedulerManagerJobService;
            _scheduler = _schedulerFactory.GetScheduler().Result;
           
            JobSchedulers = appSettings.Schedulers;           
        }

        public Scheduler JobSchedulers { get; set; }      

        public IScheduler Scheduler
        {
            get
            {
                return _scheduler;
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await RestartSchedulerFromDb(cancellationToken);            
        }

        private bool IsPauseTrigger(IEnumerable<Job> jobs)
        {
            return !jobs.Any(x => x.IsStartByDefault);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {          
            if (!_scheduler.IsShutdown)
            {
                await _scheduler.Shutdown(cancellationToken);
            }
        }

        public async Task RestartAsync(CancellationToken cancellationToken)
        {
            if (!_scheduler.IsShutdown)
            {
                await _scheduler.Start(cancellationToken);
            }
        }

        public async Task RestartSchedulerFromDb(CancellationToken cancellationToken)
        {
            var groupJobs = _schedulerManagerJobService.GetGroupJobTriggers(false);

            await UpdateJobs(cancellationToken, groupJobs);

            await RestartAsync(cancellationToken);
        }

        public async Task<IEnumerable<GroupJobEntity>> GetGroupJobs()
        {
            var groupJobs = _schedulerManagerJobService.GetGroupJobs();

            return await Task.FromResult(groupJobs);
        }

        public async Task<IEnumerable<GroupJob>> GetJobsFromDb(CancellationToken cancellationToken)
        {            
            var groupJobs = _schedulerManagerJobService.GetGroupJobTriggers(true);

            return await Task.FromResult(groupJobs);
        }
       
        private async Task UpdateJobs(CancellationToken cancellationToken, IEnumerable<GroupJob> groupJobs)
        {
            foreach (var groupJob in groupJobs)
            {
                groupJob.Jobs.ToList().ForEach(job =>
                {
                    _schedulerManagerJobService.ReplaceJob(job, groupJob.Name);
                });

                if (IsPauseTrigger(groupJob.Jobs))
                {
                    await _scheduler.PauseTriggers(GroupMatcher<TriggerKey>.GroupEquals(groupJob.Name));
                }
            }           
        }

        public async Task ReplaceJob(CancellationToken cancellationToken)
        {
            if (JobSchedulers.GroupJobs != null && JobSchedulers.GroupJobs.Any())
            {               
                var groupJobs = JobSchedulers.GroupJobs;

                foreach (var groupJob in groupJobs)
                {
                    groupJob.Jobs.ToList().ForEach(job =>
                    {
                        _schedulerManagerJobService.ReplaceJob(job, groupJob.Name);                      
                    });

                    if (IsPauseTrigger(groupJob.Jobs))
                    {
                        await _scheduler.PauseTriggers(GroupMatcher<TriggerKey>.GroupEquals(groupJob.Name));
                    }
                }

                await _scheduler.Start(cancellationToken);
            }            
        }

        public async Task ApplyNewJobFromDb(string jobName, string groupName, CancellationToken cancellationToken)
        {
            var newJob = _schedulerManagerJobService.GetSingleJob(jobName, groupName);

            if (newJob != null)
            {
                await _schedulerManagerJobService.ReplaceJob(newJob, groupName);

                _schedulerManagerJobService.ClearedJobNew(jobName, groupName);
            }

            await _scheduler.Start(cancellationToken);
        }

        public async Task ReloadJobFromDb(string jobName, string groupName, CancellationToken cancellationToken)
        {
            var newJob = _schedulerManagerJobService.GetSingleJob(jobName, groupName);

            if (newJob != null)
            {
                await _schedulerManagerJobService.ReplaceJob(newJob, groupName);                
            }

            await _scheduler.Start(cancellationToken);
        }

        public Job GetSingleJob(string jobName, string groupName)
        {
            return _schedulerManagerJobService.GetSingleJob(jobName, groupName);        
        }

        public async Task AddJobToDb(AddJobViewModel addJobModel, string environmentDbIDs, CancellationToken cancellationToken)
        {
            var data = new List<AddJobViewModel>();

            var environmentIDs = environmentDbIDs.Split(',');

            foreach (var environmentDbID in environmentIDs)
            {
                var obj = addJobModel.Clone();
                obj.EnvironmentID = int.Parse(environmentDbID);    

                data.Add(obj);
            }

            await _schedulerManagerJobService.AddJobToDb(JsonConvert.SerializeObject(data));
        }

        public async Task UpdateJobToDb(AddJobViewModel updateJobModel, string environmentDbIDs, CancellationToken cancellationToken)
        {
            var data = new List<AddJobViewModel>();

            var environmentIDs = environmentDbIDs.Split(',');

            foreach (var environmentDbID in environmentIDs)
            {               
                var obj = updateJobModel.Clone();
                obj.EnvironmentID = int.Parse(environmentDbID);

                data.Add(obj);             
            }

            await _schedulerManagerJobService.UpdateJobToDb(JsonConvert.SerializeObject(data));
        }

        public async Task DeleteJobToDb(string jobName, string groupName, CancellationToken cancellationToken)
        {
            var isSuccess = await _schedulerManagerJobService.DeleteJobToDb(jobName, groupName);         
        }                

    }
}
