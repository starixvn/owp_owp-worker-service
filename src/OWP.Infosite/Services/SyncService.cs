namespace OWP.Infosite.Services;

using Constants;
using Criteria;
using Fanex.Data.Repository;
using Helper;
using Models;
using Olympus.OWP.Constants;
using Olympus.Xml;
using System;
using System.Linq;

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
        var queriedResultTask = await QueryEvents(fromDate);
        var priorityProviders = await GetProviderArrangement();

        foreach (var x in queriedResultTask)
        {
            x.Provider = ProviderHelper.BuildProviderByOtherProvider(priorityProviders, x.OtherProvider);
        }

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

    private async Task BuildServiceLog(List<Event> queriedResult)
    {
        var loggedChanges = new List<LoggedChangeModel>();

        var ids = string.Join(",", queriedResult.Select(x => x.InfoSiteMatchId));

        var existingInfoSiteEvent = await GetExistingInfoSite(ids);

        var newInfoSiteIds = queriedResult.ExceptBy(existingInfoSiteEvent.Select(x => x.InfoSiteMatchId),
            x => x.InfoSiteMatchId).Select(x => x.InfoSiteMatchId);

        BuildLogForNewMatches(newInfoSiteIds, loggedChanges);
        BuildLogForChangedMatchesLog(queriedResult, existingInfoSiteEvent, loggedChanges);

        if (loggedChanges.Count != 0)
        {
            await SaveLogChange(loggedChanges);
        }
    }

    private static void BuildLogForChangedMatchesLog(IList<Event> queriedResult, IList<InfoSiteDbInfo> existedEvents, List<LoggedChangeModel> loggedChanges)
    {
        foreach (var infoSiteOld in existedEvents)
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

            if (loggedChangeItems.Exists(log => log.UpdateTypeId == UpdateTypeConstant.MOVE_KICKOFF_DATE))
            {
                var updateKODateLog = loggedChangeItems.First(l => l.UpdateTypeId == UpdateTypeConstant.MOVE_KICKOFF_DATE);
                loggedChanges.Add(new LoggedChangeModel
                {
                    InfoSiteMatchId = updateKODateLog.InfoSiteMatchId,
                    UpdateTypeId = UpdateTypeConstant.RENEW_MATCH,
                    Action = "Renew match",
                    NewValue = updateKODateLog.NewValue,
                    OldValue = updateKODateLog.OldValue,
                    UpDateTime = InfoSiteSyncHelper.DateTimeNowGmt8(),
                    FromService = FromService
                });
            }

            loggedChanges.AddRange(loggedChangeItems);
        }
    }

    private static void BuildLogForNewMatches(IEnumerable<int> newInfoSiteIds, List<LoggedChangeModel> loggedChanges)
    {
        var newLoggedChanges = newInfoSiteIds.Select(newInfoSiteId => new LoggedChangeModel
        {
            UpdateTypeId = UpdateTypeConstant.NEW_MATCH,
            Action = Constant.UpdateType.GetValueOrDefault(UpdateTypeConstant.NEW_MATCH),
            InfoSiteMatchId = newInfoSiteId,
            UpDateTime = InfoSiteSyncHelper.DateTimeNowGmt8(),
            NewValue = null,
            OldValue = "Initialize",
            FromService = FromService
        });

        loggedChanges.AddRange(newLoggedChanges);
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
        if ((!string.IsNullOrEmpty(infoSiteDb.Remark2) && infoSiteDb.Remark2.Contains("Postponed", StringComparison.OrdinalIgnoreCase))
            || infoSiteDb.IsPostponed)
        {
            return;
        }

        if ((!string.IsNullOrEmpty(infoSiteNew.Remark2) && infoSiteNew.Remark2.Contains("Postponed", StringComparison.OrdinalIgnoreCase))
            || infoSiteNew.IsPostponed)
        {
            loggedChanges.Add(new LoggedChangeModel
            {
                InfoSiteMatchId = infoSiteDb.InfoSiteMatchId,
                UpdateTypeId = UpdateTypeConstant.MATCH_POSTPONED,
                Action = Constant.UpdateType.GetValueOrDefault(UpdateTypeConstant.MATCH_POSTPONED),
                NewValue = "Postponed",
                OldValue = string.Empty,
                UpDateTime = InfoSiteSyncHelper.DateTimeNowGmt8(),
                FromService = FromService
            });
        }
    }

    private static void BuildLogForMatchCancelled(Event infoSiteNew, InfoSiteDbInfo infoSiteDb, List<LoggedChangeModel> loggedChanges)
    {
        if ((!string.IsNullOrEmpty(infoSiteDb.Remark2) && infoSiteDb.Remark2.Contains("Cancel", StringComparison.OrdinalIgnoreCase))
            || infoSiteDb.MatchStatus == InfoSiteMatchStatus.Cancelled)
        {
            return;
        }

        if ((!string.IsNullOrEmpty(infoSiteNew.Remark2) && infoSiteNew.Remark2.Contains("Cancel", StringComparison.OrdinalIgnoreCase))
            || infoSiteNew.MatchStatus == InfoSiteMatchStatus.Cancelled)
        {
            loggedChanges.Add(new LoggedChangeModel
            {
                InfoSiteMatchId = infoSiteDb.InfoSiteMatchId,
                UpdateTypeId = UpdateTypeConstant.MATCH_CANCELLED,
                Action = Constant.UpdateType.GetValueOrDefault( UpdateTypeConstant.MATCH_CANCELLED),
                NewValue = "Cancelled",
                OldValue = string.Empty,
                UpDateTime = InfoSiteSyncHelper.DateTimeNowGmt8(),
                FromService = FromService
            });
        }
    }

    private static void BuildLogForMatchSuspended(Event infoSiteNew, InfoSiteDbInfo infoSiteDb, List<LoggedChangeModel> loggedChanges)
    {
        if (infoSiteDb.MatchStatus == InfoSiteMatchStatus.Suspended)
        {
            return;
        }

        if (infoSiteNew.MatchStatus == InfoSiteMatchStatus.Suspended)
        {
            loggedChanges.Add(new LoggedChangeModel
            {
                InfoSiteMatchId = infoSiteDb.InfoSiteMatchId,
                UpdateTypeId = UpdateTypeConstant.MATCH_SUSPENDED,
                Action = Constant.UpdateType.GetValueOrDefault(UpdateTypeConstant.MATCH_SUSPENDED),
                NewValue = "Suspended",
                OldValue = string.Empty,
                UpDateTime = InfoSiteSyncHelper.DateTimeNowGmt8(),
                FromService = FromService
            });
        }
    }
}