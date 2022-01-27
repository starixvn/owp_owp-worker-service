using OWP.Core.Extensions;
using OWP.SyncLeagueSettings.Models;

namespace OWP.SyncLeagueSettings.Helpers;

public static class MatchHelpers
{
    public static bool IsInRangeMatch(Match match, DateTime from, DateTime to)
        => match.EventDateTime >= from && match.EventDateTime <= to;

    public static DateTime GetMatchTime(Match match)
        => match.EventDateTime.Hour >= 12 ? match.EventDateTime.Date.AddDays(1) : match.EventDateTime.Date;

    public static DateTime FindLatestDay(IEnumerable<Match> matches)
        => matches.Any() ? matches.Select(GetMatchTime).Max() : new DateTime(2021, 01, 01);

    public static List<Match> FindMatchesWithTv(IEnumerable<Match> matches, int leagueId, DateTime from, DateTime to, IEnumerable<TvCondition> tvConditions)
        => matches.Where(x => x.OWPLeagueId == leagueId && x.EventDateTime >= from && x.EventDateTime <= to && IsWithTv(x, tvConditions)).ToList();

    public static List<Match> FindMatchesWithoutTv(IEnumerable<Match> matches, int leagueId, DateTime from, DateTime to, IEnumerable<TvCondition> tvConditions)
        => matches.Where(x => x.OWPLeagueId == leagueId && x.EventDateTime >= from && x.EventDateTime <= to && !IsWithTv(x, tvConditions)).ToList();

    public static decimal? GetSpread(Match match, int betType)
        => match.MatchSettings?.FirstOrDefault(x => x.Bettype == betType)?.Spread;

    public static decimal? GetMaxBet(Match match, int betType)
        => match?.MatchSettings.FirstOrDefault(x => x.Bettype == betType)?.Maxbet;

    public static Match GetMatchWithLargestMaxbet(IList<Match> matches)
    {
        if (!matches.Any())
        {
            return null;
        }

        var result = matches.FirstOrDefault();
        var resultMaxbet = result.MatchSettings.Max(x => x.Maxbet);

        for (var i = 1; i < matches.Count(); i++)
        {
            var maxbet = matches[i].MatchSettings.Max(x => x.Maxbet);

            if (maxbet > resultMaxbet)
            {
                result = matches[i];
                resultMaxbet = maxbet;
            }
        }

        return result;
    }

    public static bool IsWithTv(Match match, IEnumerable<TvCondition> tvConditions)
    {
        const string equalCondition = "Equals";
        const string containsCondition = "Contains";

        if (string.IsNullOrEmpty(match.TV))
        {
            return false;
        }

        var tvs = match.TV.Split(',');

        foreach (var condition in tvConditions)
        {
            if (condition.Label == equalCondition && tvs.Any(x => x == condition.Value))
            {
                return true;
            }

            if (condition.Label == containsCondition && tvs.Any(x => x.IsContainsIgnoreCase(condition.Value)))
            {
                return true;
            }
        }

        return false;
    }
}