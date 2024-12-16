namespace Fanex.NAP.SPU.CTS.Features.SchedulerManager.Model
{
    public class JobInfoModel
    {
        public JobInfoModel()
        {
            Jobs = Enumerable.Empty<JobEntity>();
            Triggers = Enumerable.Empty<TriggerEntity>();
        }

        public IEnumerable<JobEntity> Jobs { get; set; }

        public IEnumerable<TriggerEntity> Triggers { get; set; }

    }
}
