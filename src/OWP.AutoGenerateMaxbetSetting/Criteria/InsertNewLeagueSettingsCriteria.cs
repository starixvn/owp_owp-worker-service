namespace OWP.AutoGenerateMaxbetSetting.Criteria;

using Fanex.Data.Repository;

public class InsertNewLeagueSettingsCriteria : CriteriaBase
{
    public string LeagueSettings { get; set; }

    public override string GetSettingKey() => "OWP_NewLeagueSetting_LeagueSettingInsert";

    public override bool IsValid() => !string.IsNullOrEmpty(LeagueSettings);
}