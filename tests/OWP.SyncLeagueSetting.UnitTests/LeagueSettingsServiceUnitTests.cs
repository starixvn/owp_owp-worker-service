namespace OWP.SyncLeagueSettings.UnitTests;

using Constants;
using Models;
using NSubstitute;
using Services;
using Xunit;
using Xunit.Asserts.Compare;

public class LeagueSettingsServiceUnitTests
{
    private readonly LeagueSettingsRepository repository;

    public LeagueSettingsServiceUnitTests()
    {
        repository = Substitute.For<LeagueSettingsRepository>();
    }

    [Fact]
    public void CreateNewLeaguesSettings_ShouldReturnData()
    {
        //Arrange
        var service = InitLeagueSettingsService();
        var matches = MockMatches();
        var tv = MockTvConditions();
        foreach (var match in matches)
        {
            match.MatchSettings = MockMatchSettings().Where(x => x.OWPMatchId == match.OWPMatchId).ToList();
        }
        var date = new DateTime(2021, 04, 12, 02, 00, 00);

        //Act
        var actual = service.CreateNewLeaguesSettings(date, matches, tv).OrderBy(x => x.OWPLeagueId).ToList();

        //Assert
        var expected = MockLeagueSettings().OrderBy(x => x.OWPLeagueId).ToList();
        DeepAssert.Equal(expected, actual);
    }

    [Fact]
    public async Task InsertNewLeagueSettings_ShouldReturnData()
    {
        //Arrange
        var service = InitLeagueSettingsService();
        var date = new DateTime(2021, 04, 12, 02, 00, 00);
        var tvCondition = new List<TvCondition>
        {
            new() { Label="TV", Value="TV"},
            new() { Label="BET(TV)", Value="BET(TV)"},
        };
        var leagueSettingResponse = new List<InsertLeagueSettingsResponse>
        {
            new() { HasTV=true, LeagueSettingId=1},
            new() { HasTV=false, LeagueSettingId=2},
        };
        repository.InsertNewLeaguesSettings(Arg.Any<List<LeagueSetting>>()).ReturnsForAnyArgs(leagueSettingResponse);
        repository.GetAllTvConditions().Returns(tvCondition);

        //Act
        await service.InsertNewLeagueSettings(date);
    }

    private LeagueSettingsService InitLeagueSettingsService()
    {
        var matches = MockMatches();
        var matchSettings = MockMatchSettings();
        var queryRepository = Substitute.For<LeagueSettingsRepository>();
        queryRepository.GetMatchesAndSettings(new DateTime(2021, 04, 12, 02, 00, 00)).Returns(Tuple.Create(matches, matchSettings));
        queryRepository.GetAllTvConditions().Returns(MockTvConditions());

        var syncService = new LeagueSettingsServiceImp(queryRepository);
        return syncService;
    }

    private IEnumerable<TvCondition> MockTvConditions()
        => new List<TvCondition>
        {
            new()
            {
                Label = "Equals",
                Value = "Euro12"
            },
            new()
            {
                Label = "Equals",
                Value = "Euro61"
            },
            new()
            {
                Label = "Contains",
                Value = "Euro121"
            }
        };

    private IEnumerable<Match> MockMatches()
        => new List<Match>
        {
            new()
            {
                OWPMatchId = 738253,
                EventDateTime = new DateTime(2021, 04, 07, 15, 00, 00),
                OWPLeagueId = 4,
                TV = "Euro12",
                MatchSettings = new List<MatchSetting>()
            },
            new()
            {
                OWPMatchId = 738254,
                EventDateTime = new DateTime(2021, 04, 07, 15, 00, 00),
                OWPLeagueId = 34,
                TV = null,
                MatchSettings = new List<MatchSetting>()
            },
            new()
            {
                OWPMatchId = 738283,
                EventDateTime = new DateTime(2021, 04, 10, 15, 00, 00),
                OWPLeagueId = 15,
                TV = "Euro12",
                MatchSettings = new List<MatchSetting>()
            },
            new()
            {
                OWPMatchId = 738284,
                EventDateTime = new DateTime(2021, 04, 10, 15, 00, 00),
                OWPLeagueId = 15,
                TV = "Euro61",
                MatchSettings = new List<MatchSetting>()
            },
            new()
            {
                OWPMatchId = 738293,
                EventDateTime = new DateTime(2021, 04, 10, 15, 00, 00),
                OWPLeagueId = 54,
                TV = "testing",
                MatchSettings = new List<MatchSetting>()
            },
            new()
            {
                OWPMatchId = 738294,
                EventDateTime = new DateTime(2021, 04, 10, 15, 30, 00),
                OWPLeagueId = 54,
                TV = null,
                MatchSettings = new List<MatchSetting>()
            },
        };

    private IEnumerable<MatchSetting> MockMatchSettings()
        => new List<MatchSetting>
        {
            new()
            {
                OWPMatchId = 738253,
                Bettype = Bettype.HDP,
                Spread = 0.2m,
                Maxbet = 100000.00m
            },
            new()
            {
                OWPMatchId = 738253,
                Bettype = Bettype.OU,
                Spread = 0.15m,
                Maxbet = null
            },
            new()
            {
                OWPMatchId = 738254,
                Bettype = Bettype.HDP,
                Spread = 0.25m,
                Maxbet = 200000.00m
            },
            new()
            {
                OWPMatchId = 738254,
                Bettype = Bettype.OU,
                Spread = 0.30m,
                Maxbet = null
            },
            new()
            {
                OWPMatchId = 738283,
                Bettype = Bettype.HDP,
                Spread = 0.05m,
                Maxbet = 150000.00m
            },
            new()
            {
                OWPMatchId = 738283,
                Bettype = Bettype.OU,
                Spread = 0.05m,
                Maxbet = null
            },
            new()
            {
                OWPMatchId = 738284,
                Bettype = Bettype.HDP,
                Spread = 0.5m,
                Maxbet = 180000.00m
            },
            new()
            {
                OWPMatchId = 738284,
                Bettype = Bettype.OU,
                Spread = 0.40m,
                Maxbet = null
            },
            new()
            {
                OWPMatchId = 738293,
                Bettype = Bettype.HDP,
                Spread = 0.04m,
                Maxbet = 300000.00m
            },
            new()
            {
                OWPMatchId = 738293,
                Bettype = Bettype.OU,
                Spread = 0.04m,
                Maxbet = null
            },
            new()
            {
                OWPMatchId = 738294,
                Bettype = Bettype.HDP,
                Spread = 0.15m,
                Maxbet = 200000.00m
            },
            new()
            {
                OWPMatchId = 738294,
                Bettype = Bettype.OU,
                Spread = 0.15m,
                Maxbet = null
            }
        };

    private IList<LeagueSetting> MockLeagueSettings()
        => new List<LeagueSetting>
        {
            new()
            {
                OWPLeagueId = 4,
                HasTv = true,
                Spread_HDP = 0.2m,
                Spread_OU = 0.15m,
                Spread_1HHDP = 0.2m,
                Spread_1HOU = 0.15m,
                MaxbetWD_HDP = 100000.00m,
                MaxbetWD_1H = 50000.00m,
                MaxbetWD_OU = null,
                MaxbetWK_1H = null,
                MaxbetWK_HDP = null,
                MaxbetWK_OU = null
            },
            new()
            {
                OWPLeagueId = 15,
                HasTv = true,
                Spread_HDP = 0.50m,
                Spread_OU = 0.40m,
                Spread_1HHDP = 0.50m,
                Spread_1HOU = 0.40m,
                MaxbetWD_HDP = null,
                MaxbetWD_1H = null,
                MaxbetWD_OU = null,
                MaxbetWK_1H = 90000.00m,
                MaxbetWK_HDP = 180000.00m,
                MaxbetWK_OU = null
            },
            new()
            {
                OWPLeagueId = 34,
                HasTv = false,
                Spread_HDP = 0.25m,
                Spread_OU = 0.30m,
                Spread_1HHDP = 0.25m,
                Spread_1HOU = 0.30m,
                MaxbetWD_HDP = 200000.00m,
                MaxbetWD_1H = 100000.00m
            },
            new()
            {
                OWPLeagueId = 54,
                HasTv = false,
                Spread_HDP = 0.04m,
                Spread_OU = 0.04m,
                Spread_1HHDP = 0.10m,
                Spread_1HOU = 0.10m,
                MaxbetWK_HDP = 300000.00m,
                MaxbetWK_1H = 300000.00m
            }
        };
}