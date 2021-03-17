namespace Fanex.NAP.SPU.Main.JobSchedulers.Services
{
    using Fanex.NAP.SPU.Main.Features.Healthchecks.Services;
    using Fanex.NAP.SPU.Main.Features.JobSchedulers.Models;
    using Fanex.NAP.SPU.Main.Features.JobSchedulers.ViewModels.Job;
    using Fanex.NAP.SPU.Main.Middleware;
    using Microsoft.Extensions.DependencyInjection;
    using System.Collections.Generic;
    using System.Linq;

    public static class JobInit
    {
        public static void AddSingletonJob(this IServiceCollection services, AppSettings appSettings)
        {
            var schedule = appSettings.Schedulers;

            if (schedule != null && schedule.GroupJobs.Any())
            {
                var jobSchedules = GetJobSchedules(schedule.GroupJobs);

                services.AddSingleton(jobSchedules);

                services.AddSingletonHealthCheck(jobSchedules);
            }
        }

        public static JobSchedules GetJobSchedules(IEnumerable<GroupJob> groupJobList)
        {
            var jobScheduleList = new List<JobScheduleViewModel>();

            foreach (var group in groupJobList)
            {
                foreach (var job in group.Jobs)
                {
                    foreach (var trigger in job.Triggers)
                    {
                        jobScheduleList.Add(new JobScheduleViewModel(group.GroupName, job, trigger));
                    }
                }
            }

            return new JobSchedules(jobScheduleList);
        }

    }
}
