namespace OWP.InfositeIdSync.Models;

using Olympus.InfoSiteClient.Extensions.XMLExtensions;

[Serializable]
[XMLName("Row")]
public class MatchMappingWithInfosite
{
    [XMLName("OWPMatchId")]
    public int OwpMatchId { get; set; }

    public int InfositeMatchId { get; set; }

    [XMLName("AdminsiteMatchId")]
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