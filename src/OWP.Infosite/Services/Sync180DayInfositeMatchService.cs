using OWP.Infosite.Constants;

namespace OWP.Infosite.Services;

using Criteria;
using Fanex.Data.Repository;
using Models;
using Olympus.Xml;
using OWP.Infosite.Helper;
using System.Reflection;

public interface Sync180DayInfositeMatchService
{
    Task Sync();
}

public class Sync180DayInfositeMatchServiceImp : Sync180DayInfositeMatchService
{
    private readonly ApiService apiService;
    private readonly IDynamicRepository repository;

    public Sync180DayInfositeMatchServiceImp(ApiService apiService, IDynamicRepository repository)
    {
        this.apiService = apiService;
        this.repository = repository;
    }

    public async Task Sync()
    {
        var infoSiteMatches = await apiService.QueryInfoSiteMatchFor180Day();

        var matchInfo = infoSiteMatches.Select(m => new MatchModelFor180Day(m));

        var infoSiteEvents = await GetInfoSiteByDate();

        var loggedChanges = new List<LoggedChangeModel>();

        BuildLogForNewMatches(matchInfo, infoSiteEvents, loggedChanges);
        BuildLogForChangedMatchesLog(matchInfo, infoSiteEvents, loggedChanges);

        if (loggedChanges.Any())
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

    private static void BuildLogForChangedMatchesLog(IEnumerable<MatchModelFor180Day> queriedResult, IList<MatchInfoSite> infoSiteEvents, List<LoggedChangeModel> loggedChanges)
    {
        var existedEvents =
            queriedResult.IntersectBy(infoSiteEvents.Select(x => x.InfoSiteMatchId), x => x.MatchId).ToList();

        foreach (var item in existedEvents)
        {
            var findInfoSiteExisted = infoSiteEvents.First(x => x.InfoSiteMatchId == item.MatchId);
            var infoSiteOld = new InfoSiteDb()
            {
                InfoSiteMatchId = findInfoSiteExisted.InfoSiteMatchId,
                AwayName = findInfoSiteExisted.AwayName,
                HomeName = findInfoSiteExisted.HomeName,
                LeagueName = findInfoSiteExisted.LeagueName,
                EventDateTime = findInfoSiteExisted.EventDateTime
            };
            var infoSiteNew = new InfoSiteDb()
            {
                InfoSiteMatchId = item.MatchId,
                AwayName = item.AwayName,
                HomeName = item.HomeName,
                LeagueName = item.LeagueName,
                EventDateTime = item.KoTime
            };

            var type = typeof(InfoSiteDb);
            var properties = type.GetProperties();

            var loggedChangeItem = InfoSiteSyncHelper.BuildLoggedChanges(properties, infoSiteOld, infoSiteNew).ToList();
            if (findInfoSiteExisted is { IsCheck: true } && loggedChangeItem.Any())
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

    private static void BuildLogForNewMatches(IEnumerable<MatchModelFor180Day> queriedResult, IEnumerable<MatchInfoSite> infoSiteEvents, List<LoggedChangeModel> loggedChanges)
    {
        var newEvents = queriedResult.ExceptBy(infoSiteEvents.Select(x => x.InfoSiteMatchId), x => x.MatchId);
        var newLoggedChanges = newEvents.Select(newEvent => new LoggedChangeModel
        {
            UpdateTypeId = 1,
            Action = Constant.UpdateType.GetValueOrDefault(1),
            InfoSiteMatchId = newEvent.MatchId,
            UpDateTime = InfoSiteSyncHelper.DateTimeNowGmt8(),
            NewValue = null,
            OldValue = "Initialize"
        });

        loggedChanges.AddRange(newLoggedChanges);
    }


    private async Task<List<MatchInfoSite>> GetInfoSiteByDate()
    {
        var criteria = new GetInfoSiteMatchesCriteria();

        return (await repository.FetchAsync<MatchInfoSite>(criteria).ConfigureAwait(false)).ToList();
    }
}