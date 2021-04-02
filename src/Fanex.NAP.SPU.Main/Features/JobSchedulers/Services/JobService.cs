namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.Services
{
    using Refit;
    using System.Threading.Tasks;

    public interface IJobService
    {
        Task<bool> Execute(string jobName, string apiUrl, int size = 0, int numberOfBatch = 0);
    }

    public class JobService : IJobService
    {
        public async Task<bool> Execute(string jobName, string apiUrl, int size = 0, int numberOfBatch = 0)
        {
            var jobServiceAPI = RestService.For<IJobServiceApi>(apiUrl);

            return await jobServiceAPI.Execute(jobName, size, numberOfBatch);
        }
    }
}
