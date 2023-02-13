namespace OWP.SumReport.PrematchReport.Services.League;

using Constants;

public interface OuLeagueService
{
    Task BuildSingleOuLeagueReport(DateTime eventDate);
}

public class OuLeagueServiceImp : OuLeagueService
{
    private readonly DataReportService dataReportService;

    private readonly Repository repository;

    public OuLeagueServiceImp(
        DataReportService dataReportService,
        Repository repository)
    {
        this.dataReportService = dataReportService;
        this.repository = repository;
    }

    public async Task BuildSingleOuLeagueReport(DateTime eventDate)
    {
        var leagues = await repository.GetLeagueByDateAndBettype(eventDate, Constant.OuId);
        var dataReport = dataReportService.BuildSingleLeagueGroupReport(leagues, Constant.OuId);
        await dataReportService.InsertLeagueReport(dataReport);
    }
}