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
    ,IApiService apiService
    ,IMatchInfoService matchInfoService) : ISyncDataService
{
    public async Task PushMatchInfoAsync(DateTime eventDate)
    {
        var fromDate = eventDate.Date.AddHours(12);
        var toDate = fromDate.AddDays(1).AddMilliseconds(-1);
        const byte BatchSize = 50;
        var cacheKey = $"OWP-PushMatchInfoToAdminSite-{eventDate:yyyyMMdd}";
        (_, var oldCacheMatchInfos) = await cache.TryGetAsync<string>(cacheKey);
        var oldMatchInfos = !string.IsNullOrEmpty(oldCacheMatchInfos)
            ? JsonConvert.DeserializeObject<List<MatchInfoApiModel>>(oldCacheMatchInfos)
            : [];

        var matchInfos = await matchInfoService.GetMatchInfoAsync(fromDate, toDate);
        var unsyncedMatchInfos = matchInfos.Except(oldMatchInfos, new MatchInfoApiModel()).ToList();
        var deletedMatches = oldMatchInfos.Where(x => !matchInfos.Any(y => y.MatchId == x.MatchId && y.SubType == x.SubType))
            .Select(x => new MatchInfoApiModel
            {
                MatchId = x.MatchId,
                SubType = x.SubType,
                MatchSettings = x.MatchSettings.Select(s => new MatchSettingApiModel { BettypeId = s.BettypeId }).ToList()
            }).ToList();

        unsyncedMatchInfos = [.. unsyncedMatchInfos, .. deletedMatches];
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
        await cache.SetAsync(cacheKey, JsonConvert.SerializeObject(cacheMatchInfos), cachePolicy);
    }
}