namespace OWP.InfositeIdSync.Models;

public class OWPMatch
{
    public string League { get; set; }

    public string Home { get; set; }

    public string Away { get; set; }

    public DateTime EventDateTime { get; set; }

    public int OWPMatchId { get; set; }
}