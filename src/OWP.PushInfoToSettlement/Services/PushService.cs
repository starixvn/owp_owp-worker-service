using OWP.PushInfoToSettlement.Model;

namespace OWP.PushInfoToSettlement.Services;

public interface PushService
{
    Task Execute();
}

public class PushServiceImp(CacheManipulator cache, EventService eventService, ISettlementApiService settlementApiService) : PushService
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
                Console.WriteLine($"Successfully sent {eventsToProcess.Count} events to settlement system");
            }
            else
            {
                Console.WriteLine($"Failed to send {eventsToProcess.Count} events to settlement system - cache not updated");
            }
        }
        else
        {
            await UpdateCacheAfterSuccess(processedDates, currentEvents);
            Console.WriteLine("No events to process");
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
}