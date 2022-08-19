using Fanex.Data.Repository;

namespace OWP.SumReportData.PrematchReport.Criteria
{
    public abstract class InsertDataReportCriteriaBase : NonQueryCommand
    {
        public string XML { get; set; }
    }
}