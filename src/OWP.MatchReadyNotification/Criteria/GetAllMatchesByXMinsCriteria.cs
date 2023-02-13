namespace OWP.MatchReadyNotification.Criteria;

using Fanex.Data.Repository;

public class GetAllMatchesByXMinsCriteria : CriteriaBase
{
    public string MatchIds { get; set; }

    public override string GetSettingKey() => "OWP_GetAllMatchesByXMins";

    public override bool IsValid() => true;
}