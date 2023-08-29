using OWP.SumReport.LAReport.Models;

namespace OWP.SumReport.LAReport.Services;

public interface SpecialAndMixedAndESoccerService
{
    public void SumUp(IList<Assignment> assignments, LaReportModel report);
}

public class SpecialAndMixedAndESoccerServiceImp : SpecialAndMixedAndESoccerService
{
    private readonly SpecialProductService specialService;

    private readonly MixedProductService mixService;

    private readonly EsoccerProductService esoccerService;

    private readonly FiveMlProductService fiveMlService;

    public SpecialAndMixedAndESoccerServiceImp(
        SpecialProductService specialService, MixedProductService mixService, EsoccerProductService esoccerService, FiveMlProductService fiveMlService)
    {
        this.specialService = specialService;
        this.mixService = mixService;
        this.esoccerService = esoccerService;
        this.fiveMlService = fiveMlService;
    }

    public void SumUp(IList<Assignment> assignments, LaReportModel report)
    {
        specialService.SumUp(assignments, report);
        mixService.SumUp(assignments, report);
        esoccerService.SumUp(assignments, report);
        fiveMlService.SumUp(assignments, report);
    }
}