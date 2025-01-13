namespace OWP.Adminsite.Models;

[Serializable]
public class AssignmentApiModel : IEqualityComparer<AssignmentApiModel>
{
    public int Bettype { get; set; }
    public int TraderId { get; set; }
    public string TraderName { get; set; }

    public bool Equals(AssignmentApiModel x, AssignmentApiModel y)
        => x.Bettype == y.Bettype && x.TraderId == y.TraderId && string.Equals(x.TraderName, y.TraderName, StringComparison.OrdinalIgnoreCase);

    public int GetHashCode(AssignmentApiModel obj) => obj.Bettype * obj.TraderId;
}