using Fanex.Caching;
using OWP.SumReportData.Constants;

namespace OWP.SumReportData.Services;

public interface CacheManipulator
{
    Task<DateTime> GetLastDateOfSumData(string cacheKey);

    Task SaveLastDateOfSumData(string cacheKey, DateTime date);
}

public class CacheManipulatorImp : CacheManipulator
{
    private readonly ICache cache;

    public CacheManipulatorImp(ICache cache)
    {
        this.cache = cache;
    }

    public async Task<DateTime> GetLastDateOfSumData(string cacheKey)
    {
        var result = await cache.TryGetAsync<String>(cacheKey);
        var dateString = result.Item2 ?? string.Empty;

        bool isDate = DateTime.TryParse(dateString, out var date);

        return isDate ? date : DateTime.Today.AddDays(-2);
    }

    public async Task SaveLastDateOfSumData(string cacheKey, DateTime date)
    {
        var cachePolicy = new CacheItemOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(Constant.CacheExpiredInDays));

        await cache.SetAsync(cacheKey, date.ToShortDateString(), cachePolicy);
    }
}