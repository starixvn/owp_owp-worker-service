namespace OWP.MatchReadyNotification.Models;

public class NotReadyMatchInfo
{
    public string MatchNo { get; set; }

    public string Event { get; set; }

    public string LeagueName { get; set; }

    public TimeSpan KoTime { get; set; }

    public List<Assignee> Assignees { get; set; }
}