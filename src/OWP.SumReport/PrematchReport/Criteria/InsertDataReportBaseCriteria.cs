namespace OWP.SumReport.PrematchReport.Criteria;

using Fanex.Data.Repository;

public abstract class InsertDataReportBaseCriteria : NonQueryCommand
{
    public string XML { get; set; }
}