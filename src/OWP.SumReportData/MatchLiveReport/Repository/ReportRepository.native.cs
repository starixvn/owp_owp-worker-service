using Fanex.Data.Repository;
using Olympus.Xml;
using OWP.SumReportData.MatchLiveReport.Criteria;
using OWP.SumReportData.MatchLiveReport.Models;

namespace OWP.SumReportData.MatchLiveReport.Repository;

public interface ReportRepository
{
    Task InsertData(IEnumerable<ReportModel> reportData);

    Task<IEnumerable<OWPMatch>> QueryMatchesByDate(DateTime eventDate);
}

public class ReportRepositoryImp : ReportRepository
{
    private readonly IDynamicRepository dynamicRepository;

    public ReportRepositoryImp(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }

    public async Task InsertData(IEnumerable<ReportModel> reportData)
        => await dynamicRepository.ExecuteAsync(new InsertReportDataCriteria { ReportData = XmlConverter.Serialize(reportData) });

    public async Task<IEnumerable<OWPMatch>> QueryMatchesByDate(DateTime eventDate)
        => await dynamicRepository.FetchAsync<OWPMatch>(new QueryMatchesByDateCriteria { EventDate = eventDate });
}

