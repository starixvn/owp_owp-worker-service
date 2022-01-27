namespace OWP.OtherSport.SyncAdminSiteId.Models;

public class LinkedMatch
{
    public int OSMatchId { get; set; }

    public int? AdminSiteMatchId { get; set; }

    public string OWPLeague { get; set; }

    public string OWPHome { get; set; }

    public string OWPAway { get; set; }

    public int? OldAdminSiteMatchId { get; set; }

    public string AdminSiteLeague { get; set; }

    public string AdminSiteHome { get; set; }

    public string AdminSiteAway { get; set; }

    public DateTime OldEventDate { get; set; }

    public DateTime NewEventDate { get; set; }
}