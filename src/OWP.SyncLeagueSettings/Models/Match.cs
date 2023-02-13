namespace OWP.SyncLeagueSettings.Models;

public class Match
{
    public int OwpLeagueId { get; set; }

    public int OwpMatchId { get; set; }

    public DateTime EventDateTime { get; set; }

    public string Tv { get; set; }

    public IList<MatchSetting> MatchSettings { get; set; }
}