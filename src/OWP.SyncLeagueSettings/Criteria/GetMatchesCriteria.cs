namespace OWP.SyncLeagueSettings.Criteria;

using Fanex.Data.Repository;

public class GetMatchesCriteria : CriteriaBase
{
    public DateTime EventDate { get; set; }

    public override string GetSettingKey() => "OWP_LeagueManagement_GetNoSettingsMatches";

    public override bool IsValid() => true;
}