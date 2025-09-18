using Fanex.Data.Repository;

namespace OWP.PushInfoToSettlement.Criteria
{
    public class GetEventCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "OWP_API_PushMatchRemarkForSettlement";

        public override bool IsValid() => true;
    }
}
