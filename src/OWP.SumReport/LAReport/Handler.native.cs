namespace OWP.SumReport.LAReport;

using Services;

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
    }
}