namespace OWP.OtherSport.SyncAdminSiteId.Models;

public class LinkedMatch
{
    public int OsMatchId { get; set; }

    public int? AdminSiteMatchId { get; set; }

    public string OwpLeague { get; set; }

    public string OwpHome { get; set; }

    public string OwpAway { get; set; }

    public int? OldAdminSiteMatchId { get; set; }

    public string AdminSiteLeague { get; set; }

    public string AdminSiteHome { get; set; }

    public string AdminSiteAway { get; set; }

    public DateTime OldEventDate { get; set; }

    public DateTime NewEventDate { get; set; }
}