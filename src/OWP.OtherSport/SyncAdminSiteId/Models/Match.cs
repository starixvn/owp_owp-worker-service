namespace OWP.OtherSport.SyncAdminSiteId.Models;

public class Match
{
    public int OsMatchId { get; set; }

    public int? AdminSiteMatchId { get; set; }

    public string LeagueName { get; set; }

    public string HomeName { get; set; }

    public bool IsMainMatch { get; set; }

    public string AwayName { get; set; }

    public string Sport { get; set; }

    public DateTime EventDate { get; set; }

    public DateTime EventDateTime { get; set; }

    public bool OfferLive { get; set; }

    public bool IsLinked { get; set; }

    public bool IsSaba => LeagueName.Contains("SABA ", StringComparison.OrdinalIgnoreCase);
}