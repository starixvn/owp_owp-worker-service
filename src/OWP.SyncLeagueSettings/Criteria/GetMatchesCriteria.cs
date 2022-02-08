using Fanex.Data.Repository;

namespace OWP.SyncLeagueSettings.Criteria;

public class GetMatchesCriteria : CriteriaBase
{
    public DateTime EventDate { get; set; }

    public override string GetSettingKey() => "OWP_LeagueManagement_GetNoSettingsMatches";

    public override bool IsValid() => true;
}