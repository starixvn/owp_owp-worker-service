namespace OWP.Infosite.Services;

using System.Globalization;
using Constants;
using Criteria;
using Fanex.Caching;
using Fanex.Data.Repository;
using Models;
using Newtonsoft.Json;
using Olympus.Xml;
using RabbitMq;
using RabbitMq.Models;

public interface SyncService
{
    Task Sync();
}

public class SyncServiceImp
{
    private readonly IDynamicRepository repository;
    private readonly ApiService apiService;
    private readonly RabbitMqClient rabbitMqClient;
    private readonly ICache cacheService;

    protected readonly DateTime MinDate = new(2000, 1, 1);
    protected DateTimeService DateTimeService { get; }

    public SyncServiceImp(
        DateTimeService dateTimeService,
        ApiService apiService,
        RabbitMqClient rabbitMqClient,
        ICache cacheService,
        IDynamicRepository repository)
    {
        DateTimeService = dateTimeService;
        this.apiService = apiService;
        this.repository = repository;
        this.cacheService = cacheService;
        this.rabbitMqClient = rabbitMqClient;
    }

    public async Task Sync(DateTime fromDate, bool matchInfoNotification = false)
    {
        var queriedResult = await QueryEvents(fromDate);
        if (matchInfoNotification)
        {
            var today = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Unspecified);
            var sessionId = Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture);
            await SaveOldInfoSiteMatchSnapshot(sessionId, today);

            await SaveEvents(queriedResult);
            await UpdateSoloMatch();

            PublishMatchInfoNotification(sessionId, today, today.AddDays(1));
        }
        else
        {
            await SaveEvents(queriedResult);
            await UpdateSoloMatch();
        }
    }

    private async Task SaveEvents(IEnumerable<Event> events)
    {
        var xml = XmlConverter.Serialize(events);
        var criteria = new PushInfoSiteMatchCommand
        {
            Xml = xml
        };
        await repository.ExecuteAsync(criteria).ConfigureAwait(false);
    }

    private async Task UpdateSoloMatch()
    {
        var criteria = new UpdateSoloMatchCriteria();
        await repository.ExecuteAsync(criteria).ConfigureAwait(false);
    }

    protected async Task<List<Event>> QueryEvents(DateTime date)
    {
        var param = new ParamsQueryModel
        {
            FromDate = date,
            ToDate = date,
            LastUpdatedTime = MinDate
        };

        var commonMatches = await apiService.QueryAsync(param);
        var queriedEvents = ToEvents(commonMatches);
        var validEvents = GetValidEvents(queriedEvents);

        return validEvents.ToList();
    }

    private static IEnumerable<Event> GetValidEvents(IEnumerable<Event> queriedEvents)
    {
        return queriedEvents.Where(evt => evt.IsValid);
    }

    private static IEnumerable<Event> ToEvents(IEnumerable<CommonMatchModel> commonMatches)
    {
        return commonMatches.Select(evt => new Event(evt));
    }

    private async Task<IEnumerable<InfoSiteMatch>> GetOldInfoSiteMatches(DateTime today)
    {
        var criteria = new GetInfoSiteMatchesByDateRangeCriteria
        {
            FromDate = today.Date.AddHours(12),
            ToDate = today.Date.AddDays(1).AddHours(11).AddMinutes(59)
        };
        var result = await repository.FetchAsync<InfoSiteMatch>(criteria).ConfigureAwait(false);

        return result;
    }

    private async Task SaveOldInfoSiteMatchSnapshot(string sessionId, DateTime today)
    {
        var oldInfoSiteMatches = await GetOldInfoSiteMatches(today);
        var absoluteTimeSpan = TimeSpan.FromMinutes(Constant.CacheExpiredIn10Minutes);
        var cachePolicy = new CacheItemOptions().SetAbsoluteExpiration(absoluteTimeSpan);
        var cacheKey = $"{Constant.OldInfositeMatchesSnapshotKey}-{sessionId}";

        var assignmentsJson = JsonConvert.SerializeObject(oldInfoSiteMatches);

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