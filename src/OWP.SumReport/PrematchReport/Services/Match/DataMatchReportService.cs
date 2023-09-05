namespace OWP.SumReport.PrematchReport.Services.Match;

using Models;

public interface DataMatchReportService
{
    Task<IList<DataReport>> BuildMatchReport(IList<LeagueDetailInfo> leagueDetailInfos, DateTime eventDate, TimeSpan? startNightShift, int bettype);

    Task InsertMatchReport(IList<DataReport> dataReports);
}

public class DataMatchReportServiceImp : DataMatchReportService
{
    private readonly Repository repository;
    private readonly DbWriterService dbWriterService;

    public DataMatchReportServiceImp(Repository repository, DbWriterService dbWriterService)
    {
        this.repository = repository;
        this.dbWriterService = dbWriterService;
    }

    public async Task<IList<DataReport>> BuildMatchReport(IList<LeagueDetailInfo> leagueDetailInfos, DateTime eventDate, TimeSpan? startNightShift, int bettype)
    {
        var preMatchLeagueIds = leagueDetailInfos.Select(i => i.PreMatchLeagueId).ToList();
        var totalDataMatches = await GetTotalMatch(preMatchLeagueIds, eventDate, startNightShift);
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

    private static int GetTotalAssignment(IEnumerable<LeagueDetailInfo> leagueDetailInfos, IList<TotalDataMatch> totalDataMatches)
    {
        var totalAssignment = 0;
        var leagueInfoByLeagueId = leagueDetailInfos.GroupBy(x => x.OwpLeagueId)
            .ToDictionary(x => x.Key, x => x.ToList());
        foreach (var item in leagueInfoByLeagueId.Keys)
        {
            var totalMatch = totalDataMatches.Where(x => x.OwpLeagueId == item).ToList();
            if (totalMatch.Any())
            {
                totalAssignment += leagueInfoByLeagueId[item].Exists(x => x.IsDay)
                    ? totalMatch.Find(x => x.IsMatchDay)?.Total ?? 0
                    : totalMatch.Find(x => !x.IsMatchDay)?.Total ?? 0;
            }
        }

        return totalAssignment;
    }

    private async Task<IList<TotalDataMatch>> GetTotalMatch(IList<int> preMatchIds, DateTime eventDate, TimeSpan? startNightShift)
    {
        var fromDate = eventDate.AddHours(12);
        var fromDateNightShift = startNightShift.HasValue ?
            CalculateQueryDateTime(startNightShift.Value, eventDate) : fromDate;
        var toDate = eventDate.AddDays(1).AddHours(11).AddMinutes(59).AddSeconds(59);

        return await repository.GetTotalMatchByLeagueIds(preMatchIds, fromDate, toDate, fromDateNightShift);
    }

    private static DateTime CalculateQueryDateTime(TimeSpan time, DateTime eventDate)
    {
        if (time.CompareTo(new TimeSpan(0, 0, 0)) >= 0 && time.CompareTo(new TimeSpan(11, 59, 59)) <= 0)
        {
            return eventDate.Date.AddDays(1).Add(time);
        }

        return eventDate.Date.Add(time);
    }

    public async Task InsertMatchReport(IList<DataReport> dataReports)
         => await dbWriterService.InsertMatch(dataReports).ConfigureAwait(false);
}