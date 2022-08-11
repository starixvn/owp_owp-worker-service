using Fanex.Caching;
using OWP.SumReportData.Constants;

namespace OWP.SumReportData.Services;

public interface CacheManipulator
{
    Task<(bool, DateTime)> GetLastDateForRegenerateData(string cacheKey);

    //Task SaveLastDateOfSumData(string cacheKey, DateTime date);

    Task RemoveLastDateOfSumData(string cacheKey);
}

public class CacheManipulatorImp : CacheManipulator
{
    private readonly ICache cache;

    public CacheManipulatorImp(ICache cache)
    {
        this.cache = cache;
    }

    public async Task<(bool, DateTime)> GetLastDateForRegenerateData(string cacheKey)
    {
        var result = await cache.TryGetAsync<string>(cacheKey);
        var isGenerateData = result.Item1;
        var dateString = result.Item2 ?? string.Empty;

        bool isDate = DateTime.TryParse(dateString, out var date);

        return (isGenerateData, isDate ? date : DateTime.Today);
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