using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.Services
{
    public static class JobExtension
    {

        public static Task UpdateJob(this IScheduler scheduler, JobKey jobKey, IJobDetail newJob)
        {
            return Task.Run(async () =>
            {
                // get existing triggers associated with job
                var triggers = await scheduler.GetTriggersOfJob(jobKey);

                // set new job to all triggers
                triggers = triggers.Select(t =>
                {
                    var b = t.GetTriggerBuilder().ForJob(newJob.Key);
                    if (t.StartTimeUtc < DateTimeOffset.UtcNow) b.StartNow();
                    return b.Build();
                }).ToArray();

                // delete old job
                await scheduler.DeleteJob(jobKey);

                // save new job with triggers
                await scheduler.ScheduleJob(newJob, triggers, replace: true);
            });
        }
    }
}
