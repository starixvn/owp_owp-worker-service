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
        var owpBasketballMatch = FilterBasketballMatch(owpMatches).ToList();
        var adminSiteBasketballMatch = FilterBasketballMatch(adminSiteMatches).ToList();
        var changedAdminSites = GetChangedLiveAdminSites(adminSiteBasketballMatch, owpBasketballMatch);

        return changedAdminSites;
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
                    InHouseMatchId = match.AdminSiteMatchId ?? 0,
                    IsLive = match.OfferLive
                };
            }
        }
    }

    private static IEnumerable<Match> FilterBasketballMatch(IEnumerable<Match> matches)
        => matches.Where(x => IsBasketball(x.Sport));

    private static bool IsBasketball(string sport) => sport.Trim().Equals("basketball", StringComparison.OrdinalIgnoreCase);
}