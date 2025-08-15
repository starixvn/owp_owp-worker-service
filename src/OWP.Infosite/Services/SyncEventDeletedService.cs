using Olympus.OWP.Constants;
using Olympus.Xml;
using OWP.Infosite.Models;

namespace OWP.Infosite.Services;

using Criteria;
using Fanex.Data.Repository;
using Fanex.Logging;

public interface SyncEventDeletedService
{
    Task Sync();
}

public class SyncEventDeletedServiceImp(
    DateTimeService dateTimeService,
    ApiService apiService,
    IDynamicRepository repository,
    ILogger logger)
    : SyncEventDeletedService
{
    private string currentDeletedMatchIds = string.Empty;

    public async Task Sync()
    {
        var dateFrom = dateTimeService.Today;
        var dateTo = dateTimeService.Today.AddDays(7);
        var ids = await GetDeletedMatchIds(dateFrom, dateTo);

        if (currentDeletedMatchIds != ids)
        {
            var matchesDeletedLogEventPicking = ids.Split(",").Select(item => new LoggedChangeModel
            {
                Action = UpdateTypeConstant.UpdateType.GetValueOrDefault(UpdateTypeConstant.DELETE_MATCH),
                UpdateTypeId = UpdateTypeConstant.DELETE_MATCH,
                InfoSiteMatchId = int.Parse(item),
                NewValue = string.Empty,
                OldValue = string.Empty,
                UpDateTime = dateTimeService.Now.AddHours(12)
            });
            await SaveDeletedMatchLogEventPicking(matchesDeletedLogEventPicking);
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


    private async Task SaveDeletedMatchLogEventPicking(IEnumerable<LoggedChangeModel> logs)
    {
        var xml = XmlConverter.Serialize(logs);
        var criteria = new SaveMatchDeleteLogsCriteria
        {
            Xml = xml
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

#pragma warning disable S4462 // Replace this use of 'Task.Result' with 'await', disabled by Tyler
        var ids = tasks.Select(t => t.Result)
            .SelectMany(i => i)
            .OrderBy(i => i);

        return string.Join(",", ids);
    }
#pragma warning restore S4462 // Replace this use of 'Task.Result' with 'await'
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