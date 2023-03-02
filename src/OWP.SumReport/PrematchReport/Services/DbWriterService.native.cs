namespace OWP.SumReport.PrematchReport.Services;

using Criteria;
using Models;
using Olympus.Xml;

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
            Xml = XmlConverter.Serialize(dataReports)
        };
        await repository.InsertLeague(criteria);
    }

    public async Task InsertMatch(IList<DataReport> dataReports)
    {
        var criteria = new InsertMatchDataReportCriteria
        {
            Xml = XmlConverter.Serialize(dataReports)
        };
        await repository.InsertMatch(criteria);
    }
}