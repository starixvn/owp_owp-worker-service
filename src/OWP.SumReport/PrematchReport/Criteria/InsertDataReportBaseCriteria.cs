namespace OWP.SumReport.PrematchReport.Criteria;

using Fanex.Data.Repository;

public abstract class InsertDataReportBaseCriteria : NonQueryCommand
{
    public string Xml { get; set; }
}