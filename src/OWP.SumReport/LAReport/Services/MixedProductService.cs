using OWP.SumReport.LAReport.Enum;
using OWP.SumReport.LAReport.Models;

namespace OWP.SumReport.LAReport.Services
{
    public interface SpecialProductService
    {
        void SumUp(IList<Assignment> assignments,
            LaReportModel sumUpResult);
    }

    public class SpecialProductServiceImp: SpecialProductService
    {
        public void SumUp(IList<Assignment> assignments, LaReportModel sumUpResult)
        {
            sumUpResult.FtCorner = Utils.CountGroupMixedProduct(assignments, ReportType.FtCorner);
            sumUpResult.FtCorner5Ml = Utils.CountGroupMixedProduct(assignments, ReportType.FtCorner5Ml);
        }
    }
}
