namespace OWP.SumReport.PrematchReport.Services.League;

public interface OULeagueService
{
    Task BuildSingleOULeagueReport(DateTime eventDate);
}

public class OULeagueServiceImp : OULeagueService
{
    private readonly DataReportService dataReportService;

    private readonly Repository repository;

    public OULeagueServiceImp(
        DataReportService dataReportService,
        Repository repository)
    {
        this.dataReportService = dataReportService;
        this.repository = repository;
    }

    public async Task BuildSingleOULeagueReport(DateTime eventDate)
    {
        var leagues = await repository.GetLeagueByDateAndBettype(eventDate, Constants.Constant.OUID);
        var dataReport = dataReportService.BuildSingleLeagueGroupReport(leagues, Constants.Constant.OUID);
        await dataReportService.InsertLeagueReport(dataReport);
    }
}