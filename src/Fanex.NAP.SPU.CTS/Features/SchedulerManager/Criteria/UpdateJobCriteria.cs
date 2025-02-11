namespace Fanex.NAP.SPU.CTS.Features.SchedulerManager
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateJobCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_JobTrigger")]
        public string JobTriggerJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_AD_CTSJob_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
