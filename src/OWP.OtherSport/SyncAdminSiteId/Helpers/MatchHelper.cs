namespace OWP.OtherSport.SyncAdminSiteId.Helpers;

using Models;

public static class MatchHelper
{
    public static IEnumerable<LinkedMatch> GetNewLinkedMatches(IList<Match> owpMatches, IList<Match> adminSiteMatches)
    {
        return owpMatches
            .Where(owpMatch => adminSiteMatches.Any(adminMatch => CompareMatch(owpMatch, adminMatch)))
            .Select(owpMatch =>
            {
                var adminSiteMatch = adminSiteMatches.First(adminMatch => CompareMatch(owpMatch, adminMatch));

                return new LinkedMatch
                {
                    OsMatchId = owpMatch.OsMatchId,
                    AdminSiteMatchId = adminSiteMatch.AdminSiteMatchId,
                    AdminSiteAway = adminSiteMatch.AwayName,
                    AdminSiteHome = adminSiteMatch.HomeName,
                    AdminSiteLeague = adminSiteMatch.LeagueName,
                    OwpAway = owpMatch.AwayName,
                    OwpHome = owpMatch.HomeName,
                    OwpLeague = owpMatch.LeagueName,
                    OldAdminSiteMatchId = owpMatch.AdminSiteMatchId,
                    OldEventDate = owpMatch.EventDateTime,
                    NewEventDate = adminSiteMatch.EventDateTime
                };
            });
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
        => (owpMatch.EventDateTime.Date == adminSiteMatch.EventDateTime.Date || owpMatch.EventDate.Date == adminSiteMatch.EventDate.Date) &&
        CheckIsEqual(owpMatch.LeagueName, adminSiteMatch.LeagueName) &&
        CheckIsEqual(owpMatch.HomeName, adminSiteMatch.HomeName) &&
        CheckIsEqual(owpMatch.AwayName, adminSiteMatch.AwayName);

    private static bool CheckIsEqual(string first, string second)
        => !string.IsNullOrEmpty(first) &&
        !string.IsNullOrEmpty(second) &&
        first.Trim().Equals(second.Trim(), StringComparison.OrdinalIgnoreCase);

    public static IEnumerable<LinkedMatch> GetRemovedMatches(IList<Match> owpMatches, IList<Match> adminSiteMatches)
    {
        return owpMatches
            .Where(owpMatch => adminSiteMatches.All(adminMatch => adminMatch.AdminSiteMatchId != owpMatch.AdminSiteMatchId))
            .Select(owpMatch => new LinkedMatch
            {
                OsMatchId = owpMatch.OsMatchId,
                OwpAway = owpMatch.AwayName,
                OwpHome = owpMatch.HomeName,
                OwpLeague = owpMatch.LeagueName,
                OldAdminSiteMatchId = owpMatch.AdminSiteMatchId,
                OldEventDate = owpMatch.EventDateTime
            });
    }
}