using Fanex.Data.Repository;
using OWP.PrematchReport.Models;
using OWP.PrematchReport.Criteria;

namespace OWP.PrematchReport;
public interface Repository
{
    Task<IList<League>> GetLeagueByDate(DateTime eventDate);
}
public class RepositoryImp : Repository
{
    private readonly IDynamicRepository repository;

    public RepositoryImp(IDynamicRepository repository)
    {
        this.repository = repository;
    }

    public async Task<IList<League>> GetLeagueByDate(DateTime eventDate)
    {
        var command = new GetLeagueCriteria()
        {
            EventDate = eventDate
        };
        var leagueInfo = await repository.FetchAsync<League>(command);

        return leagueInfo.ToList();
    }
}