namespace OWP.MatchReadyNotification.Criteria;

using Fanex.Data.Repository;

public class GetAllAssignmentsByXMinsCriteria : CriteriaBase
{
    public string LastTimeInCache { get; set; }

    public override string GetSettingKey() => "OWP_GetAllAssignmentsByXMins";

    public override bool IsValid() => true;
}