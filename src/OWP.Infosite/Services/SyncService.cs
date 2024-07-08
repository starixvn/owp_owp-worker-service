namespace OWP.Infosite.Services;

using Constants;
using Criteria;
using Fanex.Data.Repository;
using Helper;
using Models;
using Olympus.Xml;
using System;
using System.Linq;

public interface SyncService
{
    Task Sync();
}

public class SyncServiceImp
{
    private readonly IDynamicRepository repository;
    private readonly ApiService apiService;
    protected readonly DateTime MinDate = new(2000, 1, 1);

    protected DateTimeService DateTimeService { get; }

    public SyncServiceImp(
        DateTimeService dateTimeService,
        ApiService apiService,
        IDynamicRepository repository)
    {
        DateTimeService = dateTimeService;
        this.apiService = apiService;
        this.repository = repository;
    }

    public async Task Sync(DateTime fromDate)
    {
        var queriedResultTask = await QueryEvents(fromDate);
        
        var teamRoundsTask = await QueryTeamRound(fromDate);

        await BuildServiceLog(queriedResultTask);

        MapRoundToEvents(queriedResultTask, teamRoundsTask);

        await SaveEvents(queriedResultTask);
        await UpdateSoloMatch();
    }

    #region QueryTeamRound
    private async Task<Dictionary<int, string>> QueryTeamRound(DateTime fromDate)
    {
        var teamRounds = await apiService.QueryRound(fromDate, fromDate);
        if (teamRounds == null || teamRounds.content == null)
        {
            return new Dictionary<int, string>();
        }

        var roundInfo = teamRounds.content.DistinctBy(x => x.match_id).ToDictionary(
            r => r.match_id,
            r => string.IsNullOrEmpty(r.round_home_away_total) || r.round_home_away_total == "/"
                ? string.Empty
                : r.round_home_away_total);

        return roundInfo;
    }

    private static void MapRoundToEvents(IEnumerable<Event> events, Dictionary<int, string> teamRounds)
    {
        foreach (var match in events)
        {
            match.TeamRound = teamRounds.GetValueOrDefault(match.InfoSiteMatchId, string.Empty);
        }
    }
    #endregion

    private async Task SaveEvents(IEnumerable<Event> events)
    {
        var xml = XmlConverter.Serialize(events);
        var criteria = new PushInfoSiteMatchCommand
        {
            Xml = xml
        };
        await repository.ExecuteAsync(criteria).ConfigureAwait(false);
    }

    #region BuildServiceLog
    private async Task BuildServiceLog(List<Event> queriedResult)
    {
        var loggedChanges = new List<LoggedChangeModel>();

        var ids = string.Join(",", queriedResult.Select(x => x.InfoSiteMatchId));

        var existingInfoSiteEvent = await GetExistingInfoSite(ids);

        var newInfoSiteIds = queriedResult.ExceptBy(existingInfoSiteEvent.Select(x => x.InfoSiteMatchId),
            x => x.InfoSiteMatchId).Select(x=>x.InfoSiteMatchId);

        BuildLogForNewMatches(newInfoSiteIds, loggedChanges);
        BuildLogForChangedMatchesLog(queriedResult, existingInfoSiteEvent, loggedChanges);

        if (loggedChanges.Any())
        {
            await SaveLogChange(loggedChanges);
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

    private static void BuildLogForNewMatches(IEnumerable<int> newInfoSiteIds, List<LoggedChangeModel> loggedChanges)
    {
        
        var newLoggedChanges = newInfoSiteIds.Select(newInfoSiteId => new LoggedChangeModel
        {
            UpdateTypeId = 1,
            Action = Constant.UpdateType.GetValueOrDefault(1),
            InfoSiteMatchId = newInfoSiteId,
            UpDateTime = InfoSiteSyncHelper.DateTimeNowGmt8(),
            NewValue = null,
            OldValue = "Initialize"
        });

        loggedChanges.AddRange(newLoggedChanges);
    }

    private async Task<List<InfoSiteDbInfo>> GetExistingInfoSite(string ids)
    {
        var criteria = new GetExistingInfoSiteMatchesCriteria
        {
            Ids = ids
        };

        return (await repository.FetchAsync<InfoSiteDbInfo>(criteria).ConfigureAwait(false)).ToList();
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
    #endregion

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