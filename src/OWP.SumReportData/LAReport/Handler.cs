using OWP.SumReportData.LAReport.Services;

namespace OWP.SumReportData.LAReport;

public interface Handler
{
    Task Handle(DateTime eventDate);
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

    public async Task Handle(DateTime eventDate)
    {
        var laReportModels = await calculationService.Calculate(eventDate);

        await repository.Update(eventDate, laReportModels);
    }
}