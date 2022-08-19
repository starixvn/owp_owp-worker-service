namespace OWP.SumReportData.PrematchReport.Models;

public class LeagueInfo
{
    public int OWPLeagueId { get; set; }

    public DateTime EventDate { get; set; }

    public int UserId { get; set; }

    public int GroupId { get; set; }
}

public class LeagueDetailInfo : LeagueInfo
{
    public int PreMatchLeagueId { get; set; }

    public bool IsDay { get; set; }
}