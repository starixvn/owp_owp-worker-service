namespace OWP.OtherSport.SyncAdminSiteId.Models;

using Olympus.Xml;

public class LinkedMatch
{
    [XmlName("OSMatchId")]
    public int OsMatchId { get; set; }

    public int? AdminSiteMatchId { get; set; }

    [XmlName("OWPLeague")]
    public string OwpLeague { get; set; }

    [XmlName("OWPHome")]
    public string OwpHome { get; set; }

    [XmlName("OWPAway")]
    public string OwpAway { get; set; }

    public int? OldAdminSiteMatchId { get; set; }

    public string AdminSiteLeague { get; set; }

    public string AdminSiteHome { get; set; }

    public string AdminSiteAway { get; set; }

    public DateTime OldEventDate { get; set; }

    public DateTime NewEventDate { get; set; }
}