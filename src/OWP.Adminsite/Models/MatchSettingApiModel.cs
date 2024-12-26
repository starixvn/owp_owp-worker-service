namespace OWP.Adminsite.Models;

[Serializable]
public class MatchSettingApiModel
{
    public int BettypeId { get; set; }
    public decimal? Maxbet { get; set; }
    public decimal? Spread { get; set; }
    public decimal? Hdp1 { get; set; }
    public decimal? Hdp2 { get; set; }
    public decimal? Odds1 { get; set; }
    public decimal? Odds2 { get; set; }
}
