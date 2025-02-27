using Olympus.OWP.Constants;
using OWP.Infosite.Constants;

namespace OWP.Infosite.Services;

using Criteria;
using Fanex.Data.Repository;
using Helper;
using Models;
using Olympus.Xml;

public interface Sync180DayInfositeMatchService
{
    Task Sync();
}

public class Sync180DayInfositeMatchServiceImp(ApiService apiService, IDynamicRepository repository)
    : Sync180DayInfositeMatchService
{
    public async Task Sync()
    {
        var infoSiteMatches = await apiService.QueryInfoSiteMatchFor180Day();

        var matchInfo = infoSiteMatches.Select(m => new MatchModelFor180Day(m)).ToList();

        var ids = string.Join(",", matchInfo.Select(x => x.MatchId));

        var existingInfoSiteEvents = await GetExistingInfoSite(ids);

        var newInfoSiteIds = matchInfo.ExceptBy(existingInfoSiteEvents.Select(x => x.InfoSiteMatchId),
            x => x.MatchId).Select(x => x.MatchId);

        var loggedChanges = new List<LoggedChangeModel>();

        BuildLogForNewMatches(newInfoSiteIds, loggedChanges);
        BuildLogForChangedMatchesLog(matchInfo, existingInfoSiteEvents, loggedChanges);

        if (loggedChanges.Count != 0)
        {
            await SaveLogChange(loggedChanges);
        }

        var criteria = new SyncInfoMatchFor180DayCriteria
        {
            InfoSiteMatchResultDoc = XmlConverter.Serialize(matchInfo)
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

    private static void BuildLogForChangedMatchesLog(List<MatchModelFor180Day> queriedResult, IList<MatchInfoSite> existedEvents, List<LoggedChangeModel> loggedChanges)
    {
        foreach (var item in existedEvents)
        {
            var infoSiteFromQueried = queriedResult.First(x => x.MatchId == item.InfoSiteMatchId);
            var infoSiteOld = new InfoSiteDb
            {
                InfoSiteMatchId = item.InfoSiteMatchId,
                AwayName = item.AwayName,
                HomeName = item.HomeName,
                LeagueName = item.LeagueName,
                EventDateTime = item.EventDateTime
            };
            var infoSiteNew = new InfoSiteDb
            {
                InfoSiteMatchId = infoSiteFromQueried.MatchId,
                AwayName = infoSiteFromQueried.AwayName,
                HomeName = infoSiteFromQueried.HomeName,
                LeagueName = infoSiteFromQueried.LeagueName,
                EventDateTime = infoSiteFromQueried.KoTime
            };

            var type = typeof(InfoSiteDb);
            var properties = type.GetProperties();

            const string FromService = "180days";

            var loggedChangeItem = InfoSiteSyncHelper.BuildLoggedChanges(properties, infoSiteOld, infoSiteNew, FromService).ToList();
            if (item is { IsCheck: true } && loggedChangeItem.Count > 0)
            {
                loggedChanges.Add(new LoggedChangeModel
                {
                    InfoSiteMatchId = infoSiteNew.InfoSiteMatchId,
                    UpdateTypeId = null,
                    Action = "Update the <b>checking status</b>",
                    NewValue = "Unchecked",
                    OldValue = "Checked",
                    UpDateTime = InfoSiteSyncHelper.DateTimeNowGmt8(),
                    FromService = FromService
                });
            }

            if (loggedChangeItem.Exists(log => log.UpdateTypeId == 24))
            {
                var updateKODateLog = loggedChangeItem.First(l => l.UpdateTypeId == 24);
                loggedChanges.Add(new LoggedChangeModel
                {
                    InfoSiteMatchId = updateKODateLog.InfoSiteMatchId,
                    UpdateTypeId = 25,
                    Action = "Renew Match",
                    NewValue = updateKODateLog.NewValue,
                    OldValue = updateKODateLog.OldValue,
                    UpDateTime = InfoSiteSyncHelper.DateTimeNowGmt8(),
                    FromService = FromService
                });
            }

            loggedChanges.AddRange(loggedChangeItem);
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
            FromService = "180days"
        });

        loggedChanges.AddRange(newLoggedChanges);
    }

    private async Task<List<MatchInfoSite>> GetExistingInfoSite(string ids)
    {
        var criteria = new GetInfoSiteMatchesCriteria
        {
            Ids = ids
        };

        return (await repository.FetchAsync<MatchInfoSite>(criteria).ConfigureAwait(false)).ToList();
    }
}