namespace OWP.SumReport.TraderReport;

using Services;

public interface Handler
{
    Task Handle(DateTime eventDate, bool isGenerateData = false);
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

    public async Task Handle(DateTime eventDate, bool isGenerateData = false)
    {
        var robotByByUsers = await robotService.SumRobotFields(eventDate, isGenerateData);
        var eSoccerByUsers = await eSoccerService.SumESoccerFields(eventDate, isGenerateData);

        await repository.UpdateNoneRobotFields(eventDate);
        await repository.UpdateRobotFields(eventDate, robotByByUsers);
        await repository.UpdateESoccerFields(eventDate, eSoccerByUsers);
        await repository.UpdateFTOtherFields(eventDate);
    }
}