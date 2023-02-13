using Fanex.Data.Repository;

namespace OWP.SumReport.PrematchReport.Criteria;

public abstract class InsertDataReportBaseCriteria : NonQueryCommand
{
    public string XML { get; set; }
}