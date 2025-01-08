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

            // GOTO - Test, will delete
            //JobSchedulers = appSettings.Schedulers;
            JobSchedulers = _configuration.GetSection("SchedulerTests").Get<Scheduler>();
        }

        public Scheduler JobSchedulers { get; set; }

        public Scheduler NewJobSchedulers
        {
            get
            {               
                return _configuration.GetSection("NewSchedulerTests").Get<Scheduler>();
            }
        }

        public IScheduler Scheduler
        {
            get
            {
                return _scheduler;
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //if (JobSchedulers == null || JobSchedulers.GroupJobs == null)
            //{
            //    return;
            //}

            //if (JobSchedulers.GroupJobs != null && JobSchedulers.GroupJobs.Any())
            //{
            //    _scheduler.JobFactory = _jobFactory;

            //    var groupJobs = JobSchedulers.GroupJobs;

            //    foreach (var groupJob in groupJobs)
            //    {
            //        groupJob.Jobs.ToList().ForEach(job =>
            //        {
            //            var jobDetail = _schedulerManagerJobService.BuildJob(job, groupJob.Name);

            //            job.Triggers.ToList().ForEach(trigger =>
            //            {
            //                var triggerDetail = _schedulerManagerJobService.BuildTrigger(trigger, groupJob.Name);

            //                _scheduler.ScheduleJob(jobDetail, triggerDetail, cancellationToken);

            //                if (!job.Active)
            //                {
            //                    _scheduler.PauseTrigger(new TriggerKey(trigger.TriggerName, groupJob.Name));
            //                }
            //            });
            //        });

            //        if (IsPauseTrigger(groupJob.Jobs))
            //        {
            //            await _scheduler.PauseTriggers(GroupMatcher<TriggerKey>.GroupEquals(groupJob.Name));
            //        }
            //    }

            //    await _scheduler.Start(cancellationToken);
            //}

            //var groupJobs = _schedulerManagerJobService.GetJobsFromDb(false);

            //await UpdateJobs(cancellationToken, groupJobs);

            await RestartAsync(cancellationToken);
        }

        private bool IsPauseTrigger(IEnumerable<Job> jobs)
        {
            return !jobs.Any(x => x.Active);
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
            var groupJobs = _schedulerManagerJobService.GetGroupJobTriggers();

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
            var groupJobs = _schedulerManagerJobService.GetGroupJobTriggers();

            return await Task.FromResult(groupJobs);
        }

        public async Task UpdateJobsFromDb(CancellationToken cancellationToken)
        {
            var groupJobs = _schedulerManagerJobService.GetGroupJobTriggers();

            await UpdateJobs(cancellationToken, groupJobs);

            await RestartAsync(cancellationToken);
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
            if (NewJobSchedulers.GroupJobs != null && NewJobSchedulers.GroupJobs.Any())
            {               
                var groupJobs = NewJobSchedulers.GroupJobs;

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

        public async Task AddJob(AddJobViewModel addJobModel, CancellationToken cancellationToken)
        {
            var data = new List<AddJobViewModel>() { addJobModel };

            await _schedulerManagerJobService.AddJob(JsonConvert.SerializeObject(data));
        }

        public async Task DeleteJobToDb(string jobName, string groupName, CancellationToken cancellationToken)
        {
            var isSuccess = await _schedulerManagerJobService.DeleteJobToDb(jobName, groupName);         
        }

        public async Task DeleteAllJobs()
        {
            await _schedulerManagerJobService.DeleteAllJobs();
        }

        public bool UpdateFlagJobUpdate(bool isUpdate)
        {
            return _schedulerManagerJobService.UpdateFlagJobUpdate(isUpdate);
        }

        //public async Task<(List<string>, List<string>)> LoadJobsUpdateFromDb(CancellationToken cancellationToken)
        //{
        //    // Get jobs from databse
        //    //var jobsFromDb =_schedulerManagerJobService.GetJobsFromDb();

        //    //var jobsFromScheduler = await _schedulerManagerJobService.GetJobsFromScheduler();
        //    //var jobsNameFromDb = jobsFromDb.Jobs.Select(x => x.JobName).ToList();

        //    //// Compare jobs in database and scheduler
        //    //var (newJobs, removeJobs) = JobComparison.CompareJobs(jobsFromScheduler, jobsNameFromDb);

        //    //return (newJobs, removeJobs);

        //    //return new(new List<string>(), new List<string>());

        //    //await ReplaceJob(cancellationToken);
        //}

        #region Delete
        //public IEnumerable<GroupJob> GetJobSchedulerDiff()
        //{
        //    var result = JobSchedulers.Equals(NewJobSchedulers);

        //    if (!result)
        //    {
        //        var newGroupJobScheduler = NewJobSchedulers.GroupJobs.Where(x => !JobSchedulers.GroupJobs.Any(a => a.Id == x.Id));

        //        if (newGroupJobScheduler != null && newGroupJobScheduler.Any())
        //        {
        //            return newGroupJobScheduler.Select(x =>
        //            {
        //                x.NewJobs = x.Jobs;
        //                return x;
        //            });
        //        }
        //        else
        //        {
        //            return NewJobSchedulers.GroupJobs.Select(groupJob =>
        //            {
        //                var newJobs = groupJob.Jobs;
        //                var oldJobs = JobSchedulers.GroupJobs.FirstOrDefault(x => x.Id == groupJob.Id).Jobs;

        //                var f1Jobs = newJobs.Where(f1 => oldJobs.All(f2 => !f1.Equals(f2)));
        //                var f2Jobs = newJobs.Count() != oldJobs.Count() ? oldJobs.Where(f1 => newJobs.All(f2 => !f1.Equals(f2))) : new List<Job>();

        //                groupJob.Jobs = f1Jobs.Concat(f2Jobs);

        //                groupJob.ExistingJobs = f1Jobs.Where(x => oldJobs != null && oldJobs.Any(a => a.JobId == x.JobId));
        //                groupJob.NewJobs = f1Jobs.Where(x => oldJobs != null && !oldJobs.Any(a => a.JobId == x.JobId));

        //                groupJob.DeletedJobs = oldJobs.Where(x => !newJobs.Any(a => a.JobId == x.JobId));

        //                return groupJob;
        //            });
        //        }

        //    }

        //    return new List<GroupJob>();
        //}

        //public void UpdateScheduleJob()
        //{
        //    var groupJobs = GetJobSchedulerDiff();

        //    if (groupJobs != null && groupJobs.Any())
        //    {
        //        var tasks = new List<Task>();

        //        groupJobs.ToList().ForEach(groupJob =>
        //        {
        //            tasks.Add(Task.Run(() => UpdateJob(groupJob)));
        //            tasks.Add(Task.Run(() => AddNewJob(groupJob)));
        //            tasks.Add(Task.Run(() => DeleteJob(groupJob)));
        //        });

        //        Task.WhenAll(tasks);

        //        JobSchedulers = NewJobSchedulers;
        //    }
        //}

        //private void DeleteJob(GroupJob groupJob)
        //{
        //    if (groupJob.DeletedJobs != null && groupJob.DeletedJobs.Any())
        //    {
        //        groupJob.DeletedJobs.ToList().ForEach(job =>
        //        {
        //            _scheduler.DeleteJob(new JobKey(job.JobName, groupJob.Name));
        //        });
        //    }
        //}

        //private void AddNewJob(GroupJob groupJob)
        //{
        //    if (groupJob.NewJobs != null && groupJob.NewJobs.Any())
        //    {
        //        groupJob.NewJobs.ToList().ForEach(job =>
        //        {
        //            var buildJob = BuildJob(job, groupJob.Name);

        //            job.Triggers.ToList().ForEach(trigger =>
        //            {

        //                var buildTrigger = BuildTrigger(trigger, groupJob.Name);

        //                _scheduler.ScheduleJob(buildJob, buildTrigger);

        //                if (!job.Active)
        //                {
        //                    _scheduler.PauseTrigger(new TriggerKey(trigger.TriggerName, groupJob.Name));
        //                }
        //            });
        //        });
        //    }
        //}

        //private void UpdateJob(GroupJob groupJob)
        //{
        //    if (groupJob.ExistingJobs != null && groupJob.ExistingJobs.Any())
        //    {
        //        groupJob.ExistingJobs.ToList().ForEach(job =>
        //        {
        //            job.Triggers.ToList().ForEach(trigger =>
        //            {

        //                if (!job.Active)
        //                {
        //                    _scheduler.PauseTrigger(new TriggerKey(trigger.TriggerName, groupJob.Name));
        //                }
        //                else
        //                {
        //                    _scheduler.RescheduleJob(new TriggerKey(trigger.TriggerName, groupJob.Name), BuildTrigger(trigger, groupJob.Name));
        //                }
        //            });

        //        });
        //    }
        //}

        #endregion

    }
}
