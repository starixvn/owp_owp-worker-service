using Fanex.Data.Repository;
using Olympus.Xml;
using OWP.Infosite.Criteria;
using OWP.Infosite.Models;

namespace OWP.Infosite.Services;

public interface SyncService
{
    Task Sync();
}

public class SyncServiceImp
{
    private readonly IDynamicRepository repository;
    private readonly ApiService apiService;

    protected readonly DateTime minDate = new DateTime(2000, 1, 1);
    protected DateTimeService dateTimeService { get; }

    public SyncServiceImp(
        DateTimeService dateTimeService,
        ApiService apiService,
        IDynamicRepository repository)
    {
        this.dateTimeService = dateTimeService;
        this.apiService = apiService;
        this.repository = repository;
    }

    public async Task Sync(DateTime fromDate)
    {
        var queriedResult = await QueryEvents(fromDate);
        await SaveEvents(queriedResult);
        await UpdateSoloMatch();
    }

    private async Task SaveEvents(List<Event> events)
    {
        var xml = XmlConverter.Serialize(events);
        var criteria = new PushInfoSiteMatchCommand
        {
            Xml = xml
        };
        await repository.ExecuteAsync(criteria).ConfigureAwait(false);
    }

    private async Task UpdateSoloMatch()
    {
        var criteria = new UpdateSoloMatchCriteria();
        await repository.ExecuteAsync(criteria).ConfigureAwait(false);
    }

    protected async Task<List<Event>> QueryEvents(DateTime date)
    {
        var param = new ParamsQueryModel
        {
            FromDate = date,
            ToDate = date,
            LastUpdatedTime = minDate
        };

        var commonMatches = await apiService.QueryAsync(param);
        var queriedEvents = ToEvents(commonMatches);
        var validEvents = GetValidEvents(queriedEvents);

        return validEvents.ToList();
    }

    private static IEnumerable<Event> GetValidEvents(IEnumerable<Event> queriedEvents)
    {
        return queriedEvents.Where(evt => evt.IsValid);
    }

    private static IEnumerable<Event> ToEvents(IEnumerable<CommonMatchModel> commonMatches)
    {
        return commonMatches.Select(evt => new Event(evt));
    }
}