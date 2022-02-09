using OWP.Core.Extensions;
using OWP.PrematchReport.Criteria;
using OWP.PrematchReport.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OWP.PrematchReport.Services
{
    public interface DbWriterService
    {
        Task InsertLeague(IList<DataReport> dataReports);
    }

    public class DbWriterServiceImp : DbWriterService
    {
        private readonly Repository repository;

        public DbWriterServiceImp(Repository repository)
        {
            this.repository = repository;
        }

        public async Task InsertLeague(IList<DataReport> dataReports)
        {
            var criteria = new InsertLeagueDataReportCriteria
            {
                XML = dataReports.ToXml().ToString(SaveOptions.DisableFormatting)
            };
            await repository.InsertLeague(criteria);
        }
    }
}