namespace OWP.Adminsite.Models;

[Serializable]
public class MatchSettingApiModel : IEqualityComparer<MatchSettingApiModel>
{
    public int Bettype { get; set; }
    public int? Maxbet { get; set; }
    public decimal? Spread { get; set; }
    public decimal? Hdp1 { get; set; }
    public decimal? Hdp2 { get; set; }
    public decimal? Odds1 { get; set; }
    public decimal? Odds2 { get; set; }

    public bool Equals(MatchSettingApiModel x, MatchSettingApiModel y)
        => x.Bettype == y.Bettype
            && x.Maxbet == y.Maxbet
            && x.Spread == y.Spread
            && x.Hdp1 == y.Hdp1
            && x.Hdp2 == y.Hdp2
            && x.Odds1 == y.Odds1
            && x.Odds2 == y.Odds2;

    public int GetHashCode(MatchSettingApiModel obj) => obj.Bettype;
}
