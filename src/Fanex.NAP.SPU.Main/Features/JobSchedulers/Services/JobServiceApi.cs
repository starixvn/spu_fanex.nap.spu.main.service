namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.Services
{
    using Refit;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IJobServiceApi
    {
        [Post("/job/{methodName}")]
        Task<bool> Execute(string jobName, string methodName, int size = 0, int numberOfBatch = 0);

        [Post("/monitoring/{methodName}")]
        Task<bool> Detect(string jobName, string methodName);

        [Post("/{controller}/{methodName}")]
        Task<bool> Execute(string jobName, string methodName, string controller, [Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data);
    }    
}
