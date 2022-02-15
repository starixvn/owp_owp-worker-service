using OWP.Core.Extensions;
using OWP.SyncLeagueSettings.Constants;
using OWP.SyncLeagueSettings.Helpers;
using OWP.SyncLeagueSettings.Models;
using System.Linq;
namespace OWP.SyncLeagueSettings.Services;

public interface LeagueSettingsService
{
    Task InsertNewLeagueSettings(DateTime date);

    List<LeagueSetting> CreateNewLeaguesSettings(DateTime date, IEnumerable<Match> matches, IEnumerable<TvCondition> tvConditions);
}

public class LeagueSettingsServiceImp : LeagueSettingsService
{
    private readonly LeagueSettingsRepository repository;

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

    public LeagueSettingsServiceImp(LeagueSettingsRepository repository)
    {
        this.repository = repository;
    }

    public async Task InsertNewLeagueSettings(DateTime date)
    {
        var matchesAndSettings = await repository.GetMatchesAndSettings(date);
        var matches = matchesAndSettings.Item1.Where(x => x.OWPMatchId == 777984);

        foreach (var match in matches)
        {
            match.MatchSettings = matchesAndSettings.Item2.Where(x => x.OWPMatchId == match.OWPMatchId).ToList();
        }

        var tvConditions = await repository.GetAllTvConditions();
        var leagueSettings = CreateNewLeaguesSettings(date, matches, tvConditions);

        var insertLeagueSettingsResponses = await repository.InsertNewLeaguesSettings(leagueSettings);

        foreach (var leagueSetting in insertLeagueSettingsResponses)
        {
            var leagueSettingCondition = new LeagueSettingCondition
            {
                HasTV = leagueSetting.HasTV,
                LeagueSettingId = leagueSetting.LeagueSettingId,
            };

            await repository.UpdateLeagueSettingFullCondition(leagueSettingCondition);
        }
    }

    public List<LeagueSetting> CreateNewLeaguesSettings(DateTime date, IEnumerable<Match> matches, IEnumerable<TvCondition> tvConditions)
    {
        var leagueIds = matches.Select(x => x.OWPLeagueId).Distinct();
        var monday = FindLastWeekDay(date, DayOfWeek.Monday); //12PM GMT+8
        var friday = FindLastWeekDay(date, DayOfWeek.Saturday).AddMinutes(-1); //11:59AM GMT+8
        var saturday = FindLastWeekDay(date, DayOfWeek.Saturday); //12PM GMT+8
        var sunday = FindLastWeekDay(date, DayOfWeek.Sunday).AddDays(1).AddMinutes(-1); //11:59AM GMT+8

        var weekdayMatches = matches.Where(x => MatchHelpers.IsInRangeMatch(x, monday, friday));
        var weekendMatches = matches.Where(x => MatchHelpers.IsInRangeMatch(x, saturday, sunday));

        var result = new List<LeagueSetting>();

        foreach (var leagueId in leagueIds)
        {
            var latestWeekday = MatchHelpers.FindLatestDay(weekdayMatches);
            var latestWeekend = MatchHelpers.FindLatestDay(weekendMatches);

            var weekdayFromDate = latestWeekday.AddDays(-1).AddHours(12);
            var weekdayToDate = latestWeekday.AddHours(11).AddMinutes(59);

            var weekendFromDate = latestWeekend.AddDays(-1).AddHours(12);
            var weekendToDate = latestWeekend.AddHours(11).AddMinutes(59);

            var weekdayMatchesWithTV =
                MatchHelpers.FindMatchesWithTv(weekdayMatches, leagueId, weekdayFromDate, weekdayToDate, tvConditions);
            var weekendMatchesWithTV =
                MatchHelpers.FindMatchesWithTv(weekendMatches, leagueId, weekendFromDate, weekendToDate, tvConditions);

            var weekdayMatchesWithoutTV =
                MatchHelpers.FindMatchesWithoutTv(weekdayMatches, leagueId, weekdayFromDate, weekdayToDate, tvConditions);
            var weekendMatchesWithoutTV =
                MatchHelpers.FindMatchesWithoutTv(weekendMatches, leagueId, weekendFromDate, weekendToDate, tvConditions);

            var largestMaxbetWeekdayMatchWithTv = MatchHelpers.GetMatchWithLargestMaxbet(weekdayMatchesWithTV);
            var largestMaxbetWeekendMatchWithTv = MatchHelpers.GetMatchWithLargestMaxbet(weekendMatchesWithTV);

            var largestMaxbetWeekdayMatchWithoutTv = MatchHelpers.GetMatchWithLargestMaxbet(weekdayMatchesWithoutTV);
            var largestMaxbetWeekendMatchWithoutTv = MatchHelpers.GetMatchWithLargestMaxbet(weekendMatchesWithoutTV);

            if (largestMaxbetWeekdayMatchWithTv != null || largestMaxbetWeekendMatchWithTv != null)
            {
                var withTvLeagueSettings = CreateLeagueSetting(largestMaxbetWeekdayMatchWithTv, largestMaxbetWeekendMatchWithTv, true);
                result.Add(withTvLeagueSettings);
            }

            if (largestMaxbetWeekdayMatchWithoutTv != null || largestMaxbetWeekendMatchWithoutTv != null)
            {
                var withoutTvLeagueSettings = CreateLeagueSetting(largestMaxbetWeekdayMatchWithoutTv, largestMaxbetWeekendMatchWithoutTv, false);
                result.Add(withoutTvLeagueSettings);
            }
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
        var hpd1HSpread = MatchHelpers.GetSpread(latestMatch, Bettype.HDP_1H);
        var ou1HSpread = MatchHelpers.GetSpread(latestMatch, Bettype.OU_1H);

        result.HasTv = isWithTv;
        result.OWPLeagueId = latestMatch.OWPLeagueId;
        result.Spread_HDP = MatchHelpers.GetSpread(latestMatch, Bettype.HDP);
        result.Spread_OU = MatchHelpers.GetSpread(latestMatch, Bettype.OU);
        result.Spread_1HHDP = hpd1HSpread ?? Get1HSpread(result.Spread_HDP);
        result.Spread_1HOU = ou1HSpread ?? Get1HSpread(result.Spread_OU);
        //Weekday
        result.MaxbetWD_HDP = MatchHelpers.GetMaxBet(weekdayMatch, Bettype.HDP);
        result.MaxbetWD_OU = MatchHelpers.GetMaxBet(weekdayMatch, Bettype.OU);
        result.MaxbetWD_1H = Get1HMaxbet(MatchHelpers.GetMaxBet(weekdayMatch, Bettype.HDP));
        //Weekend
        result.MaxbetWK_HDP = MatchHelpers.GetMaxBet(weekendMatch, Bettype.HDP);
        result.MaxbetWK_OU = MatchHelpers.GetMaxBet(weekendMatch, Bettype.OU);
        result.MaxbetWK_1H = Get1HMaxbet(MatchHelpers.GetMaxBet(weekendMatch, Bettype.HDP));

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

        return weekendMatch?.EventDateTime >= weekdayMatch?.EventDateTime ? weekendMatch : weekdayMatch;
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