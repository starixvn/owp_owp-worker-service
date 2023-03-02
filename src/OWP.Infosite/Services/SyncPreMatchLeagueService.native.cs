namespace OWP.Infosite.Services;

using System.Globalization;
using Constants;
using Criteria;
using Fanex.Caching;
using Fanex.Data.Repository;
using Models;
using Newtonsoft.Json;
using RabbitMq;
using RabbitMq.Models;

public interface SyncPreMatchLeagueService
{
    Task Sync();
}

public class SyncPreMatchLeagueServiceImp : SyncPreMatchLeagueService
{
    private readonly DateTimeService dateTimeService;

    private readonly IDynamicRepository dynamicRepository;

    private readonly ICache cacheService;

    private readonly RabbitMqClient rabbitMqClient;

    public SyncPreMatchLeagueServiceImp(
        DateTimeService dateTimeService,
        IDynamicRepository dynamicRepository,
        ICache cacheService,
        RabbitMqClient rabbitMqClient)
    {
        this.dateTimeService = dateTimeService;
        this.dynamicRepository = dynamicRepository;
        this.cacheService = cacheService;
        this.rabbitMqClient = rabbitMqClient;
    }

    public async Task Sync()
    {
        var sessionId = Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture);
        var preMatchLeagues = await GetOldPreMatchLeagues(dateTimeService.Today);
        await SaveOldPreMatchLeaguesSnapshot(sessionId, preMatchLeagues);

        var criteria = new SyncInfoSiteMatchToPreMatchLeagueCriteria
        {
            FromDate = dateTimeService.Today,
            ToDate = dateTimeService.Today.AddYears(10)
        };

        await dynamicRepository.ExecuteAsync(criteria);

        PublishNewLeagueNotification(sessionId, dateTimeService.Today);
    }

    private async Task<IEnumerable<PreMatchLeague>> GetOldPreMatchLeagues(DateTime fromDate)
    {
        var criteria = new GetPreMatchLeaguesCriteria
        {
            FromDate = fromDate
        };

        return await dynamicRepository.FetchAsync<PreMatchLeague>(criteria);
    }

    private async Task SaveOldPreMatchLeaguesSnapshot(string sessionId, IEnumerable<PreMatchLeague> preMatchLeagues)
    {
        var cachePolicy = new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
        var cacheKey = $"{Constant.OldPreMatchLeaguesSnapshotKey}-{sessionId}";
        var leaguesJson = JsonConvert.SerializeObject(preMatchLeagues);

        await cacheService.SetAsync(cacheKey, leaguesJson, cachePolicy);
    }

    public void PublishNewLeagueNotification(string sessionId, DateTime fromDate)
    {
        var request = new PrematchNewLeagueNotifyRequest
        {
            SessionId = sessionId,
            FromDate = fromDate
        };

        rabbitMqClient.PublishRequestMessage(request);
    }
}