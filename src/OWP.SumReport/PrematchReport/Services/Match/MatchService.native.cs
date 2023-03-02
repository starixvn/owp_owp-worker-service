namespace OWP.SumReport.PrematchReport.Services.Match;

public interface MatchService
{
    Task BuildMatchReportAsync(DateTime eventDate);
}

public class MatchServiceImp : MatchService
{
    private readonly Repository repository;
    private readonly HdpMatchService hdpMatchService;
    private readonly OuMatchService ouMatchService;

    public MatchServiceImp(Repository repository, HdpMatchService hdpMatchService, OuMatchService ouMatchService)
    {
        this.repository = repository;
        this.ouMatchService = ouMatchService;
        this.hdpMatchService = hdpMatchService;
    }

    public async Task BuildMatchReportAsync(DateTime eventDate)
    {
        var startNightShift = await repository.QueryExistedStartTime(eventDate.Date);
        await hdpMatchService.BuildSingleHdpMatchReport(eventDate.Date, startNightShift);
        await ouMatchService.BuildSingleOuMatchReport(eventDate.Date, startNightShift);
    }
}