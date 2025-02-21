namespace OWP.SumReport.MatchLiveReport.Models;

using Constants;

public class OwpMatch
{
    public int OwpMatchId { get; set; }

    public int? MinorBettypeId { get; set; }

    public int? ParentMatchId { get; set; }

    public string OwpLeagueName { get; set; }

    public string LeagueCategory { get; set; }

    public bool IsSaba { get; set; }

    public BetTypeName BetTypeName { get; set; }
}