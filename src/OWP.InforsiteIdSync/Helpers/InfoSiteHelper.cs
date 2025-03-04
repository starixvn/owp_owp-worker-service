namespace OWP.InfositeIdSync.Helpers;

using Extensions;
using Models;
using System.Globalization;

public static class InfoSiteHelper
{
    public static List<MatchMappingWithInfosite> Map(List<OwpMatch> owpMatches, List<InfositeMatch> infositeMatches)
    {
        var leagueDict = infositeMatches.GroupBy(i => i.LeagueName.Trim().ToLower(CultureInfo.InvariantCulture))
            .ToDictionary(i => i.Key, i => i.ToList());

        var emptyMatchList = new List<InfositeMatch>();

        return owpMatches
            .Select(m => Map(m, leagueDict.GetValue(m.League.Trim().ToLower(CultureInfo.InvariantCulture), emptyMatchList)))
            .SelectMany(m => m)
            .ToList();
    }

    public static List<MatchMappingWithInfosite> Map(OwpMatch owpMatch, List<InfositeMatch> infositeMatches)
    {
        var matchesWithSameLeagueHomeAndAwayKoTime = MapMatchWithSameLeagueHomeAndAwayKoTime(owpMatch, infositeMatches);

        if (matchesWithSameLeagueHomeAndAwayKoTime.Count == 1)
        {
            return matchesWithSameLeagueHomeAndAwayKoTime;
        }

        var matchesWithSameLeagueKickoffTimeAndAnyTeam = MapMatchWithSameLeagueKickoffTimeAndSwapTeam(owpMatch, infositeMatches);
        return matchesWithSameLeagueKickoffTimeAndAnyTeam.Count == 1
            ? matchesWithSameLeagueKickoffTimeAndAnyTeam
            : [];
    }


    public static List<MatchMappingWithInfosite> MapMatchWithSameLeagueHomeAndAwayKoTime(OwpMatch owpMatch, List<InfositeMatch> infositeMatches)
        => infositeMatches.Where(m =>
            CompareOrdinalIgnoreCase(m.LeagueName, owpMatch.League) &&
            CompareOrdinalIgnoreCase(m.HomeName, owpMatch.Home) &&
            CompareOrdinalIgnoreCase(m.AwayName, owpMatch.Away) &&
            m.EventDateTime == owpMatch.EventDateTime)
        .Select(m => new MatchMappingWithInfosite(owpMatch.OwpMatchId, m.InfoSiteMatchId, m.AdminSiteMatchId, owpMatch.EventDateTime))
        .ToList();

    public static List<MatchMappingWithInfosite> MapMatchWithSameLeagueKickoffTimeAndSwapTeam(OwpMatch owpMatch, List<InfositeMatch> infositeMatches)
        => infositeMatches.Where(m => IsSameLeagueKickoffTimeAndSwapTeam(owpMatch, m))
           .Select(m => new MatchMappingWithInfosite(owpMatch.OwpMatchId, m.InfoSiteMatchId, m.AdminSiteMatchId, owpMatch.EventDateTime))
           .ToList();

    public static bool IsSameLeagueKickoffTimeAndSwapTeam(OwpMatch owpMatch, InfositeMatch infositeMatch)
        => CompareOrdinalIgnoreCase(owpMatch.League, infositeMatch.LeagueName)
            && owpMatch.EventDateTime.ToShortTimeString() == infositeMatch.EventDateTime.ToShortTimeString()
            && CompareOrdinalIgnoreCase(owpMatch.Home, infositeMatch.AwayName)
            && CompareOrdinalIgnoreCase(owpMatch.Away, infositeMatch.HomeName);

    private static bool CompareOrdinalIgnoreCase(string text1, string text2)
        => string.Compare(text1.Trim(), text2.Trim(), StringComparison.OrdinalIgnoreCase) == 0;
}