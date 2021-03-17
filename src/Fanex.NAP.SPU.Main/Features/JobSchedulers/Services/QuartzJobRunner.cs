namespace Fanex.NAP.SPU.Main.JobSchedulers.Services
{
    using Microsoft.Extensions.DependencyInjection;
    using ONEbook.UM.Services;
    using Quartz;
    using System;
    using System.Threading.Tasks;

    [DisallowConcurrentExecution]
    public class QuartzJobRunner : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMaintenanceService _maintenanceService;

        public QuartzJobRunner(IServiceProvider serviceProvider, IMaintenanceService maintenanceService)
        {
            _serviceProvider = serviceProvider;
            _maintenanceService = maintenanceService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var isUM = _maintenanceService.GetUnderMaintenanceTime().IsUnderMaintenanceTime;

            if (!isUM)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var job = scope.ServiceProvider.GetRequiredService(context.JobDetail.JobType) as IJob;
                    
                    await job.Execute(context);
                }
            }
        }        

    }
}
