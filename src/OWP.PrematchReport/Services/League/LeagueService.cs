namespace OWP.PrematchReport.Services.League
{
    using Constants;
    using System;
    using System.Threading.Tasks;

    public interface LeagueService
    {
        Task BuildLeagueReport(DateTime eventDate);
    }

    public class LeagueServiceImp : LeagueService
    {
        private readonly OULeagueService ouLeagueService;


        public LeagueServiceImp(OULeagueService ouLeagueService)
        {
            this.ouLeagueService = ouLeagueService;
        }
        public async Task BuildLeagueReport(DateTime eventDate)
        {
            await ouLeagueService.BuildOULeagueReport(eventDate);
            // TODO
        }
    }
}