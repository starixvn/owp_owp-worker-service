namespace OWP.MatchReadyNotification.Models;

public class MatchInfo
{
    public int MatchId { get; set; }

    public int ParentMatchId { get; set; }

    public string LeagueName { get; set; }

    public string Home { get; set; }

    public string Away { get; set; }

    public string MatchNo { get; set; }

    public TimeSpan KoTime { get; set; }
}