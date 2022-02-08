namespace OWP.PrematchReport.Services.League
{
    using Constants;
    public interface LeagueService
    {
        Task BuildLeagueReport(DateTime eventDate);
    }

    public class LeagueServiceImpl : LeagueService
    {
        private readonly Repository repository;

        public LeagueServiceImpl(Repository repository)
        {
            this.repository = repository;
        }
        public async Task BuildLeagueReport(DateTime eventDate)
        {
            var allLeague = await repository.GetLeagueByDate(eventDate);
            var ouLeague = allLeague.Where(league => league.Bettype == Constant.OUID);
            var hdpLeague = allLeague.Where(league => league.Bettype == Constant.HDPID);
        }
    }
}