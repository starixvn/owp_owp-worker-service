namespace OWP.SumReport.PrematchReport.Services.League;

using Constants;

public interface HdpLeagueService
{
    Task BuildSingleHdpLeagueReport(DateTime eventDate);
}

public class HdpLeagueServiceImp(
    DataReportService dataReportService,
    Repository repository)
    : HdpLeagueService
{
    public async Task BuildSingleHdpLeagueReport(DateTime eventDate)
    {
        var leagues = await repository.GetLeagueByDateAndBettype(eventDate, Constant.HdpId);
        var dataReport = dataReportService.BuildSingleLeagueGroupReport(leagues, Constant.HdpId);
        await dataReportService.InsertLeagueReport(dataReport);
    }
}