namespace OWP.PrematchReport.Services.Match
{
    using OWP.PrematchReport.Constants;

    public interface OUMatchService
    {
        Task BuildSingleOUMatchReport(DateTime eventDate, TimeSpan? startNightShift);
    }

    public class OUMatchServiceImp : OUMatchService
    {
        private readonly Repository repository;

        private readonly DataMatchReportService dataMatchReportService;

        public OUMatchServiceImp(Repository repository, DataMatchReportService dataMatchReportService)
        {
            this.repository = repository;
            this.dataMatchReportService = dataMatchReportService;
        }

        public async Task BuildSingleOUMatchReport(DateTime eventDate, TimeSpan? startNightShift)
        {
            var leagueDetails = await repository.GetLeagueDetailByDate(eventDate, Constant.OUID);
            var data = await dataMatchReportService.BuildMatchReport(eventDate, startNightShift, leagueDetails, Constant.OUID);
            await dataMatchReportService.InsertMatchReport(data);
        }
    }
}