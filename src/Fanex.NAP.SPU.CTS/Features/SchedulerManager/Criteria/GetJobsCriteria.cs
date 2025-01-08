namespace Fanex.NAP.SPU.CTS.Features.SchedulerManager
{
    using Fanex.Data.Repository;

    public class GetJobsCriteria : CriteriaBase
    {      
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
