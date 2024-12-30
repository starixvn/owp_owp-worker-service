using Fanex.Caching;
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
        (_, var oldMatchInfos) = await cache.TryGetAsync<IEnumerable<MatchInfoApiModel>>(CacheKey);
        oldMatchInfos ??= [];

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

        var cacheMatchInfos = unsyncedMatchInfos.Except(failedMatchInfos);
        var cachePolicy = new CacheItemOptions().SetAbsoluteExpiration(DateTime.Today.AddDays(2));
        await cache.SetAsync(CacheKey, cacheMatchInfos, cachePolicy);
    }
}