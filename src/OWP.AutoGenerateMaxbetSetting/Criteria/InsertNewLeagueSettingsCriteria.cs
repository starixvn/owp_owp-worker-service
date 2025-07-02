namespace OWP.AutoGenerateMaxbetSetting.Criteria;

using Fanex.Data.Repository;

public class InsertNewLeagueSettingsCriteria : NonQueryCommand
{
    public string LeagueSettings { get; set; }

    public override string GetSettingKey() => "OWP_NewLeagueSetting_LeagueSettingInsert";

    public override bool IsValid() => !string.IsNullOrEmpty(LeagueSettings);
}