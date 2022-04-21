using OWP.OtherSport.SyncAdminSiteId.Models;
using OWP.OtherSport.SyncLive.Model;
using OWP.OtherSport.SyncLive.Services;

namespace OWP.OtherSport.SyncLive
{
    public interface SyncLiveToAdminSiteHandler
    {
        void Handle(IEnumerable<Match> adminSiteMatches, IEnumerable<Match> newMatches);
    }

    public class SyncLiveToAdminSiteHandlerImp: SyncLiveToAdminSiteHandler
    {
        private readonly EditEventGenerator editEventGenerator;

        private readonly ApiService apiService;

        public SyncLiveToAdminSiteHandlerImp(EditEventGenerator editEventGenerator, ApiService apiService)
        {
            this.editEventGenerator = editEventGenerator;
            this.apiService = apiService;
        }

        public async void Handle(IEnumerable<Match> adminSiteMatches, IEnumerable<Match> newMatches)
        {
            var editEventList = editEventGenerator.CreateModel(adminSiteMatches, newMatches);
            if (editEventList.Count == 0) return;

            var model = new BodyModel
            {
                From = "OWP",
                UserId = 0,
                EditEventList = editEventList
            };

            await apiService.PostSyncLiveAsync(model);
        }
    }
}
