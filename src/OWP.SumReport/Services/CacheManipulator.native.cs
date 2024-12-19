namespace OWP.SumReport.Services;

using Constants;
using Fanex.Caching;

public interface CacheManipulator
{
    Task<(bool, DateTime)> GetLastDateForRegenerateData(string cacheKey);

    Task RemoveLastDateOfSumData(string cacheKey);
}

public class CacheManipulatorImp(ICache cache) : CacheManipulator
{
    public async Task<(bool, DateTime)> GetLastDateForRegenerateData(string cacheKey)
    {
        var (found, value) = await cache.TryGetAsync<string>(cacheKey);
        var dateString = value ?? string.Empty;

        var isDate = DateTime.TryParse(dateString, out var date);

        return (found, isDate ? date : DateTime.Today);
    }

    public async Task RemoveLastDateOfSumData(string cacheKey)
    {
        await cache.RemoveAsync(cacheKey);
    }

    public async Task SaveLastDateOfSumData(string cacheKey, DateTime date)
    {
        var cachePolicy = new CacheItemOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(Constant.CacheExpiredInDays));

        await cache.SetAsync(cacheKey, date.ToShortDateString(), cachePolicy);
    }
}