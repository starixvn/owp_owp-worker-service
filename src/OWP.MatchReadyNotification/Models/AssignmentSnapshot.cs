namespace OWP.MatchReadyNotification.Models;

public class AssignmentSnapshot
{
    public Dictionary<int, MatchInfo> Matches { get; set; } = new();

    public Dictionary<int, List<MatchAssignment>> Assignments { get; set; } = new();

    public string LastDateTimeNotification { get; set; } = string.Empty;
}