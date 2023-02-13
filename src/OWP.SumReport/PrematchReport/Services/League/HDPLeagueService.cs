namespace OWP.SumReport.PrematchReport.Services.League;

using Constants;

public interface HdpLeagueService
{
    Task BuildSingleHdpLeagueReport(DateTime eventDate);
}

public class HdpLeagueServiceImp : HdpLeagueService
{
    private readonly DataReportService dataReportService;

    private readonly Repository repository;

    public HdpLeagueServiceImp(
        DataReportService dataReportService,
        Repository repository)
    {
        this.dataReportService = dataReportService;
        this.repository = repository;
    }

    public async Task BuildSingleHdpLeagueReport(DateTime eventDate)
    {
        var leagues = await repository.GetLeagueByDateAndBettype(eventDate, Constant.Hdpid);
        var dataReport = dataReportService.BuildSingleLeagueGroupReport(leagues, Constant.Hdpid);
        await dataReportService.InsertLeagueReport(dataReport);
    }
}