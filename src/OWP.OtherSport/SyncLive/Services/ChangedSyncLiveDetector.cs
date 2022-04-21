using OWP.OtherSport.SyncAdminSiteId.Models;

namespace OWP.OtherSport.SyncLive.Services
{
    public interface ChangedSyncLiveDetector
    {
        IEnumerable<Match> FilterMatchHasAdminSiteId(IEnumerable<Match> matches);

        IEnumerable<Match> GetChangedLiveAdminSiteIds(IEnumerable<Match> adminSiteMatches, IEnumerable<Match> newMatches);
    }

    public class ChangedSyncLiveDetectorImp: ChangedSyncLiveDetector
    {
        private class SyncLiveComparer : IEqualityComparer<Match>
        {
            public bool Equals(Match x, Match y) => x.AdminSiteMatchId == y.AdminSiteMatchId;
            public int GetHashCode(Match obj)=> obj.AdminSiteMatchId.GetHashCode();
        }

        public IEnumerable<Match> FilterMatchHasAdminSiteId(IEnumerable<Match> matches)
            => matches.Where(x => x.AdminSiteMatchId != null).Distinct();

        public IEnumerable<Match> GetChangedLiveAdminSiteIds(IEnumerable<Match> oldMatches, IEnumerable<Match> newMatches)
        {
            var unChangedAdminsites = newMatches.Intersect(oldMatches, new SyncLiveComparer()).ToList();
            foreach(var match in unChangedAdminsites)
            {
                var oldMatch = oldMatches.Where(x => x.AdminSiteMatchId == match.AdminSiteMatchId).FirstOrDefault();

                if (oldMatch.OfferLive != match.OfferLive)
                {
                    yield return match;
                }
            }
        }
    }
}
