using Fanex.Data.Repository;
using Fanex.Logging;
using OWP.Infosite.Criteria;

namespace OWP.Infosite.Services;

public interface SyncEventDeletedService
{
    Task Sync();
}

public class SyncEventDeletedServiceImp : SyncEventDeletedService
{
    private readonly ApiService apiService;

    private readonly IDynamicRepository repository;

    private readonly ILogger logger;

    private readonly DateTimeService dateTimeService;

    private string currentDeletedMatchIds;

    public SyncEventDeletedServiceImp(
      DateTimeService dateTimeService,
      ApiService apiService,
      IDynamicRepository repository,
      ILogger logger)
    {
        this.dateTimeService = dateTimeService;
        this.apiService = apiService;
        this.repository = repository;
        this.logger = logger;
        currentDeletedMatchIds = string.Empty;
    }

    public async Task Sync()
    {
        var dateFrom = dateTimeService.Today;
        var dateTo = dateTimeService.Today.AddDays(7);
        var ids = await GetDeletedMatchIds(dateFrom, dateTo);

        if (currentDeletedMatchIds != ids)
        {
            await WriteLogForDebug(ids, dateFrom, dateTo);
            await DeleteMatch(ids);
        }

        currentDeletedMatchIds = ids;
    }

    private async Task DeleteMatch(string ids)
    {
        var criteria = new SyncEventDeletedInfoSiteMatchCriteria
        {
            Ids = ids
        };

        await repository.ExecuteAsync(criteria).ConfigureAwait(false);
    }

    private async Task<string> GetDeletedMatchIds(DateTime dateFrom, DateTime dateTo)
    {
        var tasks = new List<Task<IEnumerable<string>>>();
        for (var date = dateFrom; date <= dateTo; date = date.AddDays(1))
        {
            var task = GetDeletedMatchIds(date);
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);

        var ids = tasks.Select(t => t.Result)
            .SelectMany(i => i)
            .OrderBy(i => i);

        return string.Join(",", ids);
    }

    private async Task<IEnumerable<string>> GetDeletedMatchIds(DateTime date)
    {
        var allMatches = await apiService.GetInvalidEventKeysByDateAsync(date);

        var matchIds = allMatches.Where(m => m.Contains("-"))
            .Select(m => m.Split('-')[0])
            .Where(i => !string.IsNullOrEmpty(i));

        return matchIds;
    }

    private async Task WriteLogForDebug(string ids, DateTime dateFrom, DateTime dateTo)
    {
        var logMessages = $"SyncEventDeletedService from {dateFrom} to {dateTo} at {DateTime.Now}: {ids}";
        await logger.InfoAsync(logMessages);
    }
}