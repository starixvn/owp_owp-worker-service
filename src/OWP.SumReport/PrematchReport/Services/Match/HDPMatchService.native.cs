namespace OWP.SumReport.PrematchReport.Services.Match;

using Constants;

public interface HdpMatchService
{
    Task BuildSingleHdpMatchReport(DateTime eventDate, TimeSpan? startNightShift);
}

public class HdpMatchServiceImp : HdpMatchService
{
    private readonly Repository repository;

    private readonly DataMatchReportService dataMatchReportService;

    public HdpMatchServiceImp(Repository repository, DataMatchReportService dataMatchReportService)
    {
        this.repository = repository;
        this.dataMatchReportService = dataMatchReportService;
    }

    public async Task BuildSingleHdpMatchReport(DateTime eventDate, TimeSpan? startNightShift)
    {
        var leagueDetails = await repository.GetLeagueDetailByDate(eventDate, Constant.Hdpid);
        var data = await dataMatchReportService.BuildMatchReport(leagueDetails, eventDate, startNightShift, Constant.Hdpid);
        await dataMatchReportService.InsertMatchReport(data);
    }
}