namespace OWP.InfositeIdSync.Models;

public class SyncIsLiveModel : IEqualityComparer<SyncIsLiveModel>
{
    public int InfoSiteMatchId { get; set; }

    public int AdminSiteMatchId { get; set; }

    public bool IsLive { get; set; }

    public int? MinorBetTypeId { get; set; }

    public bool Equals(SyncIsLiveModel x, SyncIsLiveModel y)
        => x != null && y != null && x.InfoSiteMatchId == y.InfoSiteMatchId && x.MinorBetTypeId == y.MinorBetTypeId;

    public int GetHashCode(SyncIsLiveModel obj)
        => MinorBetTypeId.HasValue ? InfoSiteMatchId * MinorBetTypeId.Value : InfoSiteMatchId;
}

public class IsLiveComparer : IEqualityComparer<SyncIsLiveModel>
{
    public bool Equals(SyncIsLiveModel x, SyncIsLiveModel y)
        => x != null && y != null && x.InfoSiteMatchId == y.InfoSiteMatchId && x.MinorBetTypeId == y.MinorBetTypeId && x.IsLive == y.IsLive;

    public int GetHashCode(SyncIsLiveModel obj)
        => obj.MinorBetTypeId.HasValue ? obj.InfoSiteMatchId * obj.MinorBetTypeId.Value : obj.InfoSiteMatchId;
}