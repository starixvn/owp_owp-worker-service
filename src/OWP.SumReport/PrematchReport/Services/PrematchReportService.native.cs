using OWP.SumReport.PrematchReport.Services.League;
using OWP.SumReport.PrematchReport.Services.Match;

namespace OWP.SumReport.PrematchReport.Services;

public interface PrematchReportService
{
    Task CalculateDataReport(DateTime eventDate);
}

public class PrematchReportServiceImp : PrematchReportService
{
    private readonly LeagueService leagueService;

    private readonly MatchService matchService;

    public PrematchReportServiceImp(
        LeagueService leagueService,
        MatchService matchService)
    {
        this.leagueService = leagueService;
        this.matchService = matchService;
    }

    public async Task CalculateDataReport(DateTime eventDate)
    {
        await leagueService.BuildLeagueReport(eventDate).ConfigureAwait(false);
        await matchService.BuildMatchReportAsync(eventDate).ConfigureAwait(false);
    }
}