namespace OWP.SyncLeagueSettings.Criteria;

using Fanex.Data.Repository;

public class UpdateConditionCriteria : NonQueryCommand
{
    public int ActorId { get; set; }

    public int LeagueSettingId { get; set; }

    public bool HasTv { get; set; }

    public string Tv { get; set; }

    public string Round { get; set; }

    public string Division { get; set; }

    public string FullConditionDescription { get; set; }

    public override string GetSettingKey() => "OWP_LeagueManagement_UpdateCondition";

    public override bool IsValid() => LeagueSettingId > 0;
}