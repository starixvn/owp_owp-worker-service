namespace OWP.SumReport.PrematchReport.Models;

[Serializable]
public record DataReport : DataReportBase
{
    public int GroupId { get; init; }

    public int Amount { get; init; }

    public int BetType { get; init; }
}