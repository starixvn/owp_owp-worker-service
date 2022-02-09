using OWP.Core.Extensions;
using OWP.InfositeIdSync.Models;
using Newtonsoft.Json;

namespace OWP.InfositeIdSync.Services;

public interface InfositeIdMappingService
{
    List<MatchMappingWithInfosite> Map(List<OWPMatch> owpMatches, List<InfositeMatch> infositeMatches);
}

public class InfositeIdMappingServiceImp : InfositeIdMappingService
{
    public List<MatchMappingWithInfosite> Map(List<OWPMatch> owpMatches, List<InfositeMatch> infositeMatches)
    {
        var leagueDict = infositeMatches.GroupBy(i => i.LeagueName.ToLower())
            .ToDictionary(i => i.Key, i => i.ToList());

        var emptyMatchList = new List<InfositeMatch>();

        return owpMatches
            .Select(m => Map(m, leagueDict.GetValue(m.League.ToLower(), emptyMatchList)))
            .SelectMany(m => m)
            .ToList();
    }

    public List<MatchMappingWithInfosite> Map(OWPMatch owpMatch, List<InfositeMatch> infositeMatches)
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

    public List<MatchMappingWithInfosite> MapMatchWithSameLeagueHomeAndAway(OWPMatch owpMatch, List<InfositeMatch> infositeMatches)
        => infositeMatches.Where(m =>
            string.Compare(m.LeagueName, owpMatch.League, StringComparison.OrdinalIgnoreCase) == 0 &&
            string.Compare(m.HomeName, owpMatch.Home, StringComparison.OrdinalIgnoreCase) == 0 &&
            string.Compare(m.AwayName, owpMatch.Away, StringComparison.OrdinalIgnoreCase) == 0)
        .Select(m => new MatchMappingWithInfosite(owpMatch.OWPMatchId, m.InfoSiteMatchId, m.AdminSiteMatchId, owpMatch.EventDateTime))
        .ToList();

    public static List<MatchMappingWithInfosite> MapMatchWithSameLeagueHomeAndAwayKOTime(OWPMatch owpMatch, List<InfositeMatch> infositeMatches)
        => infositeMatches.Where(m =>
            string.Compare(m.LeagueName, owpMatch.League, StringComparison.OrdinalIgnoreCase) == 0 &&
            string.Compare(m.HomeName, owpMatch.Home, StringComparison.OrdinalIgnoreCase) == 0 &&
            string.Compare(m.AwayName, owpMatch.Away, StringComparison.OrdinalIgnoreCase) == 0 &&
            m.EventDateTime.TimeOfDay == owpMatch.EventDateTime.TimeOfDay)
        .Select(m => new MatchMappingWithInfosite(owpMatch.OWPMatchId, m.InfoSiteMatchId, m.AdminSiteMatchId, owpMatch.EventDateTime))
        .ToList();

    public List<MatchMappingWithInfosite> MapMatchWithSameLeagueKickoffTimeAndSwapTeam(OWPMatch owpMatch, List<InfositeMatch> infositeMatches)
        => infositeMatches.Where(m => IsSameLeagueKickoffTimeAndSwapTeam(owpMatch, m))
           .Select(m => new MatchMappingWithInfosite(owpMatch.OWPMatchId, m.InfoSiteMatchId, m.AdminSiteMatchId, owpMatch.EventDateTime))
           .ToList();

    public bool IsSameLeagueKickoffTimeAndSwapTeam(OWPMatch owpMatch, InfositeMatch infositeMatch)
        => string.Compare(owpMatch.League, infositeMatch.LeagueName, StringComparison.OrdinalIgnoreCase) == 0
            && owpMatch.EventDateTime.ToShortTimeString() == infositeMatch.EventDateTime.ToShortTimeString()
            && string.Compare(owpMatch.Home, infositeMatch.AwayName, StringComparison.OrdinalIgnoreCase) == 0
            && string.Compare(owpMatch.Away, infositeMatch.HomeName, StringComparison.OrdinalIgnoreCase) == 0;

    private static bool IsSabaMatch(OWPMatch match)
        => match.League.Contains("SABA ", StringComparison.CurrentCultureIgnoreCase);
}