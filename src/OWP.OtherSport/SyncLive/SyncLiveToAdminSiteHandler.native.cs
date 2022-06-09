using OWP.OtherSport.SyncAdminSiteId.Models;
using OWP.OtherSport.SyncLive.Model;
using OWP.OtherSport.SyncLive.Services;

namespace OWP.OtherSport.SyncLive
{
    public interface SyncLiveToAdminSiteHandler
    {
        Task Handle(IEnumerable<Match> adminSiteMatches, IEnumerable<Match> newMatches, DateTime eventDate);
    }

    public class SyncLiveToAdminSiteHandlerImp: SyncLiveToAdminSiteHandler
    {
        private readonly EditEventGenerator editEventGenerator;

        private readonly ApiService apiService;

        private readonly CacheManipulator cacheManipulator;

        public SyncLiveToAdminSiteHandlerImp(
            EditEventGenerator editEventGenerator, 
            ApiService apiService, 
            CacheManipulator cacheManipulator)
        {
            this.editEventGenerator = editEventGenerator;
            this.apiService = apiService;
            this.cacheManipulator = cacheManipulator;
        }

        public async Task Handle(IEnumerable<Match> adminSiteMatches, IEnumerable<Match> newMatches, DateTime eventDate)
        {
            var editEventList = editEventGenerator.GetChangedOfferLiveMatches(adminSiteMatches, newMatches);
            var deletedMatches = await cacheManipulator.GetDeletedMatches();
            var nonLiveEventList = editEventGenerator.GetNonLiveMatches(deletedMatches, eventDate);
            var eventList = editEventList.Union(nonLiveEventList);

            if (eventList.Count() == 0) return;

            var model = new BodyModel { EditEventList = eventList.ToList() };

            await apiService.PostSyncLiveAsync(model);
            await cacheManipulator.RemoveDeletedMatches();
        }
    }
}
