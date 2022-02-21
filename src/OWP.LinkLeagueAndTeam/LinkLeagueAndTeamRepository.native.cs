using Fanex.Data.Repository;
using OWP.LinkLeagueAndTeam.Criteria;
using OWP.LinkLeagueAndTeam.Models;

namespace OWP.LinkLeagueAndTeam;

public interface LinkLeagueAndTeamRepository
{
    Task<IList<TeamInfo>> GetTeamInfoInTimeRange(DateTime from, DateTime to);

    Task InsertOrUpdateTeamInfo(TeamInfo teamInfo);
}

public class LinkLeagueAndTeamRepositoryImp : LinkLeagueAndTeamRepository
{
    private readonly IDynamicRepository repository;

    public LinkLeagueAndTeamRepositoryImp(IDynamicRepository repository)
    {
        this.repository = repository;
    }

    public async Task<IList<TeamInfo>> GetTeamInfoInTimeRange(DateTime @from, DateTime to)
    {
        var command = new GetTeamInfoInTimeRangeCriteria
        {
            FromDate = from,
            ToDate = to
        };
        var teamInfo = await repository.FetchAsync<TeamInfo>(command);

        return teamInfo.ToList();
    }

    public async Task InsertOrUpdateTeamInfo(TeamInfo teamInfo)
    {
        var command = new InsertOrUpdateCriteria
        {
            LeagueId = teamInfo.LeagueId,
            TeamId = teamInfo.TeamId
        };
        await repository.ExecuteAsync(command);
    }
}