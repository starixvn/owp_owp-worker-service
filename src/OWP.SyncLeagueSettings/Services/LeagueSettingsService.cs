namespace OWP.SyncLeagueSettings.Services;

using Constants;
using Extensions;
using Helpers;
using Models;

public interface LeagueSettingsService
{
    Task InsertNewLeagueSettings(DateTime date);

    List<LeagueSetting> CreateNewLeaguesSettings(DateTime date, IList<Match> matches, IList<TvCondition> tvConditions);
}

public class LeagueSettingsServiceImp(LeagueSettingsRepository repository) : LeagueSettingsService
{
    private static readonly Dictionary<decimal, decimal> MaxBetSetting =
        new()
        {
            { 400000, 200000 },
            { 350000, 180000 },
            { 200000, 100000 },
            { 180000, 90000 },
            { 150000, 60000 },
            { 130000, 50000 },
            { 120000, 50000 },
            { 110000, 50000 },
            { 100000, 50000 },
            { 90000, 40000 },
            { 80000, 40000 },
            { 70000, 40000 },
            { 60000, 40000 },
            { 50000, 30000 },
            { 40000, 25000 },
            { 35000, 20000 },
            { 30000, 18000 },
            { 25000, 15000 },
            { 20000, 12000 },
            { 18000, 10000 },
            { 15000, 10000 },
            { 12000, 8000 },
            { 10000, 6000 },
            { 8000, 5000 },
            { 7000, 4000 },
            { 6000, 4000 },
            { 5000, 3000 },
            { 4000, 2000 },
            { 3000, 2000 },
            { 2000, 2000 }
        };

    public async Task InsertNewLeagueSettings(DateTime date)
    {
        var matchesAndSettings = await repository.GetMatchesAndSettings(date);
        var matches = matchesAndSettings.Item1.ToList();

        foreach (var match in matches)
        {
            match.MatchSettings = matchesAndSettings.Item2.Where(x => x.OwpMatchId == match.OwpMatchId).ToList();
        }

        var tvConditions = (await repository.GetAllTvConditions()).ToList();
        var leagueSettings = CreateNewLeaguesSettings(date, matches, tvConditions);

        var insertLeagueSettingsResponses = await repository.InsertNewLeaguesSettings(leagueSettings);

        foreach (var leagueSetting in insertLeagueSettingsResponses)
        {
            var leagueSettingCondition = new LeagueSettingCondition
            {
                HasTv = leagueSetting.HasTv,
                LeagueSettingId = leagueSetting.LeagueSettingId
            };

            await repository.UpdateLeagueSettingFullCondition(leagueSettingCondition);
        }
    }

    public List<LeagueSetting> CreateNewLeaguesSettings(DateTime date, IList<Match> matches, IList<TvCondition> tvConditions)
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

            var weekdayMatchesWithTv =
                MatchHelpers.FindMatchesWithTv(weekdayMatches, leagueId, weekdayFromDate, weekdayToDate, tvConditions);
            var weekendMatchesWithTv =
                MatchHelpers.FindMatchesWithTv(weekendMatches, leagueId, weekendFromDate, weekendToDate, tvConditions);

            var weekdayMatchesWithoutTv =
                MatchHelpers.FindMatchesWithoutTv(weekdayMatches, leagueId, weekdayFromDate, weekdayToDate, tvConditions);
            var weekendMatchesWithoutTv =
                MatchHelpers.FindMatchesWithoutTv(weekendMatches, leagueId, weekendFromDate, weekendToDate, tvConditions);

            var largestMaxbetWeekdayMatchWithTv = MatchHelpers.GetMatchWithLargestMaxbet(weekdayMatchesWithTv);
            var largestMaxbetWeekendMatchWithTv = MatchHelpers.GetMatchWithLargestMaxbet(weekendMatchesWithTv);

            var largestMaxbetWeekdayMatchWithoutTv = MatchHelpers.GetMatchWithLargestMaxbet(weekdayMatchesWithoutTv);
            var largestMaxbetWeekendMatchWithoutTv = MatchHelpers.GetMatchWithLargestMaxbet(weekendMatchesWithoutTv);

            if (largestMaxbetWeekdayMatchWithTv != null || largestMaxbetWeekendMatchWithTv != null)
            {
                var withTvLeagueSettings = CreateLeagueSetting(largestMaxbetWeekdayMatchWithTv, largestMaxbetWeekendMatchWithTv, true);
                result.Add(withTvLeagueSettings);
            }

            if (largestMaxbetWeekdayMatchWithoutTv == null && largestMaxbetWeekendMatchWithoutTv == null)
            {
                continue;
            }

            var withoutTvLeagueSettings = CreateLeagueSetting(largestMaxbetWeekdayMatchWithoutTv, largestMaxbetWeekendMatchWithoutTv, false);
            result.Add(withoutTvLeagueSettings);
        }

        return result;
    }

    private static DateTime FindLastWeekDay(DateTime syncingDate, DayOfWeek day)
    {
        var result = syncingDate.AddHours(10);

        while (result.DayOfWeek != day || (result.DayOfWeek == day && result.Date == syncingDate.Date))
        {
            result = result.AddDays(-1);
        }

        return result;
    }

    private static LeagueSetting CreateLeagueSetting(Match weekdayMatch, Match weekendMatch, bool isWithTv)
    {
        var latestMatch = GetLatestMatch(weekdayMatch, weekendMatch);
        var result = new LeagueSetting();
        var hpd1HSpread = MatchHelpers.GetSpread(latestMatch, Bettype.Hdp1H);
        var ou1HSpread = MatchHelpers.GetSpread(latestMatch, Bettype.Ou1H);

        result.HasTv = isWithTv;
        result.OwpLeagueId = latestMatch.OwpLeagueId;
        result.SpreadHdp = MatchHelpers.GetSpread(latestMatch, Bettype.Hdp);
        result.SpreadOu = MatchHelpers.GetSpread(latestMatch, Bettype.Ou);
        result.Spread1Hhdp = hpd1HSpread ?? Get1HSpread(result.SpreadHdp);
        result.Spread1Hou = ou1HSpread ?? Get1HSpread(result.SpreadOu);
        //Weekday
        result.MaxbetWdHdp = MatchHelpers.GetMaxBet(weekdayMatch, Bettype.Hdp);
        result.MaxbetWdOu = MatchHelpers.GetMaxBet(weekdayMatch, Bettype.Ou);
        result.MaxbetWd1H = Get1HMaxbet(MatchHelpers.GetMaxBet(weekdayMatch, Bettype.Hdp));
        //Weekend
        result.MaxbetWkHdp = MatchHelpers.GetMaxBet(weekendMatch, Bettype.Hdp);
        result.MaxbetWkOu = MatchHelpers.GetMaxBet(weekendMatch, Bettype.Ou);
        result.MaxbetWk1H = Get1HMaxbet(MatchHelpers.GetMaxBet(weekendMatch, Bettype.Hdp));

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

    private static decimal? Get1HSpread(decimal? ftSpread)
    {
        const decimal defaultSpreadValue = 0.1m;

        if (ftSpread.HasValue)
        {
            return ftSpread.Value < defaultSpreadValue ? defaultSpreadValue : ftSpread;
        }

        return null;
    }

    private static decimal? Get1HMaxbet(decimal? ftMaxbet)
    {
        if (!ftMaxbet.HasValue)
        {
            return null;
        }

        return MaxBetSetting.GetValue(ftMaxbet.Value, ftMaxbet.Value);
    }
}