using OWP.Core.Attributes;

namespace OWP.PrematchReport.Models
{
  [Serializable]
  [XmlName("Row")]
  public record TotalDataReport : DataReportBase
  {
    public int TotalLeagueHDP { get; init; }

    public int TotalLeagueOU { get; init; }

    public int TotalMatchHDP { get; init; }

    public int TotalMatchOU { get; init; }
  }
}