using Fanex.Data.Repository;
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

    public Task InsertData(IEnumerable<ReportModel> reportData)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<OWPMatch>> QueryMatchesByDate(DateTime eventDate)
        => await dynamicRepository.FetchAsync<OWPMatch>(new QueryMatchesByDateCriteria { EventDate = eventDate });
}

