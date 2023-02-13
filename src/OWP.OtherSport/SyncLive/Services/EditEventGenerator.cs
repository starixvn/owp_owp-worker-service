using OWP.OtherSport.SyncAdminSiteId.Models;
using OWP.OtherSport.SyncLive.Model;

namespace OWP.OtherSport.SyncLive.Services;

public interface EditEventGenerator
{
    IEnumerable<EditEvent> GetChangedOfferLiveMatches(IEnumerable<Match> adminSiteMatches, IEnumerable<Match> owpMatches);
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
            var adminsiteMatch = adminSiteMatches.FirstOrDefault(x => x.AdminSiteMatchId == match.AdminSiteMatchId);
            if (adminsiteMatch != null && adminsiteMatch.OfferLive != match.OfferLive)
            {
                yield return new EditEvent { 
                    in_house_match_id = (int)match.AdminSiteMatchId, 
                    is_live = match.OfferLive 
                };
            }
        }
    }

    private static IEnumerable<Match> FilterBasketballMatch(IEnumerable<Match> matches)
        => matches.Where(x => IsBasketball(x.Sport));

    private static bool IsBasketball(string sport) => sport.Trim().Equals("basketball", StringComparison.OrdinalIgnoreCase);
}