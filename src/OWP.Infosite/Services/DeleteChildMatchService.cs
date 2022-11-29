using Fanex.Data.Repository;
using Newtonsoft.Json;
using OWP.Infosite.Criteria;
using OWP.Infosite.Models;

namespace OWP.Infosite.Services;

public interface SyncDeleteChildMatchService
{
    Task Sync(DateTime date);
}

public class SyncDeleteChildMatchServiceImp : SyncDeleteChildMatchService
{
    private readonly ApiService apiService;

    private readonly IDynamicRepository repository;

    public SyncDeleteChildMatchServiceImp(ApiService apiService)
    {
        this.apiService = apiService;
    }

    public async Task Sync(DateTime date)
    {
        var deletedMatch = await apiService.QueryDeletedChildMatch(date, date.AddDays(1));
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
        => new()
        {
            InfoSiteMatchId = childMatch.match_id,
            SubType = childMatch.sub_type,
        };
}