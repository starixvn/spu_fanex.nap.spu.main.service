namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.Services
{
    using Refit;
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IJobService
    {
        Task<bool> Execute(string jobName, string apiUrl, string methodName, int size = 0, int numberOfBatch = 0);
    }

    public class JobService : IJobService
    {
        private readonly HttpClient httpClient;
        public JobService() {
            httpClient = new HttpClient();
        }
        public async Task<bool> Execute(string jobName, string apiUrl, string methodName, int size = 0, int numberOfBatch = 0)
        {            
            httpClient.Timeout = TimeSpan.FromDays(1);
            httpClient.BaseAddress = new Uri(apiUrl);

            var jobServiceAPI = RestService.For<IJobServiceApi>(httpClient);            

            return await jobServiceAPI.Execute(jobName, methodName, size, numberOfBatch);
        }
    }
}
