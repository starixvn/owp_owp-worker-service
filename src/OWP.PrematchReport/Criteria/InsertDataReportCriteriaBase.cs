using Fanex.Data.Repository;

namespace OWP.PrematchReport.Criteria
{
  public abstract class InsertDataReportCriteriaBase : NonQueryCommand
  {
    public string? XML { get; set; }
  }
}