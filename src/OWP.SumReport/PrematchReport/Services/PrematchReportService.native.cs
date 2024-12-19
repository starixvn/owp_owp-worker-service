namespace OWP.SumReport.PrematchReport.Services;

using League;
using Match;

public interface PrematchReportService
{
    Task CalculateDataReport(DateTime eventDate);
}

public class PrematchReportServiceImp(
    LeagueService leagueService,
    MatchService matchService)
    : PrematchReportService
{
    public async Task CalculateDataReport(DateTime eventDate)
    {
        await leagueService.BuildLeagueReport(eventDate).ConfigureAwait(false);
        await matchService.BuildMatchReportAsync(eventDate).ConfigureAwait(false);
    }
}