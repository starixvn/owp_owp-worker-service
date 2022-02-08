namespace OWP.SyncLeagueSettings.Models;

public class Match
{
    public int OWPLeagueId { get; set; }

    public int OWPMatchId { get; set; }

    public DateTime EventDateTime { get; set; }

    public string TV { get; set; }

    public IList<MatchSetting> MatchSettings { get; set; }
}