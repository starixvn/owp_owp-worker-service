using OWP.SumReportData.LAReport.Services;

namespace OWP.SumReportData.LAReport;

public interface Handler
{
    Task Handle(DateTime eventDate, bool isGenerateData = false);
}

public class HandlerImp : Handler
{
    private readonly LARepository repository;
    private readonly CalculationService calculationService;

    public HandlerImp(LARepository repository, CalculationService calculationService)
    {
        this.repository = repository;
        this.calculationService = calculationService;
    }

    public async Task Handle(DateTime eventDate, bool isGenerateData = false)
    {
        var laReportModels = await calculationService.Calculate(eventDate, isGenerateData);
        await repository.Update(eventDate, laReportModels);

        var otherProductMatchIds = laReportModels.SelectMany(x => x.OtherProductMatchIds).Distinct();
        await repository.SaveOtherProductMatchIds(eventDate, otherProductMatchIds);
    }
}