namespace OWP.Infosite.Services;

using Criteria;
using Fanex.Data.Repository;
using Helper;
using Models;
using Newtonsoft.Json;

public interface SyncDeleteChildMatchService
{
    Task Sync();
}

public class SyncDeleteChildMatchServiceImp(
    ApiService apiService,
    IDynamicRepository repository,
    DateTimeService dateTimeService)
    : SyncDeleteChildMatchService
{
    public async Task Sync()
    {
        var dateFrom = dateTimeService.Today;
        var dateTo = dateTimeService.Today.AddDays(7);
        var deletedMatch = await apiService.QueryDeletedChildMatch(dateFrom, dateTo);
        var mappedChildMatches = deletedMatch
            .Where(m => m.sub_type > 0)
            .Select(BuildMappedDeletedChildMatch)
            .GroupBy(x => new { x.AdminSiteMatchId, x.InfoSiteMatchId, x.SubType })
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

    private static MappedDeletedChildMatch BuildMappedDeletedChildMatch(DeleteChildMatch childMatch)
    {
        var (representativeSubtype, _) = ChildMatchHelper.GetBetTypeIdAndRepresentativeSubtype(childMatch.sub_type);

        return new MappedDeletedChildMatch
        {
            InfoSiteMatchId = childMatch.match_id,
            SubType = representativeSubtype,
            AdminSiteMatchId = childMatch.create_match_id
        };
    }
}