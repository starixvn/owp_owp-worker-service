namespace OWP.SumReport.PrematchReport.Services.League;

using Constants;

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
        var leagues = await repository.GetLeagueByDateAndBettype(eventDate, Constant.OUID);
        var dataReport = dataReportService.BuildSingleLeagueGroupReport(leagues, Constant.OUID);
        await dataReportService.InsertLeagueReport(dataReport);
    }
}