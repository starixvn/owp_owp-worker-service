namespace OWP.SyncLeagueSettings.Models;

public class MatchSetting
{
    public int OwpMatchId { get; set; }

    public int Bettype { get; set; }

    public decimal? Spread { get; set; }

    public decimal? Maxbet { get; set; }
}