using Fanex.NAP.SPU.Main;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Sentry;
using SilkierQuartz;
using System;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureAppConfiguration((hostingContext, config) =>
{
    var env = hostingContext.HostingEnvironment;

    config.SetBasePath(env.ContentRootPath);
    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

    config.AddEnvironmentVariables();
});


builder.ConfigureWebHostDefaults(webBuilder =>
{
    webBuilder.UseUrls("http://*:5000");

    webBuilder
    .UseSentry()
    .UseStartup<Startup>();
});

builder.ConfigureSilkierQuartzHost();

SentrySdk.CaptureMessage(string.Format("{0} has been started at {1}", typeof(Program).FullName, DateTime.Now));

var app = builder.Build();

app.Run();