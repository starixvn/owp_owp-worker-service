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
        private readonly HDPLeagueService hdpLeagueService;

        public LeagueServiceImp(OULeagueService ouLeagueService, HDPLeagueService hdpLeagueService)
        {
            this.ouLeagueService = ouLeagueService;
            this.hdpLeagueService = hdpLeagueService;
        }

        public async Task BuildLeagueReport(DateTime eventDate)
        {
            await ouLeagueService.BuildSingleOULeagueReport(eventDate);
            await hdpLeagueService.BuildSingleHDPLeagueReport(eventDate);
        }
    }
}