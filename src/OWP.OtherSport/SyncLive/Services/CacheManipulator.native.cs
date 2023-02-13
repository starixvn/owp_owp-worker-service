namespace OWP.OtherSport.SyncLive.Services;

using Fanex.Caching;
using Newtonsoft.Json;
using SyncAdminSiteId.Models;

public interface CacheManipulator
{
    Task<List<Match>> GetDeletedMatches();

    Task RemoveDeletedMatches();

    Task SaveMathIdsDuplicatedSnapshot(IEnumerable<int> matchIds);
}

internal class CacheManipulatorImp : CacheManipulator
{
    private readonly ICache cache;

    private const string DeletedMatchKey = "OS-DELETED-MATCH";

    private const string KeyOsSyncadminsiteid = "OWP-OS-MatchesHaveTheSameAdminSiteId";

    private const int AssignmentCacheExpiredInMinute = 3;

    public CacheManipulatorImp(ICache cache)
    {
        this.cache = cache;
    }

    public async Task<List<Match>> GetDeletedMatches()
    {
        var cacheResult = await cache.TryGetAsync<string>(DeletedMatchKey);
        var matches = !string.IsNullOrEmpty(cacheResult.Item2)
            ? JsonConvert.DeserializeObject<List<Match>>(cacheResult.Item2)
            : new List<Match>();

        matches ??= new List<Match>();

        return matches.Where(m => m.OsMatchId > 0).ToList();
    }
    public async Task SaveMathIdsDuplicatedSnapshot(IEnumerable<int> matchIds)
    {
        var absoluteTimeSpan = TimeSpan.FromMinutes(AssignmentCacheExpiredInMinute);
        var cachePolicy = new CacheItemOptions().SetAbsoluteExpiration(absoluteTimeSpan);
        const string cacheKey = $"{KeyOsSyncadminsiteid}";

        var assignmentsJson = JsonConvert.SerializeObject(matchIds);
        await cache.SetAsync(cacheKey, assignmentsJson, cachePolicy);
    }
    public async Task RemoveDeletedMatches()
    {
        await cache.RemoveAsync(DeletedMatchKey);
    }
}