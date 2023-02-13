namespace OWP.InfositeIdSync.Models;

using Olympus.InfoSiteClient.Extensions.XMLExtensions;

[Serializable]
[XMLName("Row")]
public class MatchMappingWithInfosite
{
    public int OWPMatchId { get; set; }

    public int InfositeMatchId { get; set; }

    public int AdminsiteMatchId { get; set; }

    public DateTime EventDateTime { get; set; }

    public MatchMappingWithInfosite(int owpMatchId, int infositeMatchId, int adminsiteMatchId, DateTime eventDateTime)
    {
        OWPMatchId = owpMatchId;
        InfositeMatchId = infositeMatchId;
        AdminsiteMatchId = adminsiteMatchId;
        EventDateTime = eventDateTime;
    }
}