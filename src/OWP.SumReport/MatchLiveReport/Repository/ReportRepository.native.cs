namespace OWP.SumReport.MatchLiveReport.Repository;

using Criteria;
using Fanex.Data.Repository;
using Models;
using Olympus.Xml;

public interface ReportRepository
{
    Task InsertData(IEnumerable<ReportModel> reportData, DateTime eventDate);

    Task<(IEnumerable<OwpMatch>, IEnumerable<OwpMatch>)> QueryMatchesByDate(DateTime eventDate, bool isGenerateData);

    Task<IEnumerable<int>> GetSaba5MinOwpMatchId(IList<int> ids);
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

    public async Task<(IEnumerable<OwpMatch>, IEnumerable<OwpMatch>)> QueryMatchesByDate(DateTime eventDate, bool isGenerateData)
    => await dynamicRepository.FetchMultipleAsync<OwpMatch, OwpMatch>(isGenerateData
            ? new QueryFullMatchesByDateCriteria { EventDate = eventDate }
            : new QueryMatchesByDateCriteria { EventDate = eventDate });

    public async Task<IEnumerable<int>> GetSaba5MinOwpMatchId(IList<int> ids)
    {
        var criteria = new GetSabaFiveMinMatchIdCriteria
        {
            Id = string.Join(',', ids)
        };

        return await dynamicRepository.FetchAsync<int>(criteria);
    }
}

