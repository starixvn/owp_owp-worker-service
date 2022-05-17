using Fanex.Data.Repository;
using Olympus.Xml;
using OWP.SumReportData.MatchLiveReport.Criteria;
using OWP.SumReportData.MatchLiveReport.Models;

namespace OWP.SumReportData.MatchLiveReport.Repository;

public interface ReportRepository
{
    Task InsertData(IEnumerable<ReportModel> reportData, DateTime eventDate);

    Task<(IEnumerable<OWPMatch>, IEnumerable<OWPMatch>)> QueryMatchesByDate(DateTime eventDate);

    Task<IEnumerable<int>> GetSaba5minOWPMatchId(IList<int> Ids);
}

public class ReportRepositoryImp : ReportRepository
{
    private readonly IDynamicRepository dynamicRepository;

    public ReportRepositoryImp(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }

    public async Task InsertData(IEnumerable<ReportModel> reportData, DateTime eventDate)
        => await dynamicRepository.ExecuteAsync(new InsertReportDataCriteria { Data = XmlConverter.Serialize(reportData), EventDate = eventDate });

    public async Task<(IEnumerable<OWPMatch>, IEnumerable<OWPMatch>)> QueryMatchesByDate(DateTime eventDate)
        => await dynamicRepository.FetchMultipleAsync<OWPMatch, OWPMatch>(new QueryMatchesByDateCriteria { EventDate = eventDate });
    
    public async Task<IEnumerable<int>> GetSaba5minOWPMatchId(IList<int> Ids)
    {
        var criteria = new GetSabaFiveMinMatchIdCriteria
        {
            Id = String.Join(',', Ids)
        };

        return await dynamicRepository.FetchAsync<int>(criteria);
    }
}

