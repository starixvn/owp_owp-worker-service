namespace OWP.OtherSport.SyncAdminSiteId.Helpers;

using Models;

public static class MatchHelper
{
    public static IEnumerable<LinkedMatch> GetNewLinkedMatches(IList<Match> owpMatches, IList<Match> adminSiteMatches)
    {
        foreach (var match in owpMatches)
        {
            var adminSiteMatch = adminSiteMatches.FirstOrDefault(x => CompareMatch(match, x));

            if (adminSiteMatch != null)
            {
                yield return new LinkedMatch
                {
                    OsMatchId = match.OsMatchId,
                    AdminSiteMatchId = adminSiteMatch.AdminSiteMatchId,
                    AdminSiteAway = adminSiteMatch.AwayName,
                    AdminSiteHome = adminSiteMatch.HomeName,
                    AdminSiteLeague = adminSiteMatch.LeagueName,
                    OwpAway = match.AwayName,
                    OwpHome = match.HomeName,
                    OwpLeague = match.LeagueName,
                    OldAdminSiteMatchId = match.AdminSiteMatchId,
                    OldEventDate = match.EventDateTime,
                    NewEventDate = adminSiteMatch.EventDateTime
                };
            }
        }
    }

    private static bool CompareMatch(Match owpMatch, Match adminSiteMatch)
        => ConditionOfRealMatch(owpMatch, adminSiteMatch) || ConditionOfSabaMatch(owpMatch, adminSiteMatch);

    private static bool ConditionOfSabaMatch(Match owpMatch, Match adminSiteMatch)
        => IsSameEvent(owpMatch, adminSiteMatch) && 
        CheckIsEqual(owpMatch.Sport, adminSiteMatch.Sport) && 
        owpMatch.EventDateTime == adminSiteMatch.EventDateTime;

    private static bool ConditionOfRealMatch(Match owpMatch, Match adminSiteMatch)
        => IsSameEvent(owpMatch, adminSiteMatch) && !adminSiteMatch.IsSaba;

    private static bool IsSameEvent(Match owpMatch, Match adminSiteMatch)
        => owpMatch.EventDateTime.Date == adminSiteMatch.EventDateTime.Date && 
        CheckIsEqual(owpMatch.LeagueName, adminSiteMatch.LeagueName) &&
        CheckIsEqual(owpMatch.HomeName, adminSiteMatch.HomeName) &&
        CheckIsEqual(owpMatch.AwayName, adminSiteMatch.AwayName);

    private static bool CheckIsEqual(string first, string second)
        => !string.IsNullOrEmpty(first) &&
        !string.IsNullOrEmpty(second) &&
        first.Trim().Equals(second.Trim(), StringComparison.OrdinalIgnoreCase);

    public static IEnumerable<LinkedMatch> GetRemovedMatches(IList<Match> owpMatches, IList<Match> adminSiteMatches)
    {
        foreach (var match in owpMatches)
        {
            if (adminSiteMatches.All(x => x.AdminSiteMatchId != match.AdminSiteMatchId))
            {
                yield return new LinkedMatch
                {
                    OsMatchId = match.OsMatchId,
                    OwpAway = match.AwayName,
                    OwpHome = match.HomeName,
                    OwpLeague = match.LeagueName,
                    OldAdminSiteMatchId = match.AdminSiteMatchId,
                    OldEventDate = match.EventDateTime
                };
            }
        }
    }
}