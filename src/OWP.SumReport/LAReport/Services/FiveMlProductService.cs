using OWP.SumReport.LAReport.Enum;
using OWP.SumReport.LAReport.Models;

namespace OWP.SumReport.LAReport.Services;

public interface FiveMlProductService
{
    void SumUp(IList<Assignment> assignments,
        LaReportModel sumUpResult);
}

public class FiveMlProductServiceImp : FiveMlProductService
{
    public void SumUp(IList<Assignment> assignments, LaReportModel sumUpResult)
    {
        sumUpResult.FiveMajorLeague = Utils.CountSingleMatch(assignments, ReportType.MajorLeague);
    }
}