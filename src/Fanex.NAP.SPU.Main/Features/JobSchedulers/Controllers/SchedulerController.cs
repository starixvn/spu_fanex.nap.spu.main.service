namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.Controllers
{    
    using Fanex.NAP.SPU.Main.Features.JobSchedulers.Models;
    using Fanex.NAP.SPU.Main.Features.JobSchedulers.ViewModels.Job;
    using Fanex.NAP.SPU.Main.JobSchedulers.Services;
    using Fanex.NAP.SPU.Main.Middleware;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class SchedulerController : Controller
    {
        private readonly JobSchedules _jobSchedules;
        private readonly QuartzHostedService _quartzHostedService;
        private readonly IAppSettings _appSettings;
        private readonly IConfiguration _configuration;

        public SchedulerController(JobSchedules jobSchedules
            , QuartzHostedService quartzHostedService
            , IAppSettings appSettings
            , IConfiguration configuration)
        {
            _jobSchedules = jobSchedules;
            _quartzHostedService = quartzHostedService;
            _appSettings = appSettings;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CheckUpdateJobs()
        {
            var schedulerModel = _configuration.GetSection(nameof(_appSettings.Schedulers)).Get<Schedulers>();

            foreach (var group in schedulerModel.GroupJobs)
            {
                CheckUpdateJob(group.Jobs, _quartzHostedService.CurrentJobSchedules.JobScheduleList);
            }

            return View(schedulerModel.GroupJobs);
        }

        private void CheckUpdateJob(IEnumerable<Job> jobs, List<JobScheduleViewModel> currentJobSchedules)
        {
            foreach (var job in jobs)
            {
                var isNoUpdateJob = currentJobSchedules.Any(x => x.JobName == job.JobName
                        && x.JobType.FullName == job.JobType
                        && x.APIUrl == job.APIUrl
                        && x.IsActive == job.Active);

                job.UpdateStatus = isNoUpdateJob ? UpdateStatus.NoUpdate : UpdateStatus.Update;

                if (job.UpdateStatus == UpdateStatus.NoUpdate)
                {
                    CheckUpdateTrigger(job, job.Triggers, currentJobSchedules);
                }
            }
        }

        private void CheckUpdateTrigger(Job job, IEnumerable<Trigger> triggers, List<JobScheduleViewModel> currentJobSchedules)
        {
            foreach (var trigger in triggers)
            {
                var isTriggerNoUpdate = currentJobSchedules.Any(x => x.TriggerName == trigger.TriggerName
                    && x.TriggerType == trigger.TriggerType
                    && x.RepeatForever == trigger.RepeatForever
                    && x.CronExpression == trigger.CronExpression);

                trigger.UpdateStatus = isTriggerNoUpdate ? UpdateStatus.NoUpdate : UpdateStatus.Update;

                if (trigger.UpdateStatus == UpdateStatus.Update)
                {                    
                    job.UpdateStatus = UpdateStatus.Update;
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> SaveUpdateJobs()
        {
            var schedulerModel = _appSettings.Schedulers;

            await _quartzHostedService.UpdateSchedule(schedulerModel);

            var viewModel = new SaveUpdateJobViewModel { Message = "Save Successfully" };

            return View(viewModel);
        }               
           
    }
}
