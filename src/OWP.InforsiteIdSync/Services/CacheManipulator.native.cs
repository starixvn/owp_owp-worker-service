namespace OWP.InfositeIdSync.Services;

using Constants;
using Fanex.Caching;
using Models;
using Newtonsoft.Json;

public interface CacheManipulator
{
    Task<List<MatchKickoffDate>> GetDeletedMatches();

    Task SaveDeletedMatches(List<ValueTuple<int, int?>> synchornizedMatches);

    Task SaveOldAssignmentSnapshot(string sessionId, AssignmentSnapshot assignmentSnapshot);
}

internal class CacheManipulatorImp : CacheManipulator
{
    private readonly ICache cache;

    public CacheManipulatorImp(ICache cache)
    {
        this.cache = cache;
    }

    public async Task<List<MatchKickoffDate>> GetDeletedMatches()
    {
        var cacheResult = await cache.TryGetAsync<List<MatchKickoffDate>>(CacheKey.KeyDeletedInfoSiteMatchId);
        var matches = cacheResult.Item2 ?? new List<MatchKickoffDate>();
        return matches.Where(m => m.MatchId > 0).ToList();
    }

    public async Task SaveDeletedMatches(List<ValueTuple<int, int?>> synchornizedMatches)
    {
        var expiredDate = DateTime.Today;
        var cacheResult = await cache.TryGetAsync<List<MatchKickoffDate>>(CacheKey.KeyDeletedInfoSiteMatchId);
        var currentMatches = cacheResult.Item2 ?? new List<MatchKickoffDate>();
        var remainMatches = currentMatches
            .Where(m => m.KickoffDate >= expiredDate)
            .Where(m => !synchornizedMatches.Select(x => (x.Item1, x.Item2)).Contains((m.MatchId, m.MinorBettypeId)))
            .ToList();
        var cachePolicy = new CacheItemOptions().SetAbsoluteExpiration(DateTime.Today.AddYears(10));

        await cache.SetAsync(CacheKey.KeyDeletedInfoSiteMatchId, remainMatches, cachePolicy);
    }

    public async Task SaveOldAssignmentSnapshot(string sessionId, AssignmentSnapshot assignmentSnapshot)
    {
        var cachePolicy = new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheKey.CacheExpiredIn5Minutes));
        var assignmentSnapshotInJson = JsonConvert.SerializeObject(assignmentSnapshot);
        var cacheKey = $"{CacheKey.KeyAssignmentSnapshot}-{sessionId}";

        await cache.SetAsync(cacheKey, assignmentSnapshotInJson, cachePolicy);
    }
}