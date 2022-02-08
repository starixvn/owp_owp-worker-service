using Fanex.Data.Repository;

namespace OWP.SyncLeagueSettings.Criteria;

public class UpdateConditionCriteria : NonQueryCommand
{
    public int ActorId { get; set; }

    public int LeagueSettingId { get; set; }

    public bool HasTV { get; set; }

    public string TV { get; set; }

    public string Round { get; set; }

    public string Division { get; set; }

    public string FullConditionDescription { get; set; }

    public override string GetSettingKey() => "OWP_LeagueManagement_UpdateCondition";

    public override bool IsValid() => LeagueSettingId > 0;
}