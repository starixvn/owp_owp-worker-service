using Fanex.Data.Repository;
using OWP.Core.Extensions;
using OWP.Infosite.Criteria;
using OWP.Infosite.Models;
using System.Xml.Linq;

namespace OWP.Infosite.Services;

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
        var infoSiteMatch = await apiService.QueryInfoSiteMatchFor180Day();
        var matchInfo = infoSiteMatch.Select(m => new MatchModelFor180Day(m));

        var criteria = new SyncInfoMatchFor180DayCriteria
        {
            InfoSiteMatchResultDoc = matchInfo.ToXml().ToString(SaveOptions.DisableFormatting)
        };
        await repository.ExecuteAsync(criteria).ConfigureAwait(false);
    }
}