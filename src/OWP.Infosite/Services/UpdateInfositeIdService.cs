using Fanex.Data.Repository;
using Olympus.Xml;
using OWP.Infosite.Criteria;
using OWP.Infosite.Models;

namespace OWP.Infosite.Services;

public interface UpdateInfositeIdService
{
    Task UpdateInfositeId(DateTime fromDate);
}

public class UpdateInfositeIdServiceImp : UpdateInfositeIdService
{
    private readonly ApiService apiService;
    private readonly IDynamicRepository repository;

    public UpdateInfositeIdServiceImp(ApiService apiService, IDynamicRepository repository)
    {
        this.apiService = apiService;
        this.repository = repository;
    }

    public async Task UpdateInfositeId(DateTime fromDate)
    {
        var queriedResult = await QueryMergedMatchEvents(fromDate);
        await SaveEvents(queriedResult);
    }

    public async Task<List<MatchRelationEvent>> QueryMergedMatchEvents(DateTime date)
    {
        var param = new ParamsQueryModel
        {
            FromDate = date,
            ToDate = date
        };

        var matchRelations = await apiService.QueryMatchesRelation(param);

        var filteredMatchRelations = matchRelations.Where(x => x.match_id != x.parent_match_id).ToList();
        var matchRelationEvents = ToEvents(filteredMatchRelations);

        return matchRelationEvents;
    }

    public async Task SaveEvents(List<MatchRelationEvent> events)
    {
        var criteria = new UpdateInfositeIdCommand
        {
            MatchRelationEvents = XmlConverter.Serialize(events)
        };

        await repository.ExecuteAsync(criteria);
    }

    private static List<MatchRelationEvent> ToEvents(List<MatchRelation> matchRelations)
        => matchRelations.Select(matchRelation => new MatchRelationEvent(matchRelation)).ToList();
}