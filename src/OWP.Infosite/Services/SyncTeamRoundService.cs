namespace OWP.Infosite.Services;

using Criteria;
using Fanex.Data.Repository;
using Models;
using Olympus.Xml;

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
        });

        var criteria = new SyncTeamRoundCriteria
        {
            Xml = XmlConverter.Serialize(roundInfo)
        };
        await repository.ExecuteAsync(criteria).ConfigureAwait(false);
    }

    private static string NormalizeTeamRound(string teamRound)
        => (string.IsNullOrEmpty(teamRound) || teamRound=="/")
            ?string.Empty
            : teamRound;
}