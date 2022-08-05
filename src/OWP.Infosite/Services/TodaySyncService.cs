using Fanex.Caching;
using Fanex.Data.Repository;
using Newtonsoft.Json;
using OWP.Core.RabbitMq;
using OWP.Core.RabbitMq.Models;
using OWP.Infosite.Constants;
using OWP.Infosite.Criteria;
using OWP.Infosite.Models;
using System.Globalization;

namespace OWP.Infosite.Services;

public interface TodaySyncService : SyncService
{
}

public class TodaySyncServiceImp : SyncServiceImp, TodaySyncService
{
    private readonly ICache cacheService;
    private readonly IDynamicRepository repository;
    private readonly UpdateInfositeIdService updateInfositeIdService;
    private readonly RabbitMqClient rabbitMqClient;

    public TodaySyncServiceImp(
        DateTimeService dateTimeService,
        RabbitMqClient rabbitMqClient,
        ApiService apiService,
        IDynamicRepository repository,
        ICache cacheService,
        UpdateInfositeIdService updateInfositeIdService)
        : base(dateTimeService, apiService, repository)
    {
        this.updateInfositeIdService = updateInfositeIdService;
        this.cacheService = cacheService;
        this.rabbitMqClient = rabbitMqClient;
        this.repository = repository;
    }

    public async Task Sync()
    {
        var today = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Unspecified);

        var sessionId = Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture);
        var infoSiteMatches = await GetOldInfoSiteMatches(today);
        await SaveOldInfoSiteMatchSnapshot(sessionId, infoSiteMatches);

        await Sync(today);
        await Sync(today.AddDays(1));
        await updateInfositeIdService.UpdateInfositeId(today);
        await updateInfositeIdService.UpdateInfositeId(today.AddDays(1));

        PublishMatchInfoNotification(sessionId, today, today.AddDays(1));
    }

    private async Task<IEnumerable<InfoSiteMatch>> GetOldInfoSiteMatches(DateTime fromDate)
    {
        var criteria = new GetInfoSiteMatchesByDateRangeCriteria
        {
            FromDate = fromDate.Date.AddHours(12),
            ToDate = fromDate.Date.AddDays(1).AddHours(11).AddMinutes(59)
        };
        var result = await repository.FetchAsync<InfoSiteMatch>(criteria).ConfigureAwait(false);

        return result;
    }

    private async Task SaveOldInfoSiteMatchSnapshot(string sessionId, IEnumerable<InfoSiteMatch> infoSiteMatches)
    {
        var absoluteTimeSpan = TimeSpan.FromMinutes(Constant.CacheExpiredIn10Minutes);
        var cachePolicy = new CacheItemOptions().SetAbsoluteExpiration(absoluteTimeSpan);
        var cacheKey = $"{Constant.OldInfositeMatchesSnapshotKey}-{sessionId}";

        var assignmentsJson = JsonConvert.SerializeObject(infoSiteMatches);

        await cacheService.SetAsync(cacheKey, assignmentsJson, cachePolicy);
    }

    public void PublishMatchInfoNotification(string sessionId, DateTime fromDate, DateTime toDate)
    {
        var request = new InfoSiteMatchInfoNotifyRequest
        {
            SessionId = sessionId,
            FromDate = fromDate,
            ToDate = toDate
        };

        rabbitMqClient.PublishRequestMessage(request);
    }
}