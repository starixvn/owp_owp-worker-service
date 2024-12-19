namespace OWP.InfositeIdSync.Models;

using Olympus.Xml;

[Serializable]
[XmlName("MatchMappingWithInfosite")]
public class MatchMappingWithInfosite(int owpMatchId, int infositeMatchId, int adminSiteMatchId, DateTime eventDateTime)
{
    [XmlName("OWPMatchId")]
    public int OwpMatchId { get; set; } = owpMatchId;

    public int InfositeMatchId { get; set; } = infositeMatchId;

    [XmlName("AdminsiteMatchId")]
    public int AdminSiteMatchId { get; set; } = adminSiteMatchId;

    public DateTime EventDateTime { get; set; } = eventDateTime;
}