namespace Fanex.NAP.SPU.Main
{
    using Fanex.NAP.SPU.Main.Features.JobSchedulers.Services;
    using Fanex.NAP.SPU.Main.JobSchedulers.Services;
    using Fanex.NAP.SPU.Main.Middleware;
    using HealthChecks.UI.Client;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using ONEbook.UM.AspNetCore;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Helpers;
    using ONEbook.UM.Services;
    using ONEbook.UM.Transport;
    using Quartz;
    using Quartz.Impl;
    using Quartz.Spi;
    using Refit;
    using SilkierQuartz;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = new AppSettings(this.Configuration);

            services.AddControllersWithViews();

            services.AddSingleton<IAppSettings>(appSettings);

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddSingleton(new UMClientConfiguration().Initialize(Configuration));
            services.AddSingleton<IUMCacheService, UMCacheService>();
            services.AddSingleton<IUMRestClient, UMRestClient>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IHttpContextHelper, HttpContextHelper>();
            services.AddSingleton<IMaintenanceService, MaintenanceService>();

            services
                .AddHealthChecks();

            
            services.AddRazorPages();
            services.AddSilkierQuartz();
            services.AddOptions();

            services.AddRefitClient<IJobServiceApi>();
            services.AddSingleton<IJobService, JobService>();

            services.AddSingleton<QuartzJobRunner>();
            services.AddSingleton<QuartzHostedService>();
            services.AddHostedService<QuartzHostedService>();
            services.AddSingleton<JobExecute>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseHealthChecks("/healthz", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseSilkierQuartzAuthentication();
            app.UseAuthorization();

            var schedulerProvider = (ISchedulerFactory)app.ApplicationServices.GetService(typeof(ISchedulerFactory));

            app.UseSilkierQuartz(
                new SilkierQuartzOptions()
                {

                    VirtualPathRoot = "/quartz",                                        
                    Scheduler = schedulerProvider.GetScheduler().Result,
                    UseLocalTime = true,
                    DefaultDateFormat = "MM/dd/yyyy",
                    DefaultTimeFormat = "HH:mm:ss",
                    CronExpressionOptions = new CronExpressionDescriptor.Options()
                    {
                        DayOfWeekStartIndexZero = false
                    },
                    AccountName = "admin",
                    AccountPassword = "admin!Q@W#E",
                    IsAuthenticationPersist = false
                }
                );

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            //app.UseMiddleware<ExceptionHandlerMiddleware>();

        }

    }
}
