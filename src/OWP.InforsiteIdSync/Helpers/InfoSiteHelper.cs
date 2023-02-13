namespace OWP.InfositeIdSync.Helpers;

using System.Globalization;
using Extensions;
using Models;

public static class InfoSiteHelper
{
    public static List<MatchMappingWithInfosite> Map(List<OWPMatch> owpMatches, List<InfositeMatch> infositeMatches)
    {
        var leagueDict = infositeMatches.GroupBy(i => i.LeagueName.Trim().ToLower(CultureInfo.InvariantCulture))
            .ToDictionary(i => i.Key, i => i.ToList());

        var emptyMatchList = new List<InfositeMatch>();

        return owpMatches
            .Select(m => Map(m, leagueDict.GetValue(m.League.Trim().ToLower(CultureInfo.InvariantCulture), emptyMatchList)))
            .SelectMany(m => m)
            .ToList();
    }

    public static List<MatchMappingWithInfosite> Map(OWPMatch owpMatch, List<InfositeMatch> infositeMatches)
    {
        var matchesWithSameLeagueHomeAndAway = IsSabaMatch(owpMatch)
            ? MapMatchWithSameLeagueHomeAndAwayKOTime(owpMatch, infositeMatches)
            : MapMatchWithSameLeagueHomeAndAway(owpMatch, infositeMatches);

        if (matchesWithSameLeagueHomeAndAway.Count == 1)
        {
            return matchesWithSameLeagueHomeAndAway;
        }

        var matchesWithSameLeagueKickoffTimeAndAnyTeam = MapMatchWithSameLeagueKickoffTimeAndSwapTeam(owpMatch, infositeMatches);
        if (matchesWithSameLeagueKickoffTimeAndAnyTeam.Count == 1)
        {
            return matchesWithSameLeagueKickoffTimeAndAnyTeam;
        }

        return new List<MatchMappingWithInfosite>();
    }

    public static List<MatchMappingWithInfosite> MapMatchWithSameLeagueHomeAndAway(OWPMatch owpMatch, List<InfositeMatch> infositeMatches)
        => infositeMatches.Where(m =>
            CompareOrdinalIgnoreCase(m.LeagueName, owpMatch.League) &&
            CompareOrdinalIgnoreCase(m.HomeName, owpMatch.Home) &&
            CompareOrdinalIgnoreCase(m.AwayName, owpMatch.Away))
        .Select(m => new MatchMappingWithInfosite(owpMatch.OWPMatchId, m.InfoSiteMatchId, m.AdminSiteMatchId, owpMatch.EventDateTime))
        .ToList();

    public static List<MatchMappingWithInfosite> MapMatchWithSameLeagueHomeAndAwayKOTime(OWPMatch owpMatch, List<InfositeMatch> infositeMatches)
        => infositeMatches.Where(m =>
            CompareOrdinalIgnoreCase(m.LeagueName, owpMatch.League) &&
            CompareOrdinalIgnoreCase(m.HomeName, owpMatch.Home) &&
            CompareOrdinalIgnoreCase(m.AwayName, owpMatch.Away) &&
            m.EventDateTime == owpMatch.EventDateTime)
        .Select(m => new MatchMappingWithInfosite(owpMatch.OWPMatchId, m.InfoSiteMatchId, m.AdminSiteMatchId, owpMatch.EventDateTime))
        .ToList();

    public static List<MatchMappingWithInfosite> MapMatchWithSameLeagueKickoffTimeAndSwapTeam(OWPMatch owpMatch, List<InfositeMatch> infositeMatches)
        => infositeMatches.Where(m => IsSameLeagueKickoffTimeAndSwapTeam(owpMatch, m))
           .Select(m => new MatchMappingWithInfosite(owpMatch.OWPMatchId, m.InfoSiteMatchId, m.AdminSiteMatchId, owpMatch.EventDateTime))
           .ToList();

    public static bool IsSameLeagueKickoffTimeAndSwapTeam(OWPMatch owpMatch, InfositeMatch infositeMatch)
        => CompareOrdinalIgnoreCase(owpMatch.League, infositeMatch.LeagueName)
            && owpMatch.EventDateTime.ToShortTimeString() == infositeMatch.EventDateTime.ToShortTimeString()
            && CompareOrdinalIgnoreCase(owpMatch.Home, infositeMatch.AwayName)
            && CompareOrdinalIgnoreCase(owpMatch.Away, infositeMatch.HomeName);

    private static bool IsSabaMatch(OWPMatch match)
        => match.League.Contains("SABA ", StringComparison.CurrentCultureIgnoreCase);

    private static bool CompareOrdinalIgnoreCase(string text1, string text2)
        => string.Compare(text1.Trim(), text2.Trim(), StringComparison.OrdinalIgnoreCase) == 0;
}