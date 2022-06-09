using OWP.OtherSport.SyncAdminSiteId.Models;
using OWP.OtherSport.SyncLive.Model;

namespace OWP.OtherSport.SyncLive.Services
{
    public interface EditEventGenerator
    {
        IEnumerable<EditEvent> GetChangedOfferLiveMatches(IEnumerable<Match> adminSiteMatches, IEnumerable<Match> owpMatches);

        IEnumerable<EditEvent> GetNonLiveMatches(IEnumerable<DeletedMatch> deletedMatches, DateTime eventDate);
    }

    public class EditEventGeneratorImp : EditEventGenerator
    {
        public IEnumerable<EditEvent> GetChangedOfferLiveMatches(IEnumerable<Match> adminSiteMatches, IEnumerable<Match> owpMatches)
        {
            var owpBasketballMatch = FilterBasketballMatch(owpMatches);
            var adminSiteBasketballMatch = FilterBasketballMatch(adminSiteMatches);
            var changedAdminSites = GetChangedLiveAdminSites(adminSiteBasketballMatch, owpBasketballMatch);

            return changedAdminSites;
        }

        public IEnumerable<EditEvent> GetChangedLiveAdminSites(IEnumerable<Match> adminSiteMatches, IEnumerable<Match> owpMatches)
        {
            var owpMatchHasAdminSiteId = owpMatches.Where(x => x.AdminSiteMatchId.HasValue).Distinct();
            foreach (var match in owpMatchHasAdminSiteId)
            {
                var adminsiteMatch = adminSiteMatches.Where(x => x.AdminSiteMatchId == match.AdminSiteMatchId).FirstOrDefault();
                if (adminsiteMatch != null && adminsiteMatch.OfferLive != match.OfferLive)
                {
                    yield return new EditEvent { 
                        Matchid = (int)match.AdminSiteMatchId, 
                        OfferLive = match.OfferLive 
                    };
                }
            }
        }

        public IEnumerable<EditEvent> GetNonLiveMatches(IEnumerable<DeletedMatch> deletedMatches, DateTime eventDate)
        {
            var filterMatches = deletedMatches.Where(x =>
                IsBasketball(x.Sport) &&
                x.AdminSiteMatchId.HasValue &&
                x.EventDate.Date >= eventDate.Date);

            return filterMatches.Select(x => new EditEvent { Matchid = (int)x.AdminSiteMatchId, OfferLive = false });
        }

        private IEnumerable<Match> FilterBasketballMatch(IEnumerable<Match> matches)
            => matches.Where(x => IsBasketball(x.Sport));

        private bool IsBasketball(string sport) => sport.Trim().Equals("basketball", StringComparison.OrdinalIgnoreCase);
    }
}
