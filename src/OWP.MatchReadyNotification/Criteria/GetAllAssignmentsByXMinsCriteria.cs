using Fanex.Data.Repository;

namespace OWP.MatchReadyNotification.Criteria;

public class GetAllAssignmentsByXMinsCriteria : CriteriaBase
{
    public string LastTimeInCache { get; set; }

    public override string GetSettingKey() => "OWP_GetAllAssignmentsByXMins";

    public override bool IsValid() => true;
}