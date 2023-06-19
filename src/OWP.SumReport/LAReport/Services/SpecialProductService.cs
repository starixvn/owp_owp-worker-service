using OWP.SumReport.LAReport.Enum;
using OWP.SumReport.LAReport.Models;

namespace OWP.SumReport.LAReport.Services
{
    public interface MixedProductService
    {
        void SumUp(IList<Assignment> assignments,
            LaReportModel sumUpResult);
    }

    public class MixedProductServiceImp: MixedProductService
    {
        public void SumUp(IList<Assignment> assignments, LaReportModel sumUpResult)
        {
            sumUpResult.BookingPlus = Utils.CountGroupSpecialProduct(assignments, ReportType.TotalBookingPlus);

            sumUpResult.CornerPlus = Utils.CountGroupSpecialProduct(assignments, ReportType.CornerPlus);

            sumUpResult.Fantasy = Utils.CountSingleMatch(assignments,ReportType.Fantasy);

            sumUpResult.GoalKick = Utils.CountGroupSpecialProduct(assignments, ReportType.GoalKick);

            sumUpResult.OtherProducts = Utils.CountSingleMatch(assignments, ReportType.OtherSpecialProducts);
        }
    }
}
