using OWP.SumReport.LAReport.Models;

namespace OWP.SumReport.LAReport.Services;

public interface SpecialAndMixedAndESoccerService
{
    public void SumUp(IList<Assignment> assignments, LaReportModel report);
}

public class SpecialAndMixedAndESoccerServiceImp(
    SpecialProductService specialService,
    MixedProductService mixService,
    EsoccerProductService esoccerService,
    FiveMlProductService fiveMlService)
    : SpecialAndMixedAndESoccerService
{
    public void SumUp(IList<Assignment> assignments, LaReportModel report)
    {
        specialService.SumUp(assignments, report);
        mixService.SumUp(assignments, report);
        esoccerService.SumUp(assignments, report);
        fiveMlService.SumUp(assignments, report);
    }
}