using Fanex.Caching;
using OWP.LinkLeagueAndTeam.Constants;

namespace OWP.LinkLeagueAndTeam.Services;

public interface CacheManipulator
{
    Task<DateTime> GetStartTimeFromCache();

    Task SaveStartTimeQueryTeamInfo(DateTime startTime);
}

public class CacheManipulatorImp : CacheManipulator
{
    private readonly ICache cache;

    public CacheManipulatorImp(ICache cache)
    {
        this.cache = cache;
    }

    public async Task SaveStartTimeQueryTeamInfo(DateTime startTime)
    {
        var cachePolicy = new CacheItemOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(Constant.CacheExpiredInDays));
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