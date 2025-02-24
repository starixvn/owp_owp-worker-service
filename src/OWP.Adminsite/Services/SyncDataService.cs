using Fanex.Caching;
using Newtonsoft.Json;
using OWP.Adminsite.Models;

namespace OWP.Adminsite.Services;

public interface ISyncDataService
{
    Task PushMatchInfoAsync(DateTime eventDate);
}

public class SyncDataService(
    ICache cache
    , IApiService apiService
    , IMatchInfoService matchInfoService) : ISyncDataService
{
    public async Task PushMatchInfoAsync(DateTime eventDate)
    {
        const byte BatchSize = 50;
        var fromDate = eventDate.Date.AddHours(12);
        var toDate = fromDate.AddDays(1).AddMilliseconds(-1);
        var cacheKey = $"OWP-SyncMatchInfoToAdminSite-{eventDate:yyyyMMdd}";
        var (_, oldCacheMatchInfos) = await cache.TryGetAsync<string>(cacheKey);
        var oldMatchInfos = !string.IsNullOrEmpty(oldCacheMatchInfos)
            ? JsonConvert.DeserializeObject<List<MatchInfoApiModel>>(oldCacheMatchInfos)
            : [];

        var matchInfos = await matchInfoService.GetMatchInfoAsync(fromDate, toDate);
        var unsyncedMatchInfos = matchInfos.Except(oldMatchInfos, new MatchInfoApiModel()).ToList();
        var deletedMatches = GetDeletedMatch(matchInfos, oldMatchInfos);

        unsyncedMatchInfos = [.. unsyncedMatchInfos, .. deletedMatches];
        var totalBatch = (int)Math.Ceiling((double)unsyncedMatchInfos.Count / BatchSize);
        var failedMatchInfos = new List<MatchInfoApiModel>();
        for (int i = 0; i < totalBatch; i++)
        {
            var data = unsyncedMatchInfos.Skip(i * BatchSize).Take(BatchSize).ToList();
            var result = await apiService.PushMatchInfoAsync(fromDate, toDate, data);
            failedMatchInfos.AddRange(result);
        }

        var failedDeletedMatches = failedMatchInfos.Where(x => deletedMatches.Exists(y => x.MatchId == y.MatchId && x.SubType == y.SubType)).ToList();

        var cacheMatchInfos = matchInfos.Except(failedMatchInfos, new MatchInfoApiModel()).Concat(failedDeletedMatches);
        var cachePolicy = new CacheItemOptions().SetAbsoluteExpiration(DateTime.Today.AddDays(2));
        await cache.SetAsync(cacheKey, JsonConvert.SerializeObject(cacheMatchInfos), cachePolicy);
    }

    private static List<MatchInfoApiModel> GetDeletedMatch(List<MatchInfoApiModel> matchInfos, List<MatchInfoApiModel> oldMatchInfos)
    {
        return oldMatchInfos.Where(x => !matchInfos.Exists(y => y.MatchId == x.MatchId && y.SubType == x.SubType))
            .Select(x => new MatchInfoApiModel
            {
                MatchId = x.MatchId,
                SubType = x.SubType,
                MatchSettings = x.MatchSettings.Select(s => new MatchSettingApiModel { Bettype = s.Bettype }).ToList()
            }).ToList();
    }
}