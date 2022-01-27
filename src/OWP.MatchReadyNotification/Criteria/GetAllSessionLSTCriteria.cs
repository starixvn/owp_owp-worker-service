using Fanex.Data.Repository;

namespace OWP.MatchReadyNotification.Criteria;

public class GetAllSessionLSTCriteria : CriteriaBase
{
    public override string GetSettingKey() => "OWP_GetAllSessionLST";

    public override bool IsValid() => true;
}