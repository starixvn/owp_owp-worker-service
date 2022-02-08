using Fanex.Data.Repository;

namespace OWP.SyncLeagueSettings.Criteria;

public class InsertNewLeagueSettingsCriteria : CriteriaBase
{
    public string LeagueSettings { get; set; }

    public override string GetSettingKey() => "OWP_LeagueManagement_LeagueSettingInsert";

    public override bool IsValid() => !string.IsNullOrEmpty(LeagueSettings);
}