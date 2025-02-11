namespace Fanex.NAP.SPU.CTS.Features.SchedulerManager
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetJobsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_IsAdjust")]
        public bool IsGetAdjust { get; set; }

        [SpParam(SqlParamName = "ip_EnvironmentID")]
        public int DbEnvironmentID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_AD_CTSJob_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
