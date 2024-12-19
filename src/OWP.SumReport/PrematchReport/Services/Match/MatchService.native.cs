namespace OWP.SumReport.PrematchReport.Services.Match;

public interface MatchService
{
    Task BuildMatchReportAsync(DateTime eventDate);
}

public class MatchServiceImp(Repository repository, HdpMatchService hdpMatchService, OuMatchService ouMatchService)
    : MatchService
{
    public async Task BuildMatchReportAsync(DateTime eventDate)
    {
        var startNightShift = await repository.QueryExistedStartTime(eventDate.Date);
        await hdpMatchService.BuildSingleHdpMatchReport(eventDate.Date, startNightShift);
        await ouMatchService.BuildSingleOuMatchReport(eventDate.Date, startNightShift);
    }
}