namespace OWP.Adminsite.Models;

public class MatchSetting
{
    public int OWPMatchId { get; set; }
    public int BettypeId { get; set; }
    public decimal? Maxbet { get; set; }
    public decimal? Spread { get; set; }
}
