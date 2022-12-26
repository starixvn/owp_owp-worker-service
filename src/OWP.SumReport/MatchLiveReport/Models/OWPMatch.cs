using OWP.SumReport.MatchLiveReport.Constants;

namespace OWP.SumReport.MatchLiveReport.Models;

public class OWPMatch {
    public int OWPMatchId { get; set; }

    public int? MinorBettypeId { get; set; }

    public int? ParentMatchId { get; set; }

    public string OWPLeagueName { get; set; }

    public string LeagueCategory { get; set; }

    public bool IsSaba { get; set; }

    public BetTypeName BetTypeName { get; set; }
}