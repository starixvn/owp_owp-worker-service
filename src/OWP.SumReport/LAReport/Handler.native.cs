namespace OWP.SumReport.LAReport;

using Services;

public interface Handler
{
    Task Handle(DateTime eventDate, bool isGenerateData = false);
}

public class HandlerImp : Handler
{
    private readonly LaRepository repository;
    private readonly CalculationService calculationService;

    public HandlerImp(LaRepository repository, CalculationService calculationService)
    {
        this.repository = repository;
        this.calculationService = calculationService;
    }

    public async Task Handle(DateTime eventDate, bool isGenerateData = false)
    {
        eventDate = DateTime.Parse("2023-06-19");
        var laReportModels = await calculationService.Calculate(eventDate, isGenerateData);

        await repository.Update(eventDate, laReportModels);
    }
}