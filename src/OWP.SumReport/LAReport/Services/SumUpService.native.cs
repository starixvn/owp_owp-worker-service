using OWP.SumReport.LAReport.Models;

namespace OWP.SumReport.LAReport.Services;

public interface SumUpService
{
    void Execute(IList<Assignment> assignments, LaReportModel report);
}

public class SumUpServiceImp(
    SpecialAndMixedAndESoccerService specialAndMixedAndESoccerService,
    SingleProductService singleProductService,
    MultipleProductService multipleProductService)
    : SumUpService
{
    public void Execute(IList<Assignment> assignments, LaReportModel report)
    {
        specialAndMixedAndESoccerService.SumUp(assignments, report);
        multipleProductService.SumUp(assignments, report);
        singleProductService.SumUp(assignments, report);
    }
}