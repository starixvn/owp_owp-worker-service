using Newtonsoft.Json;
using Olympus.OWP.Log.Data.Repository.Criteria;
using Olympus.OWP.Log.Data.Repository.Services;
using OWP.PushInfoToSettlement.Model;

namespace OWP.PushInfoToSettlement.Services;

public interface PushService
{
    Task Execute();
}

public class PushServiceImp(CacheManipulator cache, EventService eventService, ISettlementApiService settlementApiService, LogWriterService logger) : PushService
{
    public async Task Execute()
    {
        var nowGmt8 = DateTime.Now.AddHours(12);
        var selectedDate = nowGmt8.Hour >= 12 ? nowGmt8.Date : nowGmt8.AddDays(-1).Date;
        var fromDate = selectedDate.AddDays(-1);
        var toDate = selectedDate.AddDays(1);
        
        var eventsToProcess = new List<EventData>();
        var currentEvents = await eventService.GetEventsAsync();
        
        var processedDates = new List<DateTime>();
        
        for (var currentDate = fromDate; currentDate <= toDate; currentDate = currentDate.AddDays(1))
        {
            var changedEvents = await ProcessEventDate(currentDate, currentEvents);
            eventsToProcess.AddRange(changedEvents);
            processedDates.Add(currentDate);
        }

        if (eventsToProcess.Any())
        {
            var success = await ProcessApiCall(eventsToProcess);

            if (success)
            {
                await UpdateCacheAfterSuccess(processedDates, currentEvents);
            }

            var logInfo = new
            {
                processedDates,
                numberOfEvents = eventsToProcess.Count,
                status = success,
                eventsToProcess
            };

            await WriteLog(logInfo, "PushMatchInfoForSettlement").ConfigureAwait(false);
        }
        else
        {
            await UpdateCacheAfterSuccess(processedDates, currentEvents);
        }

    }

    private async Task<List<EventData>> ProcessEventDate(DateTime eventDate, IList<EventData> currentEvents)
    {
        var (fromDate, toDate) = NormalizeTimeRange(eventDate, eventDate);
        var eventsForDate = currentEvents.Where(e => e.EventDateTime >= fromDate && e.EventDateTime <= toDate).ToList();
        var changedEvents = new List<EventData>();
        
        foreach (var currentEvent in eventsForDate)
        {
            var hasChanged = false;

            var cachedEvent = eventsForDate.FirstOrDefault(e => e.OWPMatchId == currentEvent.OWPMatchId);
            
            if (cachedEvent != null)
            {
                if (cachedEvent.Remark != currentEvent.Remark)
                {
                    hasChanged = true;
                }
            }
            else
            {
                hasChanged = true;
            }

            if (hasChanged)
            {
                changedEvents.Add(currentEvent);
            }
        }

        // Don't save snapshot here - will be saved after successful API call
        return changedEvents;
    }

    private async Task<bool> ProcessApiCall(List<EventData> eventsToProcess)
    {
        var changeInstantSettingRequests = eventsToProcess.Select(e => new ChangeInstantSettingRequest
        {
            MatchId = e.AdminSiteId,
            Remark = e.Remark
        }).ToList();

        return await settlementApiService.ChangeInstantSettingAsync(changeInstantSettingRequests);
    }

    private async Task UpdateCacheAfterSuccess(List<DateTime> processedDates, IList<EventData> currentEvents)
    {
        foreach (var date in processedDates)
        {
            var (fromDate, toDate) = NormalizeTimeRange(date, date);
            var eventsForDate = currentEvents.Where(e => e.EventDateTime >= fromDate && e.EventDateTime <= toDate).ToList();
            await cache.SaveMatchSnapshot(eventsForDate, date);
        }
    }

    private (DateTime fromDate, DateTime toDate) NormalizeTimeRange(DateTime dayStart, DateTime dayEnd)
        => (dayStart.Date.AddHours(12), dayEnd.Date.AddDays(1).AddHours(11).AddMinutes(59));

    private async Task WriteLog(object request, string action)
    {
        await logger.Execute(new OwpLogCriteria
        {
            CreatedDate = DateTime.Now.AddHours(12),
            Action = action,
            Payload = JsonConvert.SerializeObject(request)
        });
    }
}