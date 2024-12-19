namespace OWP.SumReport.LAReport;

using Services;

public interface Handler
{
    Task Handle(DateTime eventDate, bool isGenerateData = false);
}

public class HandlerImp(LaRepository repository, CalculationService calculationService)
    : Handler
{
    public async Task Handle(DateTime eventDate, bool isGenerateData = false)
    {
        var laReportModels = await calculationService.Calculate(eventDate, isGenerateData);

        await repository.Update(eventDate, laReportModels);
    }
}