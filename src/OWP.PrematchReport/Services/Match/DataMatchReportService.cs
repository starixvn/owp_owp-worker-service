namespace OWP.PrematchReport.Services.Match
{
  using OWP.PrematchReport.Models;

  public interface DataMatchReportService
  {
    Task<IList<DataReport>> BuildMatchReport(DateTime eventDate, TimeSpan? startNightShift, IList<LeagueDetailInfo> leagueDetailInfos, int bettype);

    Task InsertMatchReport(IList<DataReport> dataReports);
  }

  public class DataMatchReportServiceImp : DataMatchReportService
  {
    private readonly Repository repository;
    private readonly DbWriterService dbWriterService;

    public DataMatchReportServiceImp(Repository repository, DbWriterService dbWriterService)
    {
      this.repository = repository;
      this.repository = repository;
      this.dbWriterService = dbWriterService;
    }

    public async Task<IList<DataReport>> BuildMatchReport(DateTime eventDate, TimeSpan? startNightShift, IList<LeagueDetailInfo> leagueDetailInfos, int bettype)
    {
      var preMatchIds = leagueDetailInfos.Select(i => i.PrematchLeagueId).ToList();
      var totalDataMatches = await GetTotalMatch(preMatchIds, eventDate, startNightShift);
      var leagueInfoDict = leagueDetailInfos.GroupBy(x => new { x.UserId, x.EventDate, x.GroupId })
          .ToDictionary(x => x.Key, x => x.ToList());
      var dataReport = new List<DataReport>();
      foreach (var item in leagueInfoDict.Keys)
      {
        var totalAssignment = GetTotalAssignment(leagueInfoDict[item], totalDataMatches);
        dataReport.Add(new DataReport
        {
          UserId = item.UserId,
          EventDate = item.EventDate,
          GroupId = item.GroupId,
          Amount = totalAssignment,
          BetType = bettype
        });
      }
      return dataReport;
    }

    private int GetTotalAssignment(IList<LeagueDetailInfo> leagueDetailInfos, IList<TotalDataMatch> totalDataMatches)
    {
      var totalAssignment = 0;
      var leagueInfoByleaugeId = leagueDetailInfos.GroupBy(x => x.OWPLeagueId)
          .ToDictionary(x => x.Key, x => x.ToList());
      foreach (var item in leagueInfoByleaugeId.Keys)
      {
        var totalMatch = totalDataMatches.FirstOrDefault(x => x.leaugeId == item);
        totalAssignment += leagueInfoByleaugeId[item].Any(x => x.IsDay)
            ? totalMatch.TotalMatchDayShift * leagueInfoByleaugeId[item].Where(x => x.IsDay).ToList().Count
            : totalMatch.TotalMatchNightShift * leagueInfoByleaugeId[item].Where(x => !x.IsDay).ToList().Count;
      }

      return totalAssignment;
    }

    private async Task<IList<TotalDataMatch>> GetTotalMatch(IList<int> preMatchIds, DateTime eventDate, TimeSpan? startNightShift)
    {
      var fromDate = eventDate.AddHours(12);
      var fromDateNightShift = startNightShift.HasValue ? CalculateQueryDateTime(startNightShift, eventDate) : fromDate;
      var toDate = eventDate.AddDays(1).AddHours(11).AddMinutes(59).AddSeconds(59);

      return await repository.GetTotalMatchByLeagueIds(preMatchIds, fromDate, toDate, fromDateNightShift);
    }

    private static DateTime CalculateQueryDateTime(TimeSpan? time, DateTime eventDate)
    {
      if (time.Value.CompareTo(new TimeSpan(0, 0, 0)) >= 0 && time.Value.CompareTo(new TimeSpan(11, 59, 59)) <= 0)
      {
        return eventDate.Date.AddDays(1).Add(time.Value);
      }

      return eventDate.Date.Add(time.Value);
    }

    public async Task InsertMatchReport(IList<DataReport> dataReports)
    => await dbWriterService.InsertMatch(dataReports).ConfigureAwait(false);
  }
}