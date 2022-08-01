using OWP.PrematchReport.Services.League;
using OWP.PrematchReport.Services.Match;

namespace OWP.PrematchReport.Services
{
    public interface PrematchReportService
    {
        Task CalculateDataReport();
    }

    public class PrematchReportServiceImp : PrematchReportService
    {
        private readonly LeagueService leagueService;

        private readonly MatchService matchService;

        public PrematchReportServiceImp(
            LeagueService leagueService,
            MatchService matchService)
        {
            this.leagueService = leagueService;
            this.matchService = matchService;
        }

        public async Task CalculateDataReport()
        {
            var fromTime = new DateTime(2021, 01, 01); ;
            var toTime = DateTime.Today;

            for (DateTime date = fromTime; date < toTime; date = date.AddDays(1))
            {
                await CalculateDataReport(date);
                Console.WriteLine(date.ToString());
            }
        }

        public async Task CalculateDataReport(DateTime eventDate)
        {
            await leagueService.BuildLeagueReport(eventDate).ConfigureAwait(false);
            await matchService.BuildMatchReportAsync(eventDate).ConfigureAwait(false);
        }
    }
}