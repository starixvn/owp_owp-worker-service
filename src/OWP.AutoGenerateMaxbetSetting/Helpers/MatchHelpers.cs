namespace OWP.AutoGenerateMaxbetSetting.Helpers;

using Models;

public static class MatchHelpers
{
    public static bool IsInRangeMatch(Match match, DateTime from, DateTime to)
        => match.EventDateTime >= from && match.EventDateTime <= to;

    public static DateTime GetMatchTime(Match match)
        => match.EventDateTime.Hour >= 12 ? match.EventDateTime.Date.AddDays(1) : match.EventDateTime.Date;

    public static DateTime FindLatestDay(IList<Match> matches)
        => matches.Any() ? matches.Select(GetMatchTime).Max() : new DateTime(2021, 01, 01);

   
    public static List<Match> FindMatches(IEnumerable<Match> matches, int leagueId, DateTime from, DateTime to)
        => matches.Where(x => x.OwpLeagueId == leagueId && x.EventDateTime >= from && x.EventDateTime <= to).ToList();

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
}