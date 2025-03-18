using Fanex.Caching;
using Newtonsoft.Json;

using OWP.Adminsite.Models;
using Olympus.OWP.Log.Data.Repository.Criteria;
using Olympus.OWP.Log.Data.Repository.Services;

namespace OWP.Adminsite.Services;

public interface ISyncDataService
{
    Task PushMatchInfoAsync(DateTime eventDate);
}

public class SyncDataService(
    ICache cache
    , IApiService apiService
    , IMatchInfoService matchInfoService
    , LogWriterService logger) : ISyncDataService
{
    public async Task PushMatchInfoAsync(DateTime eventDate)
    {
        List<MatchInfoApiModel> unsynchronizedMatches = [];
        List<MatchInfoApiModel> failedMatchInfos = [];
        try
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
            unsynchronizedMatches = matchInfos.Except(oldMatchInfos, new MatchInfoApiModel()).ToList();
            var deletedMatches = GetDeletedMatch(matchInfos, oldMatchInfos);

            unsynchronizedMatches = [.. unsynchronizedMatches, .. deletedMatches];
            var totalBatch = (int)Math.Ceiling((double)unsynchronizedMatches.Count / BatchSize);
            for (var i = 0; i < totalBatch; i++)
            {
                var data = unsynchronizedMatches.Skip(i * BatchSize).Take(BatchSize).ToList();
                var result = await apiService.PushMatchInfoAsync(fromDate, toDate, data);
                failedMatchInfos.AddRange(result);
            }

            var failedDeletedMatches = failedMatchInfos
                .Where(x => deletedMatches.Exists(y => x.MatchId == y.MatchId && x.SubType == y.SubType)).ToList();

            var cacheMatchInfos = matchInfos.Except(failedMatchInfos, new MatchInfoApiModel())
                .Concat(failedDeletedMatches);
            var cachePolicy = new CacheItemOptions().SetAbsoluteExpiration(DateTime.Today.AddDays(2));
            await cache.SetAsync(cacheKey, JsonConvert.SerializeObject(cacheMatchInfos), cachePolicy);

        }
        finally
        {
            if (unsynchronizedMatches.Any())
            {
                await WriteLogForMonitor(unsynchronizedMatches, "SyncEventToAdminSite_unsynchronizedMatches")
                .ConfigureAwait(false);
            }

            if (failedMatchInfos.Any())
            {
                await WriteLogForMonitor(failedMatchInfos, "SyncEventToAdminSite_failedMatchInfos");
            }
        }
          
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

    private async Task WriteLogForMonitor(object request, string action)
    {
        await logger.Execute(new OwpLogCriteria
        {
            CreatedDate = DateTime.Now.AddHours(12),
            Action = action,
            Payload = JsonConvert.SerializeObject(request)
        });
    }
}