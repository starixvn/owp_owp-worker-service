namespace OWP.Infosite.Services;

using System;
using System.Linq;
using Criteria;
using Fanex.Data.Repository;
using Models;
using Olympus.Xml;

public interface SyncService
{
    Task Sync();
}

public class SyncServiceImp(
    DateTimeService dateTimeService,
    ApiService apiService,
    IDynamicRepository repository,
    EventPickingLogsService eventPickingLogsService,
    ProviderService providerService)
{
    protected readonly DateTime MinDate = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    protected DateTimeService DateTimeService { get; } = dateTimeService;

    public async Task Sync(DateTime fromDate)
    {
        var queriedResult = await QueryEvents(fromDate);
        var infoSiteDbInfos = await GetExistingInfoSite(string.Join(",", queriedResult.Select(x => x.InfoSiteMatchId)));

        await providerService.UpdateProviders(queriedResult, infoSiteDbInfos);
        await eventPickingLogsService.BuildLogs(queriedResult, infoSiteDbInfos, fromDate, "Today");

        var teamRoundsTask = await QueryTeamRound(fromDate);
        MapRoundToEvents(queriedResult, teamRoundsTask);

        await SaveEvents(queriedResult);
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

    private async Task<List<InfoSiteDbInfo>> GetExistingInfoSite(string ids)
    {
        var criteria = new GetExistingInfoSiteMatchesCriteria
        {
            Ids = ids
        };

        return (await repository.FetchAsync<InfoSiteDbInfo>(criteria)).ToList();
    }

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
}