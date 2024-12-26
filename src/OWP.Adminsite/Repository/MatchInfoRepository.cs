using Fanex.Data.Repository;
using OWP.Adminsite.Criteria;
using OWP.Adminsite.Models;

namespace OWP.Adminsite.Repository;

public interface IMatchInfoRepository
{
    Task<IEnumerable<MatchInfo>> GetMatchInfoAsync(DateTime fromDate, DateTime toDate);
    Task<IEnumerable<Assignment>> GetAssignmentAsync(string matchIds);
    Task<IEnumerable<MatchSetting>> GetMatchSettingAsync(string matchIds);
    Task<(IEnumerable<MatchRecommendOdd>, IEnumerable<MatchRecommendOddDetail>)> GetMatchOddAsync(string matchIds);
    Task<IEnumerable<MatchOffer>> GetMatchOfferAsync(string matchIds);
    Task<IEnumerable<DefaultSecondAcceptValue>> GetDefaultSecondAcceptValueAsync();
    Task<IEnumerable<MaxbetSetting>> GetDefaultMaxbetSettingAsync();
}

public class MatchInfoRepository(IDynamicRepository dynamicRepository) : IMatchInfoRepository
{
    public async Task<IEnumerable<MatchInfo>> GetMatchInfoAsync(DateTime fromDate, DateTime toDate)
    {
        var criteria = new GetMatchInfoSyncToAdminSiteCriteria
        {
            FromDate = fromDate,
            ToDate = toDate
        };
        return await dynamicRepository.FetchAsync<MatchInfo>(criteria);
    }

    public async Task<IEnumerable<Assignment>> GetAssignmentAsync(string matchIds)
    {
        var criteria = new GetAssignmentSyncToAdminSiteCriteria
        {
            MatchIds = matchIds
        };
        return await dynamicRepository.FetchAsync<Assignment>(criteria);
    }

    public async Task<IEnumerable<MatchSetting>> GetMatchSettingAsync(string matchIds)
    {
        var criteria = new GetMatchSettingSyncToAdminSiteCriteria
        {
            MatchIds = matchIds
        };
        return await dynamicRepository.FetchAsync<MatchSetting>(criteria);
    }

    public async Task<(IEnumerable<MatchRecommendOdd>, IEnumerable<MatchRecommendOddDetail>)> GetMatchOddAsync(string matchIds)
    {
        var criteria = new GetMatchOddSyncToAdminSiteCriteria
        {
            MatchIds = matchIds
        };
        return await dynamicRepository.FetchMultipleAsync<MatchRecommendOdd, MatchRecommendOddDetail>(criteria);
    }

    public async Task<IEnumerable<MatchOffer>> GetMatchOfferAsync(string matchIds)
    {
        var criteria = new GetMatchOfferSyncToAdminSiteCriteria
        {
            MatchIds = matchIds
        };
        return await dynamicRepository.FetchAsync<MatchOffer>(criteria);
    }

    public async Task<IEnumerable<DefaultSecondAcceptValue>> GetDefaultSecondAcceptValueAsync()
    {
        var criteria = new GetDefaultSecondAcceptValueCriteria();
        return await dynamicRepository.FetchAsync<DefaultSecondAcceptValue>(criteria);
    }

    public async Task<IEnumerable<MaxbetSetting>> GetDefaultMaxbetSettingAsync()
    {
        var criteria = new GetMaxbetSettingCriteria();
        return await dynamicRepository.FetchAsync<MaxbetSetting>(criteria);
    }
}
