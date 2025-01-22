namespace Fanex.NAP.SPU.Main.Features.SchedulerManager.Controller
{
    using Fanex.NAP.SPU.Main.Features.JobSchedulers.ViewModels.Job;
    using Fanex.NAP.SPU.Main.Features.SchedulerManager.ViewModel;
    using Fanex.NAP.SPU.Main.JobSchedulers.Services;
    using Fanex.NAP.SPU.Main.Middleware;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    [Authorize(AuthenticationSchemes = "AdminScheme")]
    public class SchedulerManagerController : Controller
    {
        private readonly ISentryClient _sentryClient;
        private readonly QuartzHostedService _quartzHostedService;
        private readonly IAppSettings _appSettings;
        private readonly IConfiguration _configuration;

        public SchedulerManagerController(ISentryClient sentryClient
            , QuartzHostedService quartzHostedService           
            , IAppSettings appSettings
            , IConfiguration configuration)
        {
            _sentryClient = sentryClient;
            _quartzHostedService = quartzHostedService;
            _appSettings = appSettings;
            _configuration = configuration;           
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddJobToDb(JobViewModel jobViewModel, CancellationToken cancellationToken)
        {
            try
            {               
                await _quartzHostedService.AddJobToDb(jobViewModel, false, cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                _sentryClient.CaptureException(ex);

                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        public async Task<IActionResult> UpdateJobToDb(JobViewModel jobViewModel, CancellationToken cancellationToken)
        {
            try
            {
                await _quartzHostedService.AddJobToDb(jobViewModel, true, cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                _sentryClient.CaptureException(ex);

                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPost]
        public IActionResult GetSingleJob(string jobName, string groupName)
        {
            try
            {
                var jobTrigger = _quartzHostedService.GetSingleJob(jobName, groupName);

                return Json(new { jobTrigger });
            }
            catch (Exception ex)
            {
                _sentryClient.CaptureException(ex);

                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPost]
        public IActionResult CheckExistJob(string jobName, string groupName, string dbEnvironmentIDs)
        {
            try
            {
                var jobs = _quartzHostedService.CheckExistJob(jobName, groupName, dbEnvironmentIDs);

                return Json(new { jobs });
            }
            catch (Exception ex)
            {
                _sentryClient.CaptureException(ex);

                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetGroupJobs()
        {
            try
            {
                var groups = await _quartzHostedService.GetGroupJobs();

                return Json(new { groups });
            }
            catch (Exception ex)
            {
                _sentryClient.CaptureException(ex);

                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetJobsFromDb(CancellationToken cancellationToken)
        {
            try
            {
                var groupJobs = await _quartzHostedService.GetJobsFromDb(cancellationToken);

                return Json(new { groupJobs });
            }
            catch (Exception ex)
            {
                _sentryClient.CaptureException(ex);

                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }

        }

        [HttpPost]
        public async Task<IActionResult> RestartSchedulerFromDb(CancellationToken cancellationToken)
        {
            try
            {
                await _quartzHostedService.RestartSchedulerFromDb(cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                _sentryClient.CaptureException(ex);

                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> ApplyAdjustJobFromDb(string jobName, string groupName, CancellationToken cancellationToken)
        {
            try
            {
                await _quartzHostedService.ApplyAdjustJobFromDb(jobName, groupName, cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                _sentryClient.CaptureException(ex);

                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ReloadJobFromDb(string jobName, string groupName, CancellationToken cancellationToken)
        {
            try
            {
                await _quartzHostedService.ReloadJobFromDb(jobName, groupName, cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                _sentryClient.CaptureException(ex);

                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }

        }

        [HttpPost]
        public async Task<IActionResult> DeleteJobToDb(string jobName, string groupName, CancellationToken cancellationToken)
        {
            try
            {
                await _quartzHostedService.DeleteJobToDb(jobName, groupName, cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                _sentryClient.CaptureException(ex);

                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }
     
    }
}
