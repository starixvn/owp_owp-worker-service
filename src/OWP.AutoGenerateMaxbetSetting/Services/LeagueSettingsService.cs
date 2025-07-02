namespace OWP.AutoGenerateMaxbetSetting.Services;

using Constants;
using Helpers;
using Models;
using AutoGenerateMaxbetSetting;

public interface GeneratorService
{
    Task GenerateNewLeagueSetting(DateTime date);
}

public class GeneratorServiceImp(LeagueSettingsRepository repository) : GeneratorService
{
    public async Task GenerateNewLeagueSetting(DateTime date)
    {
        var matchesAndSettings = await repository.GetMatchesAndSettings(date);
        var matches = matchesAndSettings.Item1.ToList();

        foreach (var match in matches)
        {
            match.MatchSettings = matchesAndSettings.Item2.Where(x => x.OwpMatchId == match.OwpMatchId).ToList();
        }

        var leagueSettings = CreateNewLeaguesSettings(date, matches);

        await repository.InsertNewLeaguesSettings(leagueSettings);

    }

    private static List<LeagueSetting> CreateNewLeaguesSettings(DateTime date, IList<Match> matches)
    {
        var leagueIds = matches.Select(x => x.OwpLeagueId).Distinct();
        var monday = FindLastWeekDay(date, DayOfWeek.Monday); //12PM GMT+8
        var friday = FindLastWeekDay(date, DayOfWeek.Saturday).AddMinutes(-1); //11:59AM GMT+8
        var saturday = FindLastWeekDay(date, DayOfWeek.Saturday); //12PM GMT+8
        var sunday = FindLastWeekDay(date, DayOfWeek.Sunday).AddDays(1).AddMinutes(-1); //11:59AM GMT+8

        var weekdayMatches = matches.Where(x => MatchHelpers.IsInRangeMatch(x, monday, friday)).ToList();
        var weekendMatches = matches.Where(x => MatchHelpers.IsInRangeMatch(x, saturday, sunday)).ToList();

        var result = new List<LeagueSetting>();

        foreach (var leagueId in leagueIds)
        {
            var latestWeekday = MatchHelpers.FindLatestDay(weekdayMatches);
            var latestWeekend = MatchHelpers.FindLatestDay(weekendMatches);

            var weekdayFromDate = latestWeekday.AddDays(-1).AddHours(12);
            var weekdayToDate = latestWeekday.AddHours(11).AddMinutes(59);

            var weekendFromDate = latestWeekend.AddDays(-1).AddHours(12);
            var weekendToDate = latestWeekend.AddHours(11).AddMinutes(59);

            var largestMaxbetWeekday = MatchHelpers.GetMatchWithLargestMaxbet(
                MatchHelpers.FindMatches(weekdayMatches, leagueId, weekdayFromDate, weekdayToDate));

            var largestMaxbetWeekend = MatchHelpers.GetMatchWithLargestMaxbet(
                MatchHelpers.FindMatches(weekendMatches, leagueId, weekendFromDate, weekendToDate));

            var leagueSetting = CreateLeagueSetting(largestMaxbetWeekday, largestMaxbetWeekend);
            result.Add(leagueSetting);
        }

        return result;
    }

    private static DateTime FindLastWeekDay(DateTime syncingDate, DayOfWeek day)
    {
        var result = syncingDate.AddHours(10);

        while (result.DayOfWeek != day || result.DayOfWeek == day && result.Date == syncingDate.Date)
        {
            result = result.AddDays(-1);
        }

        return result;
    }

    private static LeagueSetting CreateLeagueSetting(Match weekdayMatch, Match weekendMatch)
    {
        var latestMatch = GetLatestMatch(weekdayMatch, weekendMatch);
        var result = new LeagueSetting
        {
            OwpLeagueId = latestMatch.OwpLeagueId,
            SpreadHdp = MatchHelpers.GetSpread(latestMatch, Bettype.Hdp),
            SpreadOu = MatchHelpers.GetSpread(latestMatch, Bettype.Ou),
            //Weekday
            MaxbetWdHdp = MatchHelpers.GetMaxBet(weekdayMatch, Bettype.Hdp),
            //Weekend
            MaxbetWkHdp = MatchHelpers.GetMaxBet(weekendMatch, Bettype.Hdp)
        };

        return result;
    }

    private static Match GetLatestMatch(Match weekdayMatch, Match weekendMatch)
    {
        if (weekdayMatch is null)
        {
            return weekendMatch;
        }

        if (weekendMatch == null)
        {
            return weekdayMatch;
        }

        return weekendMatch.EventDateTime >= weekdayMatch.EventDateTime ? weekendMatch : weekdayMatch;
    }
}