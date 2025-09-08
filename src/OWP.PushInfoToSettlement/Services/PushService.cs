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
        var lastDate =  DateTime.Today.AddDays(-1);
        var todayPlusOne = DateTime.Today.AddDays(1);
        
        var eventsToProcess = new List<EventData>();
        var currentEvents = await eventService.GetEventsAsync();
        
        var processedDates = new List<DateTime>();
        
        for (var currentDate = lastDate; currentDate <= todayPlusOne; currentDate = currentDate.AddDays(1))
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
                await WriteLog(eventsToProcess, "PushMatchInfoForSettlement_Successfully").ConfigureAwait(false);
            }
            else
            {
                await WriteLog(eventsToProcess, "PushMatchInfoForSettlement_Failed").ConfigureAwait(false);
            }
        }
        else
        {
            await UpdateCacheAfterSuccess(processedDates, currentEvents);
        }
    }

    private async Task<List<EventData>> ProcessEventDate(DateTime eventDate, IList<EventData> currentEvents)
    {
        var cachedEvents = await cache.GetMatchesByDate(eventDate);
        var changedEvents = new List<EventData>();

        foreach (var currentEvent in currentEvents)
        {
            var hasChanged = false;

            var cachedEvent = cachedEvents.FirstOrDefault(e => e.OWPMatchId == currentEvent.OWPMatchId);
            
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
            await cache.SaveMatchSnapshot(currentEvents, date);
        }
        // await cache.SaveLastProcessedDate(DateTime.Today);
    }

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