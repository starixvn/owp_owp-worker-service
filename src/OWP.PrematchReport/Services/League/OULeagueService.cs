using System;
using System.Threading.Tasks;

namespace OWP.PrematchReport.Services.League
{
    public interface OULeagueService
    {
        Task BuildOULeagueReport(DateTime eventDate);
    }

    public class OULeagueServiceImp : OULeagueService
    {
        private readonly DataReportBuilderService dataReportBuilder;

        private readonly ReportIdBuilder reportIdBuilder;

        private readonly Repository repository;

        private readonly DbWriterService dbWriterService;

        public OULeagueServiceImp(
            DataReportBuilderService dataReportBuilder,
            ReportIdBuilder reportIdBuilder,
            Repository repository,
            DbWriterService dbWriterService)
        {
            this.dataReportBuilder = dataReportBuilder;
            this.reportIdBuilder = reportIdBuilder;
            this.repository = repository;
            this.dbWriterService = dbWriterService;
        }

        public async Task BuildOULeagueReport(DateTime eventDate)
        {
            var leagues = await repository.GetLeagueByDateAndBettype(eventDate, Constants.Constant.OUID);
            
            var dataReport = dataReportBuilder.Build(leagues, Constants.Constant.OUID);
            await reportIdBuilder.BuildReportId(dataReport);

            await dbWriterService.InsertLeague(dataReport).ConfigureAwait(false);
        }
    }
}