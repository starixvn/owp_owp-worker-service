namespace OWP.MatchReadyNotification.Models;

public class AssignmentSnapshot
{
    public Dictionary<int, MatchInfo> Matches { get; set; }

    public Dictionary<int, List<MatchAssignment>> Assignments { get; set; }

    public string LastDateTimeNotification { get; set; }

    public AssignmentSnapshot()
    {
        Matches = new Dictionary<int, MatchInfo>();
        Assignments = new Dictionary<int, List<MatchAssignment>>();
        LastDateTimeNotification = string.Empty;
    }
}