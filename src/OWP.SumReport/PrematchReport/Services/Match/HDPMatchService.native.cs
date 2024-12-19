namespace OWP.SumReport.PrematchReport.Services.Match;

using Constants;

public interface HdpMatchService
{
    Task BuildSingleHdpMatchReport(DateTime eventDate, TimeSpan? startNightShift);
}

public class HdpMatchServiceImp(Repository repository, DataMatchReportService dataMatchReportService)
    : HdpMatchService
{
    public async Task BuildSingleHdpMatchReport(DateTime eventDate, TimeSpan? startNightShift)
    {
        var leagueDetails = await repository.GetLeagueDetailByDate(eventDate, Constant.HdpId);
        var data = await dataMatchReportService.BuildMatchReport(leagueDetails, eventDate, startNightShift, Constant.HdpId);
        await dataMatchReportService.InsertMatchReport(data);
    }
}