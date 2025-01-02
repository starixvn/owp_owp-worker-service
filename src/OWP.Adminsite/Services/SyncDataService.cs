using Fanex.Caching;
using Newtonsoft.Json;
using OWP.Adminsite.Models;

namespace OWP.Adminsite.Services;

public interface ISyncDataService
{
    Task PushMatchInfoAsync(DateTime fromDate, DateTime toDate);
}

public class SyncDataService(
    ICache cache
    ,IApiService apiService
    ,IMatchInfoService matchInfoService) : ISyncDataService
{
    public async Task PushMatchInfoAsync(DateTime fromDate, DateTime toDate)
    {
        const byte BatchSize = 50;
        const string CacheKey = "OWP-PushMatchInfoToAdminSite";
        (_, var oldCacheMatchInfos) = await cache.TryGetAsync<string>(CacheKey);
        var oldMatchInfos = !string.IsNullOrEmpty(oldCacheMatchInfos)
            ? JsonConvert.DeserializeObject<List<MatchInfoApiModel>>(oldCacheMatchInfos)
            : [];

        var matchInfos = await matchInfoService.GetMatchInfoAsync(fromDate, toDate);
        var unsyncedMatchInfos = matchInfos.Except(oldMatchInfos, new MatchInfoApiModel()).ToList();

        int totalBatch = (int)Math.Ceiling((double)unsyncedMatchInfos.Count / BatchSize);
        var failedMatchInfos = new List<MatchInfoApiModel>();
        for (int i = 0; i < totalBatch; i++)
        {
            var data = unsyncedMatchInfos.Skip(i * BatchSize).Take(BatchSize).ToList();
            var result = await apiService.PushMatchInfoAsync(fromDate, toDate, data);
            failedMatchInfos.AddRange(result);
        }

        var cacheMatchInfos = matchInfos.Except(failedMatchInfos, new MatchInfoApiModel());
        var cachePolicy = new CacheItemOptions().SetAbsoluteExpiration(DateTime.Today.AddDays(2));
        await cache.SetAsync(CacheKey, JsonConvert.SerializeObject(cacheMatchInfos), cachePolicy);
    }
}