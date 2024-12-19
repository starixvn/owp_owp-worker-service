namespace OWP.SumReport.PrematchReport.Services.League;

public interface LeagueService
{
    Task BuildLeagueReport(DateTime eventDate);
}

public class LeagueServiceImp(OuLeagueService ouLeagueService, HdpLeagueService hdpLeagueService)
    : LeagueService
{
    public async Task BuildLeagueReport(DateTime eventDate)
    {
        await ouLeagueService.BuildSingleOuLeagueReport(eventDate);
        await hdpLeagueService.BuildSingleHdpLeagueReport(eventDate);
    }
}