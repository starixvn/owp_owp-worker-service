namespace OWP.SumReport.PrematchReport.Services.League;

using Models;

public interface DataReportService
{
    IList<DataReport> BuildSingleLeagueGroupReport(IList<LeagueInfo> leagues, int bettype);

    Task InsertLeagueReport(IList<DataReport> dataReports);
}

public class DataReportBuilderServiceImp : DataReportService
{
    private readonly DbWriterService dbWriterService;

    public DataReportBuilderServiceImp(DbWriterService dbWriterService)
    {
        this.dbWriterService = dbWriterService;
    }
    public IList<DataReport> BuildSingleLeagueGroupReport(IList<LeagueInfo> leagues, int bettype)
    {
        var dataReport = new List<DataReport>();
        var group = leagues
            .GroupBy(league => new { league.UserId, league.GroupId, league.EventDate })
            .ToDictionary(x => x.Key, x => x.Count());

        foreach (var key in group.Keys)
        {
            var amount = group[key];
            dataReport.Add(new DataReport
            {
                UserId = key.UserId,
                EventDate = key.EventDate,
                GroupId = key.GroupId,
                Amount = amount,
                BetType = bettype
            });
        }

        return dataReport;
    }

    public async Task InsertLeagueReport(IList<DataReport> dataReports)
        => await dbWriterService.InsertLeague(dataReports).ConfigureAwait(false);
}