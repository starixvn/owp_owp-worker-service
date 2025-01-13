namespace OWP.Adminsite.Models;

public class MatchRecommendOdd
{
    public int OWPMatchId { get; set; }
    public int BettypeId { get; set; }
    public byte TeamChoice { get; set; }
    public decimal? HDP { get; set; }
}
