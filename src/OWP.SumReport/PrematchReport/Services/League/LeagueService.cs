namespace OWP.SumReport.PrematchReport.Services.League;

public interface LeagueService
{
    Task BuildLeagueReport(DateTime eventDate);
}

public class LeagueServiceImp : LeagueService
{
    private readonly OuLeagueService ouLeagueService;
    private readonly HdpLeagueService hdpLeagueService;

    public LeagueServiceImp(OuLeagueService ouLeagueService, HdpLeagueService hdpLeagueService)
    {
        this.ouLeagueService = ouLeagueService;
        this.hdpLeagueService = hdpLeagueService;
    }

    public async Task BuildLeagueReport(DateTime eventDate)
    {
        await ouLeagueService.BuildSingleOuLeagueReport(eventDate);
        await hdpLeagueService.BuildSingleHdpLeagueReport(eventDate);
    }
}