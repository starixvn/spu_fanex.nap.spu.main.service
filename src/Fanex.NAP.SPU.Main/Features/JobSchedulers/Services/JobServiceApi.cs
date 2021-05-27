namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.Services
{
    using Refit;
    using System.Threading.Tasks;

    public interface IJobServiceApi
    {
        [Post("/job/{methodName}")]
        Task<bool> Execute(string jobName, string methodName, int size = 0, int numberOfBatch = 0);

        [Post("/monitoring/{methodName}")]
        Task<bool> Detect(string jobName, string methodName);
    }    
}
