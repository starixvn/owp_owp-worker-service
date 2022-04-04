namespace OWP.SumReportData.MatchLiveReport.Models;

public class ReportModel {
    public DateTime EventDate { get; set; }

    public string LeagueCategory { get; set; }

    public string BetType { get; set; }

    public bool IsSaba { get; set; }

    public int MatchCount { get; set; }
}