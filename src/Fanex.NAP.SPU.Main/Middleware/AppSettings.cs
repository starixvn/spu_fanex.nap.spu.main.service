using Fanex.NAP.SPU.Main.Features.JobSchedulers.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel;

namespace Fanex.NAP.SPU.Main.Middleware
{
    public interface IAppSettings
    {
        string HealthCheckStorage { get; }

        int HostingPort { get; }

        Schedulers Schedulers { get; }
    }

    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration configuration;

        public AppSettings(IConfiguration configuration)
        {
            this.configuration = configuration;

            HealthCheckStorage = GetValue<string>(nameof(HealthCheckStorage));

            HostingPort = GetValue<int>(nameof(HostingPort));

            Schedulers = GetSection<Schedulers>(nameof(Schedulers));
        }

        public string HealthCheckStorage { get; }

        public int HostingPort { get; }

        public Schedulers Schedulers { get; }

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
