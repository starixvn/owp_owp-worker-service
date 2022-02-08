namespace OWP.SyncLeagueSettings.Models;

public class LeagueSettingCondition
{
    public int LeagueSettingId { get; set; }

    public bool HasTV { get; set; }

    public string TV { get; set; }

    public IEnumerable<Condition> Rounds { get; set; } = new List<Condition>();

    public IEnumerable<Condition> Divisions { get; set; } = new List<Condition>();
}