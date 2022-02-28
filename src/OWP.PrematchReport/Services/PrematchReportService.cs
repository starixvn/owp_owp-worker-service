using OWP.PrematchReport.Models;
using OWP.PrematchReport.Services.League;
using OWP.PrematchReport.Services.Match;

namespace OWP.PrematchReport.Services
{
  public interface PrematchReportService
  {
    Task CalculateDataReport();
  }

  public class PrematchReportServiceImp : PrematchReportService
  {
    private readonly CacheManipulator cache;

    private readonly LeagueService leagueService;

    private readonly MatchService matchService;

    public PrematchReportServiceImp(CacheManipulator cache,
    LeagueService leagueService,
    MatchService matchService
    )
    {
      this.cache = cache;
      this.leagueService = leagueService;
      this.matchService = matchService;
    }

    public async Task CalculateDataReport()
    {
      var startTime = await cache.GetStartTimeFromCache();
      var latestStartTime = await CalculateDataReport(startTime, DateTime.Today);
      await cache.SaveStartTime(latestStartTime).ConfigureAwait(false);
    }

    public async Task<DateTime> CalculateDataReport(DateTime startTime, DateTime finishRunTime)
    {
      if (DateTime.Compare(startTime, finishRunTime) < 0)
      {
        await leagueService.BuildLeagueReport(startTime).ConfigureAwait(false);
        await matchService.BuildMatchReportAsync(startTime).ConfigureAwait(false);
        startTime = startTime.AddDays(1);
      }
      else
      {
        return startTime;
      }
      return await CalculateDataReport(startTime, finishRunTime);
    }
  }
}