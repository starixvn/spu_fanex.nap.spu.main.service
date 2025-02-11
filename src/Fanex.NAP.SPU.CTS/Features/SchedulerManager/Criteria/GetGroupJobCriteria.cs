namespace Fanex.NAP.SPU.CTS.Features.SchedulerManager
{
    using Fanex.Data.Repository; 

    public class GetGroupJobCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "CTS_AD_CTSGroupJob_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
