namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.Services
{
    using RestSharp;
    using System;
    using System.Text;

    public interface IJobService
    {
        bool Execute(string jobName, string apiUrl, string methodName, string controller, string parameter);
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
    }
}
