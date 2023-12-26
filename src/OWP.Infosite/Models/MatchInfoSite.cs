namespace OWP.Infosite.Models;

public class MatchInfoSite
{
    public int InfoSiteMatchId { get; set; }

    public DateTime EventDateTime { get; set; }

    public string LeagueName { get; set; }

    public string HomeName { get; set; }

    public string AwayName { get; set; }

    public bool IsCheck { get; set; }
}