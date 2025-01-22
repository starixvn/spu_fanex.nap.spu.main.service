namespace Fanex.NAP.SPU.CTS.Features.SchedulerManager.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CheckExistJobCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_JobName")]
        public string JobName { get; set; }

        [SpParam(SqlParamName = "ip_GroupName")]
        public string GroupName { get; set; }

        [SpParam(SqlParamName = "ip_EnvironmentIDs")]
        public string EnvironmentIDs { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_AD_CTSJob_CheckExist";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
