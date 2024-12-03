namespace Fanex.NAP.SPU.Main.Features.SchedulerManager.Controller
{
    using Fanex.NAP.SPU.Main.JobSchedulers.Services;
    using Fanex.NAP.SPU.Main.Middleware;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using System.Threading;
    using System.Threading.Tasks;

    [Authorize(AuthenticationSchemes = "AdminScheme")]
    public class SchedulerManagerController : Controller
    {
        private readonly QuartzHostedService _quartzHostedService;
        private readonly IAppSettings _appSettings;
        private readonly IConfiguration _configuration;

        public SchedulerManagerController(QuartzHostedService quartzHostedService           
            , IAppSettings appSettings
            , IConfiguration configuration)
        {
            _quartzHostedService = quartzHostedService;
            _appSettings = appSettings;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RestartSchedulerFromDb(CancellationToken cancellationToken)
        {
            await _quartzHostedService.RestartSchedulerFromDb(cancellationToken);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> LoadJobsUpdateFromDb(CancellationToken cancellationToken)
        {           
            var groupJobs = await _quartzHostedService.LoadJobsUpdateFromDb(cancellationToken);

            return Json(new
            {
                groupJobs
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateJobsFromDb(CancellationToken cancellationToken)
        {
            await _quartzHostedService.UpdateJobsFromDb(cancellationToken);
           
            return Ok();         
        }

        [HttpPost]
        public async Task<IActionResult> UpdateJob(string jobName, string groupName, CancellationToken cancellationToken)
        {
            await _quartzHostedService.UpdateJob(jobName, groupName, cancellationToken);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteJob(string jobName, string groupName, CancellationToken cancellationToken)
        {
            await _quartzHostedService.DeleteJob(jobName, groupName, cancellationToken);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAllJobs(CancellationToken cancellationToken)
        {
            await _quartzHostedService.DeleteAllJobs();

            return Ok();
        }   
        
        [HttpPost]
        public async Task<IActionResult> UpdateFlagJobUpdate()
        {
            await Task.FromResult(_quartzHostedService.UpdateFlagJobUpdate(true));

            return Ok();
        }
    }
}
