using OWP.SumReport.PrematchReport.Criteria;
using OWP.SumReport.PrematchReport.Models;
using System.Xml.Linq;
using Olympus.Xml;
namespace OWP.SumReport.PrematchReport.Services;

public interface DbWriterService
{
    Task InsertLeague(IList<DataReport> dataReports);

    Task InsertMatch(IList<DataReport> dataReports);
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
            XML = XmlConverter.Serialize(dataReports)
        };
        await repository.InsertLeague(criteria);
    }

    public async Task InsertMatch(IList<DataReport> dataReports)
    {
        var criteria = new InsertMatchDataReportCriteria
        {
            XML = XmlConverter.Serialize(dataReports)
        };
        await repository.InsertMatch(criteria);
    }
}