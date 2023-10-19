namespace OWP.OtherSport.SyncLive.Services;

using Model;
using SyncAdminSiteId.Models;

public interface EditEventGenerator
{
    IEnumerable<EditEvent> GetChangedOfferLiveMatches(IEnumerable<Match> adminSiteMatches, IEnumerable<Match> owpMatches);
}

public class EditEventGeneratorImp : EditEventGenerator
{
    public IEnumerable<EditEvent> GetChangedOfferLiveMatches(IEnumerable<Match> adminSiteMatches, IEnumerable<Match> owpMatches)
    {
        return GetChangedLiveAdminSites(adminSiteMatches.ToList(), owpMatches.ToList());
    }

    public IEnumerable<EditEvent> GetChangedLiveAdminSites(IList<Match> adminSiteMatches, IList<Match> owpMatches)
    {
        var owpMatchHasAdminSiteId = owpMatches.Where(x => x.AdminSiteMatchId.HasValue).Distinct();
        foreach (var match in owpMatchHasAdminSiteId)
        {
            var adminSiteMatch = adminSiteMatches.FirstOrDefault(x => x.AdminSiteMatchId == match.AdminSiteMatchId);
            if (adminSiteMatch != null && adminSiteMatch.OfferLive != match.OfferLive)
            {
                yield return new EditEvent
                {
                    in_house_match_id = match.AdminSiteMatchId ?? 0,
                    is_live = match.OfferLive
                };
            }
        }
    }
}