
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

        int Environment { get; }

        string EnvironmentString { get; }

        int DbEnvironmentID { get; }

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

            environment = GetValue<string>(nameof(Environment));

            DbEnvironmentID = GetValue<int>(nameof(DbEnvironmentID));

            Redis = GetValue<string>(nameof(Redis));

            Proxy = GetValue<string>(nameof(Proxy));
        }

        public string HealthCheckStorage { get; }

        public int HostingPort { get; }        

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
                    case "PROD":
                        return 3;
                    case "SIT":
                        return 4;
                    case "KLDR":
                        return 5;
                    default:
                        return 1;
                }
            }
        }

        public string Redis { get; }

        public string Proxy { get; }

        public string EnvironmentString => environment;

        public int DbEnvironmentID { get; }

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
