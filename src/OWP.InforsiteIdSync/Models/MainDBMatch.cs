namespace OWP.InfositeIdSync.Models;

public class MainDBMatch
{
    public int MatchId { get; set; } // AdminSiteMatchId

    public int? SubType { get; set; }

    public bool OfferLive { get; set; }

    public int? ParentId { get; set; }
}