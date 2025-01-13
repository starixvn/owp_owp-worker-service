namespace OWP.Adminsite.Models;

[Serializable]
public class MatchInfoApiModel : IEqualityComparer<MatchInfoApiModel>
{
    public int MatchId { get; set; }
    public int? SubType { get; set; }
    public string Provider { get; set; }
    public string OtherProvider { get; set; }
    public string WatchBet { get; set; }
    public int? SecondAcceptValue { get; set; }
    public int? CreditFtLines { get; set; }
    public int? CreditFirstHalfLines { get; set; }
    public int? LicenseeFtLines { get; set; }
    public int? LicenseeFirstHalfLines { get; set; }
    public int? SuspenderUserId { get; set; }
    public string SuspenderUsername { get; set; }
    public IList<AssignmentApiModel> Traders { get; set; } = [];
    public IList<MatchSettingApiModel> MatchSettings { get; set; } = [];

    public bool Equals(MatchInfoApiModel x, MatchInfoApiModel y)
    {
        return x != null && y != null
            && x.MatchId == y.MatchId
            && x.SubType == y.SubType
            && string.Equals(x.Provider, y.Provider, StringComparison.OrdinalIgnoreCase)
            && string.Equals(x.OtherProvider, y.OtherProvider, StringComparison.OrdinalIgnoreCase)
            && string.Equals(x.WatchBet, y.WatchBet, StringComparison.OrdinalIgnoreCase)
            && x.SecondAcceptValue == y.SecondAcceptValue
            && EqualNumberOfLines(x, y)
            && EqualSuspender(x, y)
            && EqualTraders(x, y)
            && EqualMatchSettings(x, y);
    }

    public int GetHashCode(MatchInfoApiModel obj)
        => obj.SubType.HasValue ? obj.MatchId * obj.SubType.Value : obj.MatchId;

    private static bool EqualNumberOfLines(MatchInfoApiModel x, MatchInfoApiModel y)
        => x.CreditFtLines == y.CreditFtLines
            && x.CreditFirstHalfLines == y.CreditFirstHalfLines
            && x.LicenseeFtLines == y.LicenseeFtLines
            && x.LicenseeFirstHalfLines == y.LicenseeFirstHalfLines;

    private static bool EqualSuspender(MatchInfoApiModel x, MatchInfoApiModel y)
        => x.SuspenderUserId == y.SuspenderUserId
            && string.Equals(x.SuspenderUsername, y.SuspenderUsername, StringComparison.OrdinalIgnoreCase);

    private static bool EqualTraders(MatchInfoApiModel x, MatchInfoApiModel y)
        => x.Traders != null && y.Traders != null
            && x.Traders.Count == y.Traders.Count
            && !x.Traders.Except(y.Traders, new AssignmentApiModel()).Any();

    private static bool EqualMatchSettings(MatchInfoApiModel x, MatchInfoApiModel y)
        => x.MatchSettings != null && y.MatchSettings != null
            && x.MatchSettings.Count == y.MatchSettings.Count
            && !x.MatchSettings.Except(y.MatchSettings, new MatchSettingApiModel()).Any();
}