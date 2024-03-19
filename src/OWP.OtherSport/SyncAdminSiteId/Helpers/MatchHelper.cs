namespace OWP.OtherSport.SyncAdminSiteId.Helpers;

using Models;

public static class MatchHelper
{
    public static IEnumerable<LinkedMatch> GetNewLinkedMatches(IList<Match> owpMatches, IList<Match> adminSiteMatches)
    {
        var linkedMatches = new List<LinkedMatch>();
        linkedMatches.AddRange(GetNewLinkedMatches_Rule_1_2(owpMatches, adminSiteMatches));
        linkedMatches.AddRange(GetNewLinkedMatches_Rule_3(owpMatches, adminSiteMatches));
        linkedMatches.AddRange(GetNewLinkedMatches_Rule_4(owpMatches, adminSiteMatches));
        linkedMatches.AddRange(GetNewLinkedMatches_Rule_5(owpMatches, adminSiteMatches));

        return linkedMatches;
    }

    private static IList<LinkedMatch> GetNewLinkedMatches_Rule_1_2(IList<Match> owpMatches, IList<Match> adminSiteMatches)
    => GetNewLinkedMatchesCommon(owpMatches, adminSiteMatches, CompareMatch_Rule_1_2);
    private static IList<LinkedMatch> GetNewLinkedMatches_Rule_3(IList<Match> owpMatches, IList<Match> adminSiteMatches)
    => GetNewLinkedMatchesCommon(owpMatches, adminSiteMatches, CompareMatch_Rule_3);
    private static IList<LinkedMatch> GetNewLinkedMatches_Rule_4(IList<Match> owpMatches, IList<Match> adminSiteMatches)
    {
        var linkedMatches = owpMatches?
            .Where(owpMatch => !owpMatch.IsLinked && adminSiteMatches.Count(adminMatch => CompareMatch_Rule_4(owpMatch, adminMatch)) == 1)
            .Select(owpMatch =>
            {
                var adminSiteMatch = adminSiteMatches.First(adminMatch => CompareMatch_Rule_4(owpMatch, adminMatch));
                owpMatch.IsLinked = true;
                return CreateLinkedMatchObject(owpMatch, adminSiteMatch);
            }).ToList();
        return linkedMatches ?? new List<LinkedMatch>();
    }
    private static IList<LinkedMatch> GetNewLinkedMatches_Rule_5(IList<Match> owpMatches, IList<Match> adminSiteMatches)
    => GetNewLinkedMatchesCommon(owpMatches, adminSiteMatches, CompareMatch_Rule_5);

    private static IList<LinkedMatch> GetNewLinkedMatchesCommon(IList<Match> owpMatches, IList<Match> adminSiteMatches, Func<Match, Match, bool> funcCondition)
    {
        var linkedMatches = owpMatches?
            .Where(owpMatch => !owpMatch.IsLinked && adminSiteMatches.Any(adminMatch => funcCondition.Invoke(owpMatch, adminMatch)))
            .Select(owpMatch =>
            {
                var adminSiteMatch = adminSiteMatches.First(adminMatch => funcCondition.Invoke(owpMatch, adminMatch));
                owpMatch.IsLinked = true;
                return CreateLinkedMatchObject(owpMatch, adminSiteMatch);
            }).ToList();
        return linkedMatches ?? new List<LinkedMatch>();
    }

    private static LinkedMatch CreateLinkedMatchObject(Match owpMatch, Match adminSiteMatch)
   => new()
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

    private static bool CompareMatch_Rule_3(Match owpMatch, Match adminSiteMatch)
        => (CheckIsEqual(owpMatch.HomeName, adminSiteMatch.HomeName) ||
         CheckIsEqual(owpMatch.AwayName, adminSiteMatch.AwayName)) &&
          owpMatch.EventDateTime == adminSiteMatch.EventDateTime
          && !adminSiteMatch.IsSaba && !owpMatch.IsSaba;

    private static bool CompareMatch_Rule_4(Match owpMatch, Match adminSiteMatch)
    => (CheckContains(adminSiteMatch.HomeName, owpMatch.HomeName) ||
    CheckContains(adminSiteMatch.AwayName, owpMatch.AwayName)) &&
    owpMatch.LeagueName.Equals(adminSiteMatch.LeagueName, StringComparison.OrdinalIgnoreCase) &&
       owpMatch.EventDateTime == adminSiteMatch.EventDateTime
       && !adminSiteMatch.IsSaba && !owpMatch.IsSaba;

    private static bool CompareMatch_Rule_5(Match owpMatch, Match adminSiteMatch)
     => (CheckEndWith(owpMatch.LeagueName, "Home")
     && CheckContains(adminSiteMatch.HomeName, GetTargetValueInTeamName(owpMatch.HomeName))
     && CheckIsEqual(adminSiteMatch.AwayName, "N/A")
     && owpMatch.EventDateTime == adminSiteMatch.EventDateTime
     && CheckIsEqual(adminSiteMatch.LeagueName, "*NBA - Most Points Scored")
     && CheckIsEqual(owpMatch.LeagueName, "*NBA - Most Points Scored Home")
     && !adminSiteMatch.IsSaba && !owpMatch.IsSaba) ||
     (CheckEndWith(owpMatch.LeagueName, "Away")
     && CheckContains(adminSiteMatch.AwayName, GetTargetValueInTeamName(owpMatch.AwayName))
     && CheckIsEqual(adminSiteMatch.HomeName, "N/A")
     && owpMatch.EventDateTime == adminSiteMatch.EventDateTime
     && CheckIsEqual(adminSiteMatch.LeagueName, "*NBA - Most Points Scored")
    && CheckIsEqual(owpMatch.LeagueName, "*NBA - Most Points Scored Away")
     && !adminSiteMatch.IsSaba && !owpMatch.IsSaba);


    private static bool CompareMatch_Rule_1_2(Match owpMatch, Match adminSiteMatch)
           => ConditionOfRealMatch(owpMatch, adminSiteMatch) || ConditionOfSabaMatch(owpMatch, adminSiteMatch);

    private static string GetTargetValueInTeamName(string source)
    => CheckContains(source, "(")
    ? source.Split("(")[^2].Trim().Split(" ")[^1].Trim()
    : string.Empty;

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

    private static bool CheckEndWith(string source, string endsWith)
  => !string.IsNullOrEmpty(source) &&
    source.Trim().EndsWith(endsWith, StringComparison.OrdinalIgnoreCase);

    private static bool CheckContains(string parent, string child)
        => !string.IsNullOrEmpty(parent) &&
        !string.IsNullOrEmpty(child) &&
        parent.Trim().Contains(child.Trim(), StringComparison.OrdinalIgnoreCase);

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