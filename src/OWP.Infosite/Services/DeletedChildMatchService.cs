using Fanex.Data.Repository;
using Newtonsoft.Json;
using OWP.Infosite.Criteria;
using OWP.Infosite.Helper;
using OWP.Infosite.Models;

namespace OWP.Infosite.Services;

public interface SyncDeletedChildMatchService
{
    Task Sync();
}

public class SyncDeletedChildMatchServiceImp : SyncDeletedChildMatchService
{
    private readonly ApiService apiService;

    private readonly IDynamicRepository repository;

    private readonly DateTimeService dateTimeService;

    public SyncDeletedChildMatchServiceImp(ApiService apiService, IDynamicRepository repository, DateTimeService dateTimeService)
    {
        this.apiService = apiService;
        this.repository = repository;
        this.dateTimeService = dateTimeService;
    }

    public async Task Sync()
    {
        var dateFrom = dateTimeService.Today;
        var dateTo = dateTimeService.Today.AddDays(7);
        var deletedMatch = await apiService.QueryDeletedChildMatch(dateFrom, dateTo);
        var mappedChildMatches = deletedMatch
        .Where(m => m.sub_type > 0)
        .Select(m => BuildMappedDeletedChildMatch(m))
        .GroupBy(x => new { x.InfoSiteMatchId, x.SubType })
        .Select(x => x.First());

        await DeleteChildMatch(mappedChildMatches);

        await Task.CompletedTask;
    }

    private async Task DeleteChildMatch(IEnumerable<MappedDeletedChildMatch> childMatches)
    {
        var criteria = new DeletedChildMatch
        {
            ChildMatchesJson = JsonConvert.SerializeObject(childMatches)
        };

        await repository.ExecuteAsync(criteria);
    }

    private MappedDeletedChildMatch BuildMappedDeletedChildMatch(DeleteChildMatch childMatch)
    {
        var (subType, _) = ChildMatchHelper.GetBettypeIdAndRepresentativeSubtype(childMatch.sub_type);
        return new()
        {
            InfoSiteMatchId = childMatch.match_id,
            SubType = subType,
        };
    }
}