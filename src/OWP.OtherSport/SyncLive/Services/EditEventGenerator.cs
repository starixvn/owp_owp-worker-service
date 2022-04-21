using OWP.OtherSport.SyncAdminSiteId.Models;
using OWP.OtherSport.SyncLive.Model;

namespace OWP.OtherSport.SyncLive.Services
{
    public interface EditEventGenerator
    {
        List<EditEvent> CreateModel(IEnumerable<Match> adminSiteMatches, IEnumerable<Match> newMatches);
    }

    public class EditEventGeneratorImp : EditEventGenerator
    {
        public List<EditEvent> CreateModel(IEnumerable<Match> adminSiteMatches, IEnumerable<Match> owpMatches)
        {            
            var changedAdminSites = GetChangedLiveAdminSiteIds(adminSiteMatches, owpMatches);

            return CreateAdminSitesModel(changedAdminSites).ToList();
        }

        public IEnumerable<Match> GetChangedLiveAdminSiteIds(IEnumerable<Match> adminSiteMatches, IEnumerable<Match> owpMatches)
        {
            var filteredMatchHasAdminSiteId = owpMatches.Where(x => x.AdminSiteMatchId.HasValue).Distinct();
            foreach (var match in filteredMatchHasAdminSiteId)
            {
                var adminsiteMatch = adminSiteMatches.Where(x => x.AdminSiteMatchId == match.AdminSiteMatchId).FirstOrDefault();
                if (adminsiteMatch != null && adminsiteMatch.OfferLive != match.OfferLive)
                {
                    yield return match;
                }
            }
        }

        public IEnumerable<EditEvent> CreateAdminSitesModel(IEnumerable<Match> adminSiteMatches)
        {
            foreach(var adminSiteMatch in adminSiteMatches)
            {
                yield return new EditEvent
                {
                    Matchid = (int)adminSiteMatch.AdminSiteMatchId,
                    OfferLive = adminSiteMatch.OfferLive
                };
            }
        }
    }
}
