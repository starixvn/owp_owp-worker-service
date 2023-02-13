namespace OWP.Infosite.Models;

public class PreMatchLeague
{
    public int PreMatchLeagueId { get; set; }

    public int OwpLeagueId { get; set; }

    public DateTime EventDate { get; set; }

    public bool IsDay { get; set; }

    public string OwpLeagueName { get; set; }
}