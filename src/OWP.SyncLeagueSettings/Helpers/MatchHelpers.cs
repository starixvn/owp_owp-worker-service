namespace OWP.SyncLeagueSettings.Helpers;

using Extensions;
using Models;

public static class MatchHelpers
{
    public static bool IsInRangeMatch(Match match, DateTime from, DateTime to)
        => match.EventDateTime >= from && match.EventDateTime <= to;

    public static DateTime GetMatchTime(Match match)
        => match.EventDateTime.Hour >= 12 ? match.EventDateTime.Date.AddDays(1) : match.EventDateTime.Date;

    public static DateTime FindLatestDay(IList<Match> matches)
        => matches.Any() ? matches.Select(GetMatchTime).Max() : new DateTime(2021, 01, 01);

    public static List<Match> FindMatchesWithTv(IEnumerable<Match> matches, int leagueId, DateTime from, DateTime to, IEnumerable<TvCondition> tvConditions)
        => matches.Where(x => x.OwpLeagueId == leagueId && x.EventDateTime >= from && x.EventDateTime <= to && IsWithTv(x, tvConditions)).ToList();

    public static List<Match> FindMatchesWithoutTv(IEnumerable<Match> matches, int leagueId, DateTime from, DateTime to, IEnumerable<TvCondition> tvConditions)
        => matches.Where(x => x.OwpLeagueId == leagueId && x.EventDateTime >= from && x.EventDateTime <= to && !IsWithTv(x, tvConditions)).ToList();

    public static decimal? GetSpread(Match match, int betType)
        => match.MatchSettings?.FirstOrDefault(x => x.Bettype == betType)?.Spread;

    public static decimal? GetMaxBet(Match match, int betType)
        => match?.MatchSettings.FirstOrDefault(x => x.Bettype == betType)?.Maxbet;

    public static Match GetMatchWithLargestMaxbet(IList<Match> matches)
    {
        var firstMatch = matches.FirstOrDefault();
        if (firstMatch == null)
        {
            return null;
        }

        var resultMaxbet = firstMatch.MatchSettings.Max(x => x.Maxbet);

        for (var i = 1; i < matches.Count; i++)
        {
            var maxbet = matches[i].MatchSettings.Max(x => x.Maxbet);

            if (!(maxbet > resultMaxbet))
            {
                continue;
            }

            firstMatch = matches[i];
            resultMaxbet = maxbet;
        }

        return firstMatch;
    }

    public static bool IsWithTv(Match match, IEnumerable<TvCondition> tvConditions)
    {
        const string equalCondition = "Equals";
        const string containsCondition = "Contains";

        if (string.IsNullOrEmpty(match.Tv))
        {
            return false;
        }

        var tvs = match.Tv.Split(',');

        foreach (var condition in tvConditions)
        {
            switch (condition.Label)
            {
                case equalCondition when tvs.Any(x => x == condition.Value):
                case containsCondition when tvs.Any(x => x.IsContainsIgnoreCase(condition.Value)):
                    return true;
                default: continue;
            }
        }

        return false;
    }
}