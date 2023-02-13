namespace OWP.SumReport.PrematchReport.Models;

using Attributes;

[Serializable]
[XmlName("Row")]
public record DataReport : DataReportBase
{
    public int GroupId { get; init; }

    public int Amount { get; init; }

    public int BetType { get; init; }
}