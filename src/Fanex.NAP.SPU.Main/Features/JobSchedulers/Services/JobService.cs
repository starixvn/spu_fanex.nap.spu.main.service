namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.Services
{
    using Refit;
    using RestSharp;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public interface IJobService
    {
        bool Execute(string jobName, string apiUrl, string methodName, string controller, string parameter);

        Task<bool> Execute(string jobName, string apiUrl, string methodName, string controller, Dictionary<string, object> data);
    }

    public class JobService : IJobService
    {        
        public bool Execute(string jobName, string apiUrl, string methodName, string controller, string parameter)
        {
            var url = new StringBuilder();
            url.Append(apiUrl);
            url.Append("/");
            url.Append(controller);
            url.Append("/");
            url.Append(methodName);
            url.Append(parameter);                       

            var client = new RestClient(url.ToString());

            client.Timeout = -1;

            var request = new RestRequest(Method.POST);

            IRestResponse response = client.Execute(request);

            if (string.IsNullOrWhiteSpace(response.Content)) return false;

            return Convert.ToBoolean(response.Content);
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
    }
}
