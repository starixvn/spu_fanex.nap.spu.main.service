namespace Fanex.NAP.SPU.Main.Features.Healthchecks.Services
{
    using Fanex.NAP.SPU.Main.Features.Healthchecks.Models;
    using Fanex.NAP.SPU.Main.Features.JobSchedulers.Models;
    using Fanex.NAP.SPU.Main.JobSchedulers.Services;
    using Fanex.NAP.SPU.Main.Middleware;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;

    public static class HealthcheckInit
    {
        public static void AddSingletonHealthCheck(this IServiceCollection services, Scheduler jobSchedules)
        {
            services
               .AddHealthChecksUI(setup =>
               {
                   setup.AddHealthCheckEndpoint("Main Service", "/healthz");                                      

                   foreach (var groupJob in jobSchedules.GroupJobs)
                   {
                       setup.AddHealthCheckEndpoint(groupJob.Name, groupJob.HealthCheck);
                   }

                   setup.AddWebhookNotification("notification",
                       "WebhookNotification/send",
                        payload: JsonConvert.SerializeObject(new NotificationModel("[[LIVENESS]]: [[FAILURE]] [[DESCRIPTIONS]]", 1)),
                        restorePayload: JsonConvert.SerializeObject(new NotificationModel("[[LIVENESS]] is back to LIFE", 0)));
               })
               .AddInMemoryStorage();               
        }
    }
}
