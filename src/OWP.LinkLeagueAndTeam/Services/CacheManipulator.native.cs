namespace OWP.LinkLeagueAndTeam.Services;

using Constants;
using Fanex.Caching;

public interface CacheManipulator
{
    Task<DateTime> GetStartTimeFromCache();

    Task SaveStartTimeQueryTeamInfo(DateTime startTime);
}

public class CacheManipulatorImp(ICache cache) : CacheManipulator
{
    public async Task SaveStartTimeQueryTeamInfo(DateTime startTime)
    {
        await cache.RemoveAsync(Constant.KeyLinkLeagueTeam);

        var cachePolicy = new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromDays(Constant.CacheExpiredInDays));
        await cache.SetAsync(Constant.KeyLinkLeagueTeam, startTime.ToShortDateString(), cachePolicy);
    }

    public async Task<DateTime> GetStartTimeFromCache()
    {
        var cacheResult = await cache.TryGetAsync<string>(Constant.KeyLinkLeagueTeam);

        return DateTime.TryParse(cacheResult.Item2, out var value)
        ? value
        : Constant.StartTimeDefault;
    }
}