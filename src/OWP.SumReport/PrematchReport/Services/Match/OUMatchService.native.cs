namespace OWP.SumReport.PrematchReport.Services.Match;

using Constants;

public interface OuMatchService
{
    Task BuildSingleOuMatchReport(DateTime eventDate, TimeSpan? startNightShift);
}

public class OuMatchServiceImp(Repository repository, DataMatchReportService dataMatchReportService)
    : OuMatchService
{
    public async Task BuildSingleOuMatchReport(DateTime eventDate, TimeSpan? startNightShift)
    {
        var leagueDetails = await repository.GetLeagueDetailByDate(eventDate, Constant.OuId);
        var data = await dataMatchReportService.BuildMatchReport(leagueDetails, eventDate, startNightShift, Constant.OuId);
        await dataMatchReportService.InsertMatchReport(data);
    }
}