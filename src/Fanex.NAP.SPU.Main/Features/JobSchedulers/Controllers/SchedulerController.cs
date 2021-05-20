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
        
        private readonly QuartzHostedService _quartzHostedService;
        private readonly IAppSettings _appSettings;
        private readonly IConfiguration _configuration;

        public SchedulerController(QuartzHostedService quartzHostedService
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

        [HttpGet]
        public IActionResult CheckUpdateJobs()
        {
           
            return View(_quartzHostedService.GetJobSchedulerDiff()) ;
        }

        [HttpGet]
        public async Task<IActionResult> SaveUpdateJobs()
        {
            _quartzHostedService.UpdateScheduleJob();

            return RedirectToAction("CheckUpdateJobs") ;
        }        
    }
}
