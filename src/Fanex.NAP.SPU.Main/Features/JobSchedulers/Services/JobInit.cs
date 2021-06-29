namespace Fanex.NAP.SPU.Main.JobSchedulers.Services
{
    using Fanex.NAP.SPU.Main.Features.Healthchecks.Services;
    using Fanex.NAP.SPU.Main.Middleware;
    using Microsoft.Extensions.DependencyInjection;
    using System.Linq;

    public static class JobInit
    {
        public static void AddSingletonJob(this IServiceCollection services, IAppSettings appSettings)
        {
           // var schedulers = appSettings.Schedulers;

            //if (schedulers != null && schedulers.GroupJobs.Any())
            //{
            //    services.AddSingletonHealthCheck(schedulers);
            //}
        }
    }
}
