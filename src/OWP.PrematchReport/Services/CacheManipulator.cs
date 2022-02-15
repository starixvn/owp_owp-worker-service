using Fanex.Caching;

namespace OWP.PrematchReport.Services
{
  public interface CacheManipulator
  {
    Task<DateTime> GetStartTimeFromCache();

    Task SaveStartTime(DateTime startTime);
  }

  public class CacheManipulatorImp : CacheManipulator
  {
    private readonly ICache cache;

    public CacheManipulatorImp(ICache cache)
    {
      this.cache = cache;
    }

    public async Task SaveStartTime(DateTime startTime)
    {
      var cachePolicy = new CacheItemOptions()
          .SetAbsoluteExpiration(TimeSpan.FromMinutes(Constants.Constant.CACHE_EXPIRED_IN_DAYS));
      await cache.SetAsync(Constants.Constant.PRE_MATCH_REPORT_KEY, startTime.ToShortDateString(), cachePolicy);
    }

    public async Task<DateTime> GetStartTimeFromCache()
    {
      var cacheResult = await cache.TryGetAsync<string>(Constants.Constant.PRE_MATCH_REPORT_KEY);

      return DateTime.TryParse(cacheResult.Item2, out var value)
      ? value
      : Constants.Constant.START_TIME_DEFAULT;
    }
  }
}