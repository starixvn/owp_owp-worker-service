using OWP.Core.Attributes;

namespace OWP.PrematchReport.Models
{
  [Serializable]
  [XmlName("Row")]
  public record DataReport : DataReportBase
  {
    public int GroupId { get; init; }

    public int Amount { get; init; }

    public int BetType { get; init; }
  }
}