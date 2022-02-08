namespace OWP.InfositeIdSync.Models;

public class InfositeMatch
{
    public int AdminSiteMatchId { get; set; }

    public int InfoSiteMatchId { get; set; }

    public string LeagueName { get; set; }

    public string HomeName { get; set; }

    public string AwayName { get; set; }

    public DateTime EventDateTime { get; set; }
}