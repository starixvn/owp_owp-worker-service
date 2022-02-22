using OWP.OtherSport.SyncAdminSiteId.Models;

namespace OWP.OtherSport.SyncAdminSiteId.Helpers;

public static class MatchHelper
{
    public static IEnumerable<LinkedMatch> GetNewLinkedMatches(IEnumerable<Match> owpMatches, IEnumerable<Match> adminSiteMatches)
    {
        foreach (var match in owpMatches)
        {
            var adminSiteMatch = adminSiteMatches.FirstOrDefault(x => x.Equals(match));

            if (adminSiteMatch != null)
            {
                yield return new LinkedMatch
                {
                    OSMatchId = match.OSMatchId,
                    AdminSiteMatchId = adminSiteMatch.AdminSiteMatchId,
                    AdminSiteAway = adminSiteMatch.AwayName,
                    AdminSiteHome = adminSiteMatch.HomeName,
                    AdminSiteLeague = adminSiteMatch.LeagueName,
                    OWPAway = match.AwayName,
                    OWPHome = match.HomeName,
                    OWPLeague = match.LeagueName,
                    OldAdminSiteMatchId = match.AdminSiteMatchId,
                    OldEventDate = match.EventDate,
                    NewEventDate = adminSiteMatch.EventDate
                };
            }
        }
    }

    public static IEnumerable<LinkedMatch> GetUpdatedMatches(IEnumerable<Match> owpMatches, IEnumerable<Match> adminSiteMatches)
    {
        foreach (var match in owpMatches)
        {
            if (!adminSiteMatches.Any(x => x.Equals(match)))
            {
                yield return new LinkedMatch
                {
                    OSMatchId = match.OSMatchId,
                    OWPAway = match.AwayName,
                    OWPHome = match.HomeName,
                    OWPLeague = match.LeagueName,
                    OldAdminSiteMatchId = match.AdminSiteMatchId,
                    OldEventDate = match.EventDate
                };
            }
        }
    }
}