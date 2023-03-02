namespace OWP.SyncLeagueSettings.Models;

public class LeagueSettingCondition
{
    public int LeagueSettingId { get; set; }

    public bool HasTv { get; set; }

    public string Tv { get; set; }

    public IEnumerable<Condition> Rounds { get; set; } = new List<Condition>();

    public IEnumerable<Condition> Divisions { get; set; } = new List<Condition>();
}