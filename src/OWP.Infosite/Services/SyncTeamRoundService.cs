using OWP.Infosite.Constants;

namespace OWP.Infosite.Services;

using Criteria;
using Fanex.Data.Repository;
using Models;
using Olympus.Xml;
using OWP.Infosite.Helper;
using System.Reflection;

public interface SyncTeamRoundService
{
    Task Sync(DateTime fromDate);
}

public class SyncTeamRoundServiceImp : SyncTeamRoundService
{
    private readonly ApiService apiService;
    private readonly IDynamicRepository repository;

    public SyncTeamRoundServiceImp(ApiService apiService, IDynamicRepository repository)
    {
        this.apiService = apiService;
        this.repository = repository;
    }

    public async Task Sync(DateTime fromDate)
    {
        var roundTw = await apiService.QueryRound(fromDate,fromDate);
        var roundInfo = roundTw.content?.Select(r => new TeamRoundInfo{
            InfoSiteMatchId=r.match_id,
            TeamRound=NormalizeTeamRound(r.round_home_away_total)
        }).ToList();

        var roundDbs =await GetRoundInfoByDate(fromDate);

        var loggedChanges = BuildLoggedRoundChanges(roundInfo, roundDbs);
        if (loggedChanges.Any())
        {
            await SaveLogRoundChange(loggedChanges);
        }
        var criteria = new SyncTeamRoundCriteria
        {
            Xml = XmlConverter.Serialize(roundInfo)
        };
        await repository.ExecuteAsync(criteria);
    }

    private static string NormalizeTeamRound(string teamRound)
        => (string.IsNullOrEmpty(teamRound) || teamRound=="/")
            ?string.Empty
            : teamRound;

    private static List<LoggedChangeModel> BuildLoggedRoundChanges(List<TeamRoundInfo> temRoundInfos, List<TeamRoundInfoDb> teamRoundInfoDbs)
    {
        var loggedChanges = new List<LoggedChangeModel>();
        var existedTeamRounds =
            temRoundInfos.IntersectBy(teamRoundInfoDbs.Select(x => x.InfoSiteMatchId), x => x.InfoSiteMatchId).ToList();
        foreach (var item in existedTeamRounds)
        {

            var teamRoundDb = teamRoundInfoDbs.FirstOrDefault(x => x.InfoSiteMatchId == item.InfoSiteMatchId);
            var newValue = item.TeamRound;
            var oldValue = teamRoundDb?.TeamRound ?? string.Empty;
            if (!string.Equals(newValue, oldValue ))
            {
                var updateTypeId = InfoSiteSyncHelper.BuildActionId("TeamRound", newValue, oldValue);
                loggedChanges.Add(new LoggedChangeModel
                {
                    InfoSiteMatchId = item.InfoSiteMatchId,
                    UpdateTypeId = updateTypeId,
                    Action = Constant.UpdateType.GetValueOrDefault(updateTypeId),
                    NewValue = newValue,
                    OldValue = oldValue,
                    UpDateTime = InfoSiteSyncHelper.DateTimeNowGmt8()
                });
                if (teamRoundDb is { IsCheck: true })
                {
                    loggedChanges.Add(new LoggedChangeModel
                    {
                        InfoSiteMatchId = item.InfoSiteMatchId,
                        UpdateTypeId = null,
                        Action = "Update the <b>checking status</b>",
                        NewValue = "Unchecked",
                        OldValue = "Checked",
                        UpDateTime = InfoSiteSyncHelper.DateTimeNowGmt8()
                    });
                }
            }

        }

        return loggedChanges;
    }

    private async Task<List<TeamRoundInfoDb>> GetRoundInfoByDate(DateTime date)
    {
        var criteria = new GetTeamRoundInfoByDateCriteria
        {
            EventDate = date
        };

        return (await repository.FetchAsync<TeamRoundInfoDb>(criteria).ConfigureAwait(false)).ToList();
    }

    private async Task SaveLogRoundChange(IEnumerable<LoggedChangeModel> loggedChange)
    {
        var xml = XmlConverter.Serialize(loggedChange);
        var criteria = new SaveLogsChangeCriteria
        {
            Xml = xml
        };
        await repository.ExecuteAsync(criteria).ConfigureAwait(false);
    }

}