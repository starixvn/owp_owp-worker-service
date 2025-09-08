using Fanex.Data.Repository;

namespace OWP.PushInfoToSettlement.Criteria
{
    public class GetEventCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "OWP_GetAllEvent";

        public override bool IsValid() => true;
    }
}
