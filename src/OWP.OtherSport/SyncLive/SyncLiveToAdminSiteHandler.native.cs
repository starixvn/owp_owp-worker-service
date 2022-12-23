using OWP.OtherSport.SyncAdminSiteId.Models;
using OWP.OtherSport.SyncLive.Model;
using OWP.OtherSport.SyncLive.Services;

namespace OWP.OtherSport.SyncLive
{
    public interface SyncLiveToAdminSiteHandler
    {
        Task Handle(IEnumerable<Match> adminSiteMatches, IEnumerable<Match> owpMatches, DateTime eventDate);
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

        public async Task Handle(IEnumerable<Match> adminSiteMatches, IEnumerable<Match> owpMatches, DateTime eventDate)
        {
            var deletedMatches = await cacheManipulator.GetDeletedMatches();

            var duplicateAdminSiteIdMatches = owpMatches.Where(x => CheckDuplicateAdminSiteId(owpMatches, x.AdminSiteMatchId));
            var syncMatches = owpMatches.Except(duplicateAdminSiteIdMatches);
            
            var editEventList = editEventGenerator.GetChangedOfferLiveMatches(adminSiteMatches, syncMatches.Union(deletedMatches));

            if (editEventList.Count() == 0) return;

            var model = new BodyModel { live_configs = editEventList.ToList() };

            await apiService.PostSyncLiveAsync(model);
            await cacheManipulator.RemoveDeletedMatches();
        }

        private static bool CheckDuplicateAdminSiteId(IEnumerable<Match> match, int? id)
            => match.Count(x => x.AdminSiteMatchId == id) > 1;
    }
}
