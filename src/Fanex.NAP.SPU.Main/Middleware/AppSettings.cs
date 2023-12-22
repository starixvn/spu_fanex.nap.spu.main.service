using Fanex.NAP.SPU.Main.Features.JobSchedulers.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel;

namespace Fanex.NAP.SPU.Main.Middleware
{
    public interface IAppSettings
    {
        string HealthCheckStorage { get; }

        string Redis { get; }

        int HostingPort { get; }

        Scheduler Schedulers { get; }

        int Environment { get; }

        string EnvironmentString { get; }

        string Proxy { get; }
    }

    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration configuration;
        private readonly string environment;

        public AppSettings(IConfiguration configuration)
        {
            this.configuration = configuration;

            HealthCheckStorage = GetValue<string>(nameof(HealthCheckStorage));

            HostingPort = GetValue<int>(nameof(HostingPort));

            Schedulers = GetSection<Scheduler>(nameof(Schedulers));

            environment = GetValue<string>(nameof(Environment));

            Redis = GetValue<string>(nameof(Redis));


            Proxy = GetValue<string>(nameof(Proxy));
        }

        public string HealthCheckStorage { get; }

        public int HostingPort { get; }

        public Scheduler Schedulers { get; }

        public int Environment
        {
            get
            {
                switch (environment)
                {
                    case "LOCAL":
                        return 1;
                    case "UAT":
                        return 2;
                    default:
                        return 3;
                }
            }
        }

        public string Redis { get; }

        public string Proxy { get; }

        public string EnvironmentString => environment;

        public T GetValue<T>(string key)
        {
            try
            {
                var value = configuration[key];

                var typeConverter = TypeDescriptor.GetConverter(typeof(T));

                return (T)typeConverter.ConvertFromString(value);
            }
            catch (Exception ex)
            {
                throw new InvalidCastException($"Key: {key}", ex);
            }
        }

        public T GetSection<T>(string key)
        {
            var schedulerSection = configuration.GetSection(key);

            return schedulerSection.Get<T>();
        }

    }
}
