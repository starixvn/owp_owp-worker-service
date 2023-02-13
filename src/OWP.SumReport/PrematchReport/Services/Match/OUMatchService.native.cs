namespace OWP.SumReport.PrematchReport.Services.Match;

using Constants;

public interface OuMatchService
{
    Task BuildSingleOuMatchReport(DateTime eventDate, TimeSpan? startNightShift);
}

public class OuMatchServiceImp : OuMatchService
{
    private readonly Repository repository;

    private readonly DataMatchReportService dataMatchReportService;

    public OuMatchServiceImp(Repository repository, DataMatchReportService dataMatchReportService)
    {
        this.repository = repository;
        this.dataMatchReportService = dataMatchReportService;
    }

    public async Task BuildSingleOuMatchReport(DateTime eventDate, TimeSpan? startNightShift)
    {
        var leagueDetails = await repository.GetLeagueDetailByDate(eventDate, Constant.Ouid);
        var data = await dataMatchReportService.BuildMatchReport(leagueDetails, eventDate, startNightShift, Constant.Ouid);
        await dataMatchReportService.InsertMatchReport(data);
    }
}