using Fanex.Data.Repository;

namespace OWP.SumReport.PrematchReport.Criteria
{
    public abstract class InsertDataReportCriteriaBase : NonQueryCommand
    {
        public string XML { get; set; }
    }
}