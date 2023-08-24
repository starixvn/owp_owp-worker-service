using OWP.SumReport.LAReport.Enum;
using OWP.SumReport.LAReport.Models;

namespace OWP.SumReport.LAReport.Services;

public interface EsoccerProductService
{
    void SumUp(IList<Assignment> assignments,
        LaReportModel sumUpResult);
}

public class EsoccerProductServiceImp : EsoccerProductService
{
    public void SumUp(IList<Assignment> assignments, LaReportModel sumUpResult)
    {
        sumUpResult.ESoccer5Mins = Utils.CountSingleMatch(assignments, ReportType.Esoccer5Mins);
        sumUpResult.ESoccer6Mins = Utils.CountSingleMatch(assignments, ReportType.Esoccer6Mins);
        sumUpResult.ESoccer15Mins = Utils.CountSingleMatch(assignments, ReportType.Esoccer15Mins);
        sumUpResult.ESoccer16Mins = Utils.CountSingleMatch(assignments, ReportType.Esoccer16Mins);
        sumUpResult.ESoccer20Mins = Utils.CountSingleMatch(assignments, ReportType.Esoccer20Mins);
    }
}