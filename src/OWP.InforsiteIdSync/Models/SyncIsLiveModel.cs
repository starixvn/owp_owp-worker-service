namespace OWP.InfositeIdSync.Models;

public class SyncIsLiveModel : IEqualityComparer<SyncIsLiveModel>
{
    public int InfoSiteMatchId { get; set; }

    public int AdminSiteMatchId { get; set; }

    public bool IsLive { get; set; }

    public int? MinorBettypeId { get; set; }

    public bool Equals(SyncIsLiveModel x, SyncIsLiveModel y)
        => x.InfoSiteMatchId == y.InfoSiteMatchId && x.MinorBettypeId == y.MinorBettypeId;

    public int GetHashCode(SyncIsLiveModel obj)
        => MinorBettypeId.HasValue ? InfoSiteMatchId * MinorBettypeId.Value : InfoSiteMatchId;
}

public class IsLiveComparer : IEqualityComparer<SyncIsLiveModel>
{
    public bool Equals(SyncIsLiveModel x, SyncIsLiveModel y)
        => x.InfoSiteMatchId == y.InfoSiteMatchId && x.MinorBettypeId == y.MinorBettypeId && x.IsLive == y.IsLive;

    public int GetHashCode(SyncIsLiveModel obj)
        => obj.MinorBettypeId.HasValue ? obj.InfoSiteMatchId * obj.MinorBettypeId.Value : obj.InfoSiteMatchId;
}