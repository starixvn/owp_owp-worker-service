using OWP.PrematchReport.Constants;

namespace OWP.PrematchReport.Services.Match
{
  public interface HDPMatchService
  {
    Task BuildSingleHDPMatchReport(DateTime eventDate, TimeSpan? startNightShift);
  }

  public class HDPMatchServiceImp : HDPMatchService
  {
    private readonly Repository repository;

    private readonly DataMatchReportService dataReportMatchService;

    public HDPMatchServiceImp(Repository repository, DataMatchReportService dataReportMatchService)
    {
      this.repository = repository;
      this.dataReportMatchService = dataReportMatchService;
    }

    public async Task BuildSingleHDPMatchReport(DateTime eventDate, TimeSpan? startNightShift)
    {
      var leagueDetails = await repository.GetLeagueDetailByDate(eventDate, Constant.HDPID);
      var data = await dataReportMatchService.BuildMatchReport(eventDate, startNightShift, leagueDetails, Constant.HDPID);
      await dataReportMatchService.InsertMatchReport(data);
    }
  }
}