namespace Fanex.NAP.SPU.CTS.Features.SchedulerManager
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateFlagJobUpdateCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_IsUpdate")]
        public bool IsUpdate { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_AD_CTSJob_FlagUpdate_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
