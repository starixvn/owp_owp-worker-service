namespace OWP.OtherSport.SyncAdminSiteId.Models;

public class Match : IEquatable<Match>
{
    public int OSMatchId { get; set; }

    public int? AdminSiteMatchId { get; set; }

    public string LeagueName { get; set; }

    public string HomeName { get; set; }

    public string AwayName { get; set; }

    public string Sport { get; set; }

    public DateTime EventDateTime { get; set; }

    public bool OfferLive { get; set; }

    public bool IsSaba => LeagueName.Contains("SABA ", StringComparison.OrdinalIgnoreCase);

    public bool Equals(Match other)
        => IsSameMatch(other) && (!IsSaba || EventDateTime == other.EventDateTime);

    public bool IsSameMatch(Match other)
        => EventDateTime.Date == other.EventDateTime.Date &&
           Sport.Trim().Equals(other.Sport.Trim(), StringComparison.OrdinalIgnoreCase) &&
           IsSameEvent(other);

    private bool IsSameEvent(Match other)
        => LeagueName.Trim().Equals(other.LeagueName.Trim(), StringComparison.OrdinalIgnoreCase) &&
           HomeName.Trim().Equals(other.HomeName.Trim(), StringComparison.OrdinalIgnoreCase) &&
           AwayName.Trim().Equals(other.AwayName.Trim(), StringComparison.OrdinalIgnoreCase);
}