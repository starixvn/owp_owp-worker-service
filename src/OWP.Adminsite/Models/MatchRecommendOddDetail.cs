namespace OWP.Adminsite.Models;

public class MatchRecommendOddDetail
{
    public int OWPMatchId { get; set; }
    public int BettypeId { get; set; }
    public byte Choice { get; set; }
    public decimal? Odds { get; set; }
}
