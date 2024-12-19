namespace OWP.SumReport.TraderReport;

using Services;

public interface Handler
{
    Task Handle(DateTime eventDate, bool isGenerateData = false);
}

public class HandlerImp(TraderRepository repository, RobotService robotService, ESoccerService eSoccerService)
    : Handler
{
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