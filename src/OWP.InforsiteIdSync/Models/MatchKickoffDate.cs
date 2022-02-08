namespace OWP.InfositeIdSync.Models;

public class MatchKickoffDate : IEqualityComparer<MatchKickoffDate>
{
    public int MatchId { get; set; }

    public DateTime KickoffDate { get; set; }

    public int? MinorBettypeId { get; set; }

    public bool Equals(MatchKickoffDate x, MatchKickoffDate y)
        => x.MatchId == y.MatchId && x.MinorBettypeId == y.MinorBettypeId;

    public int GetHashCode(MatchKickoffDate obj)
        => MinorBettypeId.HasValue ? MatchId * MinorBettypeId.Value : MatchId;
}