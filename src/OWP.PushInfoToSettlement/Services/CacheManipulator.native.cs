namespace OWP.PushInfoToSettlement.Services;

using Fanex.Caching;
using Newtonsoft.Json;
using OWP.PushInfoToSettlement.Model;

public interface CacheManipulator
{
    Task SaveMatchSnapshot(IEnumerable<EventData> matches, DateTime eventDate);

    Task<List<EventData>> GetMatchesByDate(DateTime eventDate);

    Task<DateTime?> GetLastProcessedDate();

    Task SaveLastProcessedDate(DateTime date);

}
public class CacheManipulatorImp(ICache cache) : CacheManipulator
{
    private const string KeyMatchSendToSettlement = "Match-OldSnapshot-SendToSettlement";
    private const string KeyLastProcessedDate = "LastProcessedDate-SendToSettlement";

    private const int AssignmentCacheExpiredInMinute = 5;

    public async Task SaveMatchSnapshot(IEnumerable<EventData> matches, DateTime eventDate)
    {
        var absoluteTimeSpan = TimeSpan.FromDays(AssignmentCacheExpiredInMinute);
        var cachePolicy = new CacheItemOptions().SetAbsoluteExpiration(absoluteTimeSpan);
        var cacheKey = $"{KeyMatchSendToSettlement}-{eventDate:d}";

        var matchesJson = JsonConvert.SerializeObject(matches);
        await cache.SetAsync(cacheKey, matchesJson, cachePolicy);
    }

    public async Task<List<EventData>> GetMatchesByDate(DateTime eventDate)
    {
        var cacheKey = $"{KeyMatchSendToSettlement}-{eventDate:d}";
        var (_, value) = await cache.TryGetAsync<string>(cacheKey);

        return string.IsNullOrWhiteSpace(value) ? [] : JsonConvert.DeserializeObject<List<EventData>>(value) ?? [];
    }

    public async Task<DateTime?> GetLastProcessedDate()
    {
        var (_, value) = await cache.TryGetAsync<string>(KeyLastProcessedDate);
        return string.IsNullOrWhiteSpace(value) ? null : DateTime.Parse(value);
    }

    public async Task SaveLastProcessedDate(DateTime date)
    {
        var absoluteTimeSpan = TimeSpan.FromDays(30);
        var cachePolicy = new CacheItemOptions().SetAbsoluteExpiration(absoluteTimeSpan);
        await cache.SetAsync(KeyLastProcessedDate, date.ToString("yyyy-MM-dd"), cachePolicy);
    }
}