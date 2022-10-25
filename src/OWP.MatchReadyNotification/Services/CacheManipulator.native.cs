using Fanex.Caching;
using OWP.MatchReadyNotification.Constants;

namespace OWP.MatchReadyNotification.Services;

public interface CacheManipulator
{
    Task<string> GetLastTimeNotReadyNotification();

    Task SaveLastTimeNotification(string lastTimeNotification);
}

public class CacheManipulatorImp : CacheManipulator
{
    private readonly ICache cache;

    public CacheManipulatorImp(ICache cache)
    {
        this.cache = cache;
    }

    public async Task SaveLastTimeNotification(string lastTimeNotification)
    {
        var cachePolicy = new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(Constant.CacheExpiredInDays));
        const string cacheKey = $"{Constant.KeyLastTimeNotification}";
        await cache.SetAsync(cacheKey, lastTimeNotification, cachePolicy);
    }

    public async Task<string> GetLastTimeNotReadyNotification()
    {
        const string cacheKey = $"{Constant.KeyLastTimeNotification}";
        var cacheResult = await cache.TryGetAsync<string>(cacheKey);

        return cacheResult.Item2 ?? string.Empty;
    }
}