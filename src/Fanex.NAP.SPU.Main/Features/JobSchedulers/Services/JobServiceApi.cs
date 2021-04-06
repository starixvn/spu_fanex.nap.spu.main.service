namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.Services
{
    using Fanex.NAP.SPU.Main.Features.JobSchedulers.Models;
    using Refit;
    using System;
    using System.Threading.Tasks;

    public interface IJobServiceApi
    {
        [Post("/job/{methodName}")]
        Task<bool> Execute(string jobName, string methodName, int size = 0, int numberOfBatch = 0);
    }    
}
