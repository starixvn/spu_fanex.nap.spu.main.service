namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.Services
{
    using Refit;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface IJobService
    {
        Task<bool> Execute(string jobName, string apiUrl, string methodName, int size = 0, int numberOfBatch = 0);

        Task<bool> Execute(string jobName, string apiUrl, string methodName, string controller, Dictionary<string, object> data);

        Task<bool> Monitor(string jobName, string apiUrl, string methodName);
    }

    public class JobService : IJobService
    {
        public async Task<bool> Execute(string jobName, string apiUrl, string methodName, int size = 0, int numberOfBatch = 0)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(apiUrl),
                Timeout = TimeSpan.FromDays(1)
            };
            
            var jobServiceAPI = RestService.For<IJobServiceApi>(httpClient);

            return await jobServiceAPI.Execute(jobName, methodName, size, numberOfBatch);
        }

        public async Task<bool> Execute(string jobName, string apiUrl, string methodName, string controller, Dictionary<string, object> data)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(apiUrl),
                Timeout = TimeSpan.FromDays(1)
            };

            var jobServiceAPI = RestService.For<IJobServiceApi>(httpClient);

            return await jobServiceAPI.Execute(jobName, methodName, controller, data);
        }

        public async Task<bool> Monitor(string jobName, string apiUrl, string methodName)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(apiUrl),
                Timeout = TimeSpan.FromDays(1)
            };

            var jobServiceAPI = RestService.For<IJobServiceApi>(httpClient);

            return await jobServiceAPI.Detect(jobName, methodName);
        }
    }
}
