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
        var lastProcessedDate = await cache.GetLastProcessedDate() ?? DateTime.Today.AddDays(-5);
        var todayPlusOne = DateTime.Today.AddDays(1);
        
        var eventsToProcess = new List<EventData>();
        var currentEvents = await eventService.GetEventsAsync();
        
        for (var currentDate = lastProcessedDate; currentDate <= todayPlusOne; currentDate = currentDate.AddDays(1))
        {
            var changedEvents = await ProcessEventDate(currentDate, currentEvents);
            eventsToProcess.AddRange(changedEvents);
        }
        
        await cache.SaveLastProcessedDate(DateTime.Today);
        
        if (eventsToProcess.Any())
        {
            var changeInstantSettingRequests = eventsToProcess.Select(e => new ChangeInstantSettingRequest
            {
                MatchId = e.AdminSiteMatchId,
                Remark = e.Remark
            }).ToList();

            var success = await settlementApiService.ChangeInstantSettingAsync(changeInstantSettingRequests);
            
            if (success)
            {
                Console.WriteLine($"Successfully sent {eventsToProcess.Count} events to settlement system");
            }
            else
            {
                Console.WriteLine($"Failed to send {eventsToProcess.Count} events to settlement system");
            }
        }
        else
        {
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

        await cache.SaveMatchSnapshot(currentEvents, eventDate);
        
        return changedEvents;
    }
}