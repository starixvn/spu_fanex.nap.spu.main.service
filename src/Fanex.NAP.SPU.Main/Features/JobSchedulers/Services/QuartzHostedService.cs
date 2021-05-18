namespace Fanex.NAP.SPU.Main.JobSchedulers.Services
{
    using Fanex.NAP.SPU.Main.Features.JobSchedulers.Models;
    using Fanex.NAP.SPU.Main.Middleware;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Quartz;
    using Quartz.Spi;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class QuartzHostedService : IHostedService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;
        private readonly IConfiguration _configuration;
        private readonly IAppSettings _appSettings;
        private readonly IScheduler _scheduler;
        private readonly ISentryClient sentryClient;


        public QuartzHostedService(
            ISchedulerFactory schedulerFactory,
            IJobFactory jobFactory,
            IConfiguration _configuration,
            IAppSettings _appSettings,
            ISentryClient sentryClient)
        {
            this.sentryClient = sentryClient;
            _schedulerFactory = schedulerFactory;
            _jobFactory = jobFactory;
            this._configuration = _configuration;
            this._appSettings = _appSettings;
            _scheduler = _schedulerFactory.GetScheduler().Result;
            JobSchedulers = _appSettings.Schedulers;

        }

        public Scheduler JobSchedulers { get; set; }

        public Scheduler NewJobSchedulers
        {
            get
            {
                return _configuration.GetSection(nameof(_appSettings.Schedulers)).Get<Scheduler>();
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
            if (JobSchedulers.GroupJobs != null && JobSchedulers.GroupJobs.Any())
            {
                _scheduler.JobFactory = _jobFactory;

                var groupJobs = JobSchedulers.GroupJobs;
                                
                foreach (var groupJob in groupJobs)
                {
                    groupJob.Jobs.ToList().ForEach(job =>
                    {
                        var jobDetail = BuildJob(job, groupJob.Name);

                        sentryClient.CaptureMessage(job.JobName + " - " + job.APIUrl);

                        job.Triggers.ToList().ForEach(trigger =>
                        {
                            var triggerDetail = BuildTrigger(trigger);

                            _scheduler.ScheduleJob(jobDetail, triggerDetail, cancellationToken);

                            if (!job.Active)
                            {
                                _scheduler.PauseTrigger(new TriggerKey(trigger.TriggerName));
                            }
                        });

                    });
                }

                await _scheduler.Start(cancellationToken);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

            await scheduler.Shutdown(cancellationToken);
        }

        public IEnumerable<GroupJob> GetJobSchedulerDiff()
        {
            var result = JobSchedulers.Equals(NewJobSchedulers);

            if (!result)
            {
                var newGroupJobScheduler = NewJobSchedulers.GroupJobs.Where(x => !JobSchedulers.GroupJobs.Any(a => a.Id == x.Id));

                if (newGroupJobScheduler != null && newGroupJobScheduler.Any())
                {
                    return newGroupJobScheduler.Select(x=> {
                        x.NewJobs = x.Jobs;
                        return x;
                    });
                }
                else
                {
                    return NewJobSchedulers.GroupJobs.Select(groupJob =>
                    {
                        var newJobs = groupJob.Jobs;
                        var oldJobs = JobSchedulers.GroupJobs.FirstOrDefault(x => x.Id == groupJob.Id).Jobs;

                        var f1Jobs = newJobs.Where(f1 => oldJobs.All(f2 => !f1.Equals(f2)));
                        var f2Jobs = newJobs.Count() != oldJobs.Count() ? oldJobs.Where(f1 => newJobs.All(f2 => !f1.Equals(f2))) : new List<Job>();

                        groupJob.Jobs = f1Jobs.Concat(f2Jobs);

                        groupJob.ExistingJobs = f1Jobs.Where(x => oldJobs != null && oldJobs.Any(a => a.JobId == x.JobId));
                        groupJob.NewJobs = f1Jobs.Where(x => oldJobs != null && !oldJobs.Any(a => a.JobId == x.JobId));

                        groupJob.DeletedJobs = oldJobs.Where(x => !newJobs.Any(a => a.JobId == x.JobId));

                        return groupJob;
                    });
                }

            }

            return new List<GroupJob>();
        }

        public void ReScheduleJob()
        {
            var groupJobs = GetJobSchedulerDiff();

            if (groupJobs != null && groupJobs.Any())
            {
                var tasks = new List<Task>();

                groupJobs.ToList().ForEach(groupJob =>
                {
                    tasks.Add(Task.Run(() => UpdateJob(groupJob)));
                    tasks.Add(Task.Run(() => AddNewJob(groupJob)));
                    tasks.Add(Task.Run(() => DeleteJob(groupJob)));
                });

                Task.WhenAll(tasks);

                JobSchedulers = NewJobSchedulers;
            }           
        }
        private void DeleteJob(GroupJob groupJob)
        {
            if (groupJob.DeletedJobs != null && groupJob.DeletedJobs.Any())
            {
                groupJob.DeletedJobs.ToList().ForEach(job =>
                {
                    _scheduler.DeleteJob(new JobKey(job.JobName, groupJob.Name));
                });
            }
        }

        private void AddNewJob(GroupJob groupJob)
        {
            if (groupJob.NewJobs != null && groupJob.NewJobs.Any())
            {
                groupJob.NewJobs.ToList().ForEach(job =>
                {
                    var buildJob = BuildJob(job, groupJob.Name);

                    job.Triggers.ToList().ForEach(trigger =>
                    {

                        var buildTrigger = BuildTrigger(trigger);

                        _scheduler.ScheduleJob(buildJob, buildTrigger);

                        if (!job.Active)
                        {
                            _scheduler.PauseTrigger(new TriggerKey(trigger.TriggerName));
                        }
                    });
                });
            }
        }

        private void UpdateJob(GroupJob groupJob)
        {
            if (groupJob.ExistingJobs != null && groupJob.ExistingJobs.Any())
            {
                groupJob.ExistingJobs.ToList().ForEach(job =>
                {
                    job.Triggers.ToList().ForEach(trigger =>
                    {

                        if (!job.Active)
                        {
                            _scheduler.PauseTrigger(new TriggerKey(trigger.TriggerName));
                        }
                        else
                        {
                            _scheduler.RescheduleJob(new TriggerKey(trigger.TriggerName), BuildTrigger(trigger));
                        }
                    });

                });
            }
        }

        private IJobDetail BuildJob(Job job, string groupName)
        {
            return JobBuilder
                .Create(Type.GetType(job.JobType))
                .WithIdentity(job.JobName, groupName)
                .WithDescription(job.JobName)
                .UsingJobData("apiUrl", job.APIUrl)
                .UsingJobData("methodName", job.MethodName)
                .Build();
        }

        private ITrigger BuildTrigger(Trigger trigger)
        {
            return TriggerBuilder
                .Create()
                .WithIdentity(trigger.TriggerName)
                .WithDescription(trigger.TriggerName)
                .WithCronSchedule(trigger.CronExpression)
                .Build();
        }

    }
}
