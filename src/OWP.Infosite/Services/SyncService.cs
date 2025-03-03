namespace OWP.Infosite.Services;

using System;
using System.Linq;
using Constants;
using Criteria;
using Fanex.Data.Repository;
using Helper;
using Models;
using Olympus.OWP.Constants;
using Olympus.Xml;

public interface SyncService
{
    Task Sync();
}

public class SyncServiceImp(
    DateTimeService dateTimeService,
    ApiService apiService,
    IDynamicRepository repository)
{
    protected readonly DateTime MinDate = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private const string FromService = "Today";
    protected DateTimeService DateTimeService { get; } = dateTimeService;

    public async Task Sync(DateTime fromDate)
    {
        var priorityProviders = await GetProviderArrangement();
        var queriedResult = await QueryEvents(fromDate);
        var infoSiteDbInfos = await GetExistingInfoSite(string.Join(",", queriedResult.Select(x => x.InfoSiteMatchId)));
        UpdateProviders(queriedResult, priorityProviders, infoSiteDbInfos);

        await BuildServiceLog(queriedResult, infoSiteDbInfos);

        var teamRoundsTask = await QueryTeamRound(fromDate);
        MapRoundToEvents(queriedResult, teamRoundsTask);

        await SaveEvents(queriedResult);

        await UpdateSoloMatch();
    }

    private static void UpdateProviders(List<Event> queriedResult, List<string> priorityProviders, List<InfoSiteDbInfo> infoSiteDbInfos)
    {
        foreach (var x in queriedResult)
        {
            var currentTime = DateTime.UtcNow.AddHours(8);
            if (currentTime < x.EventDateTime.AddDays(-2))
            {
                x.Provider = ProviderHelper.BuildProviderByOtherProvider(priorityProviders, x.OtherProvider);
            }
            else
            {
                var matchingEvent = infoSiteDbInfos.FirstOrDefault(e => e.InfoSiteMatchId == x.InfoSiteMatchId);
                if (matchingEvent != null && !string.IsNullOrEmpty(matchingEvent.Provider))
                {
                    x.Provider = matchingEvent.Provider;
                }
            }
        }
    }

    #region QueryTeamRound

    private async Task<Dictionary<int, string>> QueryTeamRound(DateTime fromDate)
    {
        var teamRounds = await apiService.QueryRound(fromDate, fromDate);
        if (teamRounds?.content == null)
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

    #endregion QueryTeamRound

    private async Task SaveEvents(IEnumerable<Event> events)
    {
        var xml = XmlConverter.Serialize(events);
        var criteria = new PushInfoSiteMatchCommand
        {
            Xml = xml
        };
        await repository.ExecuteAsync(criteria);
    }

    #region BuildServiceLog

    private async Task BuildServiceLog(List<Event> queriedResult, List<InfoSiteDbInfo> infoSiteDbInfos)
    {
        var loggedChanges = new List<LoggedChangeModel>();
        var newInfoSiteIds = GetNewInfoSiteIds(queriedResult, infoSiteDbInfos);

        BuildLogForNewMatches(newInfoSiteIds, loggedChanges);
        BuildLogForChangedMatchesLog(queriedResult, infoSiteDbInfos, loggedChanges);

        if (loggedChanges.Count != 0)
        {
            await SaveLogChange(loggedChanges);
        }
    }

    private static IEnumerable<int> GetNewInfoSiteIds(List<Event> queriedResult, List<InfoSiteDbInfo> infoSiteDbInfos)
    {
        return queriedResult
            .ExceptBy(infoSiteDbInfos.Select(x => x.InfoSiteMatchId), x => x.InfoSiteMatchId)
            .Select(x => x.InfoSiteMatchId);
    }

    private static void BuildLogForChangedMatchesLog(IList<Event> queriedResult, IList<InfoSiteDbInfo> infoSiteDbInfos, List<LoggedChangeModel> loggedChanges)
    {
        foreach (var infoSiteOld in infoSiteDbInfos)
        {
            var infoSiteNew = queriedResult.First(x => x.InfoSiteMatchId == infoSiteOld.InfoSiteMatchId);
            var infoSiteClone = InfoSiteSyncHelper.CloneInfoSiteMatch(infoSiteNew);

            var type = typeof(InfoSiteDb);
            var properties = type.GetProperties()
                .Where(x => !x.Name.Equals("MatchStatus") && !x.Name.Equals("IsPostponed")).ToList();

            var loggedChangeItems = InfoSiteSyncHelper.BuildLoggedChanges(properties, infoSiteOld, infoSiteClone).ToList();

            BuildLogForMatchPostponed(infoSiteNew, infoSiteOld, loggedChangeItems);
            BuildLogForMatchCancelled(infoSiteNew, infoSiteOld, loggedChangeItems);
            BuildLogForMatchSuspended(infoSiteNew, infoSiteOld, loggedChangeItems);

            if (infoSiteOld is { IsCheck: true } && loggedChangeItems.Any())
            {
                loggedChanges.Add(new LoggedChangeModel
                {
                    InfoSiteMatchId = infoSiteClone.InfoSiteMatchId,
                    UpdateTypeId = null,
                    Action = "Update the <b>checking status</b>",
                    NewValue = "Unchecked",
                    OldValue = "Checked",
                    UpDateTime = InfoSiteSyncHelper.DateTimeNowGmt8(),
                    FromService = FromService
                });
            }

            //if (loggedChangeItems.Exists(log => log.UpdateTypeId == UpdateTypeConstant.MOVE_KICKOFF_DATE))
            //{
            //    var updateKODateLog = loggedChangeItems.First(l => l.UpdateTypeId == UpdateTypeConstant.MOVE_KICKOFF_DATE);
            //    loggedChanges.Add(new LoggedChangeModel
            //    {
            //        InfoSiteMatchId = updateKODateLog.InfoSiteMatchId,
            //        UpdateTypeId = UpdateTypeConstant.RENEW_MATCH,
            //        Action = "Renew Match",
            //        NewValue = updateKODateLog.NewValue,
            //        OldValue = updateKODateLog.OldValue,
            //        UpDateTime = InfoSiteSyncHelper.DateTimeNowGmt8(),
            //        FromService = FromService
            //    });
            //}

            loggedChanges.AddRange(loggedChangeItems);
        }
    }

    private static void BuildLogForNewMatches(IEnumerable<int> newInfoSiteIds, List<LoggedChangeModel> loggedChanges)
    {
        foreach (var newInfoSiteId in newInfoSiteIds)
        {
            AddLoggedChange(loggedChanges, newInfoSiteId, UpdateTypeConstant.NEW_MATCH, null, "Initialize");
        }
    }

    private async Task<List<InfoSiteDbInfo>> GetExistingInfoSite(string ids)
    {
        var criteria = new GetExistingInfoSiteMatchesCriteria
        {
            Ids = ids
        };

        return (await repository.FetchAsync<InfoSiteDbInfo>(criteria)).ToList();
    }

    private async Task SaveLogChange(IEnumerable<LoggedChangeModel> loggedChange)
    {
        var xml = XmlConverter.Serialize(loggedChange);
        var criteria = new SaveLogsChangeCriteria
        {
            Xml = xml
        };
        await repository.ExecuteAsync(criteria);
    }

    #endregion BuildServiceLog

    private async Task UpdateSoloMatch()
    {
        var criteria = new UpdateSoloMatchCriteria();
        await repository.ExecuteAsync(criteria);
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

    private async Task<List<string>> GetProviderArrangement()
    {
        var criteria = new GetProviderCriteria();
        var provider = (await repository.FetchAsync<ProviderInfo>(criteria))
            .OrderBy(x => x.Order)
            .Select(x => x.Value)
            .ToList();

        return provider;
    }

    private static void BuildLogForMatchPostponed(Event infoSiteNew, InfoSiteDbInfo infoSiteDb, List<LoggedChangeModel> loggedChanges)
    {
        BuildLogForMatchStatus(infoSiteNew, infoSiteDb, loggedChanges,
            "Postponed", "Postponed", InfoSiteMatchStatus.Postponed,
            UpdateTypeConstant.RENEW_MATCH, UpdateTypeConstant.MATCH_POSTPONED);
    }

    private static void BuildLogForMatchCancelled(Event infoSiteNew, InfoSiteDbInfo infoSiteDb, List<LoggedChangeModel> loggedChanges)
    {
        BuildLogForMatchStatus(infoSiteNew, infoSiteDb, loggedChanges,
            "Cancel", "Cancelled", InfoSiteMatchStatus.Cancelled,
            UpdateTypeConstant.RENEW_MATCH, UpdateTypeConstant.MATCH_CANCELLED);
    }

    private static void BuildLogForMatchSuspended(Event infoSiteNew, InfoSiteDbInfo infoSiteDb, List<LoggedChangeModel> loggedChanges)
    {
        BuildLogForMatchStatus(infoSiteNew, infoSiteDb, loggedChanges,
            "", "Suspended", InfoSiteMatchStatus.Suspended,
            UpdateTypeConstant.RENEW_MATCH, UpdateTypeConstant.MATCH_SUSPENDED);
    }

    private static void BuildLogForMatchStatus(
        Event infoSiteNew,
        InfoSiteDbInfo infoSiteDb,
        List<LoggedChangeModel> loggedChanges,
        string statusString,
        string loggedStatus,
        byte matchStatus,
        int updateTypeRenew,
        int updateTypeChanged)
    {
        if (infoSiteDb.IsMatchHasStatus(statusString, matchStatus)
            && !infoSiteNew.IsMatchHasStatus(statusString, matchStatus))
        {
            AddLoggedChange(loggedChanges, infoSiteDb.InfoSiteMatchId, updateTypeRenew, string.Empty, loggedStatus);
        }

        if (!infoSiteDb.IsMatchHasStatus(statusString, matchStatus)
            && infoSiteNew.IsMatchHasStatus(statusString, matchStatus))
        {
            AddLoggedChange(loggedChanges, infoSiteDb.InfoSiteMatchId, updateTypeChanged, loggedStatus, string.Empty);
        }
    }

    private static void AddLoggedChange(List<LoggedChangeModel> loggedChanges, int matchId, int updateTypeId, string newValue, string oldValue)
    {
        loggedChanges.Add(new LoggedChangeModel
        {
            InfoSiteMatchId = matchId,
            UpdateTypeId = updateTypeId,
            Action = UpdateTypeConstant.UpdateType.GetValueOrDefault(updateTypeId),
            NewValue = newValue,
            OldValue = oldValue,
            UpDateTime = InfoSiteSyncHelper.DateTimeNowGmt8(),
            FromService = FromService
        });
    }
}