namespace OWP.MatchReadyNotification.Models;

public class Assignee : IEqualityComparer<Assignee>
{
    public int AssigneeId { get; set; }

    public string AssigneeName { get; set; }

    public bool Equals(Assignee x, Assignee y)
    {
        return x != null && y != null && x.AssigneeId == y.AssigneeId && x.AssigneeName == y.AssigneeName;
    }

    public int GetHashCode(Assignee obj)
    {
        return AssigneeId;
    }
}