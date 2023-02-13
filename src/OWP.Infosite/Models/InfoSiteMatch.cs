namespace OWP.Infosite.Models;

public class InfoSiteMatch
{
    public int InfoSiteMatchId { get; set; }

    public int OwpLeagueId { get; set; }

    public DateTime EventDateTime { get; set; }

    public string LeagueName { get; set; }

    public string HomeName { get; set; }

    public string AwayName { get; set; }
}