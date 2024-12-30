namespace OWP.Adminsite.Models;

[Serializable]
public class AssignmentApiModel : IEqualityComparer<AssignmentApiModel>
{
    public int BettypeId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; }

    public bool Equals(AssignmentApiModel x, AssignmentApiModel y)
        => x.BettypeId == y.BettypeId && x.UserId == y.UserId && string.Equals(x.Username, y.Username, StringComparison.OrdinalIgnoreCase);

    public int GetHashCode(AssignmentApiModel obj) => obj.BettypeId * obj.UserId;
}