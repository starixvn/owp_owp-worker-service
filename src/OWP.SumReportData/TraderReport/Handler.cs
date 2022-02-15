using OWP.SumReportData.TraderReport.Services;

namespace OWP.SumReportData.TraderReport;

public interface Handler
{
    Task Handle(DateTime eventDate);
}

public class HandlerImp : Handler
{
    private readonly TraderRepository repository;
    private readonly RobotService robotService;
    private readonly ESoccerService eSoccerService;

    public HandlerImp(TraderRepository repository, RobotService robotService, ESoccerService eSoccerService)
    {
        this.repository = repository;
        this.robotService = robotService;
        this.eSoccerService = eSoccerService;
    }

    public async Task Handle(DateTime eventDate)
    {
        Console.WriteLine("{0} {1}","Sum TraderReport at: ", eventDate);
        var robotByByUsers = await robotService.SumRobotFields(eventDate);
        var eSoccerByUsers = await eSoccerService.SumESoccerFields(eventDate);

        await repository.UpdateNoneRobotFields(eventDate);
        await repository.UpdateRobotFields(eventDate, robotByByUsers);
        await repository.UpdateESoccerFields(eventDate, eSoccerByUsers);
    }
}