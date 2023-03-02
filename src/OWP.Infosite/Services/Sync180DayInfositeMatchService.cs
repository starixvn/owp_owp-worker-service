namespace OWP.Infosite.Services;

using Criteria;
using Fanex.Data.Repository;
using Models;
using Olympus.Xml;

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

        var criteria = new SyncInfoMatchFor180DayCriteria
        {
            InfoSiteMatchResultDoc = XmlConverter.Serialize(matchInfo)
        };
        await repository.ExecuteAsync(criteria).ConfigureAwait(false);
    }
}