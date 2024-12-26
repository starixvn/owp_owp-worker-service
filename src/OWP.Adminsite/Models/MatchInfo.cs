namespace OWP.Adminsite.Models;

public class MatchInfo
{
    public int OWPMatchId { get; set; }
    public int? ParentMatchId { get; set; }
    public int? MinorBettypeId { get; set; }
    public int AdminSiteMatchId { get; set; }
    public string League { get; set; }
    public string Provider { get; set; }
    public string OtherProvider { get; set; }
    public string WatchBet { get; set; }
    public int NoOfHDPId { get; set; }
    public int? LeagueGroupId { get; set; }
}