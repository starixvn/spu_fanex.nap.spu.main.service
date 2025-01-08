namespace Fanex.NAP.SPU.CTS.Features.SchedulerManager
{
    using Fanex.Data;
    using Fanex.Data.Repository; 

    public class ClearedJobNewCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_JobName")]
        public string JobName { get; set; }

        [SpParam(SqlParamName = "ip_GroupName")]
        public string GroupName { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_AD_CTSJob_NewJobCleared";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
