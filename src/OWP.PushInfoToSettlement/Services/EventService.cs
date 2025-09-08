
using Fanex.Data.Repository;
using OWP.PushInfoToSettlement.Criteria;
using OWP.PushInfoToSettlement.Model;

namespace OWP.PushInfoToSettlement.Services
{
    public interface EventService
    {
        Task<IList<EventData>> GetEventsAsync();
    }
    public class EventServiceImp(IDynamicRepository dynamicRepository) : EventService
    {
        public async Task<IList<EventData>> GetEventsAsync()
        {
            var criteria = new GetEventCriteria();
            var events = await dynamicRepository.FetchAsync<EventData>(criteria);
            return events.ToList();
        }
    }
}
