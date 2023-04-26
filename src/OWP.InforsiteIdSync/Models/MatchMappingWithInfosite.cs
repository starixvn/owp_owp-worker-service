namespace OWP.InfositeIdSync.Models;

using Olympus.Xml;

[Serializable]
[XmlName("MatchMappingWithInfosite")]
public class MatchMappingWithInfosite
{
    [XmlName("OWPMatchId")]
    public int OwpMatchId { get; set; }

    public int InfositeMatchId { get; set; }

    [XmlName("AdminsiteMatchId")]
    public int AdminSiteMatchId { get; set; }

    public DateTime EventDateTime { get; set; }

    public MatchMappingWithInfosite(int owpMatchId, int infositeMatchId, int adminSiteMatchId, DateTime eventDateTime)
    {
        OwpMatchId = owpMatchId;
        InfositeMatchId = infositeMatchId;
        AdminSiteMatchId = adminSiteMatchId;
        EventDateTime = eventDateTime;
    }
}