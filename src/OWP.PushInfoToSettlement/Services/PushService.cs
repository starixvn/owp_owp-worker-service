using OWP.PushInfoToSettlement.Model;

namespace OWP.PushInfoToSettlement.Services;

public interface PushService
{
    Task Execute();
}

public class PushServiceImp(CacheManipulator cache, EventService eventService) : PushService
{
    public async Task Execute()
    {
        var lastProcessedDate = await cache.GetLastProcessedDate() ?? DateTime.Today.AddDays(-5);
        var todayPlusOne = DateTime.Today.AddDays(1);
        
        var eventsToProcess = new List<EventData>();
        
        var currentEvents = await eventService.GetEventsAsync();
        
        for (var currentDate = lastProcessedDate; currentDate <= todayPlusOne; currentDate = currentDate.AddDays(1))
        {
            var cachedEvents = await cache.GetMatchesByDate(currentDate);

            var cachedEventsDict = cachedEvents.ToDictionary(e => e.OWPMatchId, e => e);

            foreach (var currentEvent in currentEvents)
            {
                var hasChanged = false;

                if (cachedEventsDict.TryGetValue(currentEvent.OWPMatchId, out var cachedEvent))
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
                    eventsToProcess.Add(currentEvent);
                }
            }

            await cache.SaveMatchSnapshot(currentEvents, currentDate);
        }
        
        await cache.SaveLastProcessedDate(DateTime.Today);
        
        Console.WriteLine($"Found {eventsToProcess.Count} events to process");
    }
}