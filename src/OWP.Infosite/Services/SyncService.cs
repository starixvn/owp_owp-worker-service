namespace OWP.Infosite.Services;

using Constants;
using Criteria;
using Fanex.Data.Repository;
using Helper;
using Models;
using Olympus.Xml;
using RabbitMq;
using System;
using System.Globalization;
using System.Reflection;

public interface SyncService
{
    Task Sync();
}

public class SyncServiceImp
{
    private readonly IDynamicRepository repository;
    private readonly ApiService apiService;
    private readonly RabbitMqClient rabbitMqClient;

    protected readonly DateTime MinDate = new(2000, 1, 1);

    protected DateTimeService DateTimeService { get; }

    public SyncServiceImp(
        DateTimeService dateTimeService,
        ApiService apiService,
        RabbitMqClient rabbitMqClient,
        IDynamicRepository repository)
    {
        DateTimeService = dateTimeService;
        this.apiService = apiService;
        this.repository = repository;
        this.rabbitMqClient = rabbitMqClient;
    }

    public async Task Sync(DateTime fromDate, bool matchInfoNotification = false)
    {

        var queriedResult = await QueryEvents(fromDate);

        var infoSiteEvents = await GetInfoSiteByDate(fromDate);

        var loggedChanges = new List<LoggedChangeModel>();

        BuildLogForNewMatches(queriedResult, infoSiteEvents, loggedChanges);
        BuildLogForChangedMatchesLog(queriedResult, infoSiteEvents, loggedChanges);

        if (loggedChanges.Any())
        {
            await SaveLogChange(loggedChanges);
        }
        if (matchInfoNotification)
        {
            var today = DateTime.Today;
            var sessionId = Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture);

            await SaveEvents(queriedResult);
            await UpdateSoloMatch();
        }
        else
        {
            await SaveEvents(queriedResult);
            await UpdateSoloMatch();
        }
    }

    private static void BuildLogForChangedMatchesLog(IEnumerable<Event> queriedResult, IList<InfoSiteDbInfo> infoSiteEvents, List<LoggedChangeModel> loggedChanges)
    {
        var existedEvents =
            queriedResult.IntersectBy(infoSiteEvents.Select(x => x.InfoSiteMatchId), x => x.InfoSiteMatchId).ToList();

        foreach (var item in existedEvents)
        {
            var infoSiteOld = infoSiteEvents.FirstOrDefault(x => x.InfoSiteMatchId == item.InfoSiteMatchId);
            var infoSiteNew = InfoSiteSyncHelper.CloneInfoSiteMatch(item);

            var type = typeof(InfoSiteDb);
            var properties = type.GetProperties();

            var loggedChangeItem = InfoSiteSyncHelper.BuildLoggedChanges(properties, infoSiteOld, infoSiteNew).ToList();
            if (infoSiteOld is { IsCheck: true } && loggedChangeItem.Any())
            {
                loggedChanges.Add(new LoggedChangeModel
                {
                    InfoSiteMatchId = infoSiteNew.InfoSiteMatchId,
                    UpdateTypeId = null,
                    Action = "Update the <b>checking status</b>",
                    NewValue = "Unchecked",
                    OldValue = "Checked",
                    UpDateTime = InfoSiteSyncHelper.DateTimeNowGmt8()
                });
            }
            loggedChanges.AddRange(loggedChangeItem);
        }
    }

    private static void BuildLogForNewMatches(IEnumerable<Event> queriedResult, IEnumerable<InfoSiteDb> infoSiteEvents, List<LoggedChangeModel> loggedChanges)
    {
        var newEvents = queriedResult.ExceptBy(infoSiteEvents.Select(x => x.InfoSiteMatchId), x => x.InfoSiteMatchId);
        var newLoggedChanges = newEvents.Select(newEvent => new LoggedChangeModel
        {
            UpdateTypeId = 1,
            Action = Constant.UpdateType.GetValueOrDefault(1),
            InfoSiteMatchId = newEvent.InfoSiteMatchId,
            UpDateTime = InfoSiteSyncHelper.DateTimeNowGmt8(),
            NewValue = null,
            OldValue = "Initialize"
        });

        loggedChanges.AddRange(newLoggedChanges);
    }

    private async Task<List<InfoSiteDbInfo>> GetInfoSiteByDate(DateTime date)
    {
        var criteria = new GetInfoSiteMatchesByDateCriteria
        {
            EventDate = date
        };

        return (await repository.FetchAsync<InfoSiteDbInfo>(criteria).ConfigureAwait(false)).ToList();
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

    private async Task SaveLogChange(IEnumerable<LoggedChangeModel> loggedChange)
    {
        var xml = XmlConverter.Serialize(loggedChange);
        var criteria = new SaveLogsChangeCriteria
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
}