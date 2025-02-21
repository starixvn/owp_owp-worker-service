namespace OWP.SyncLeagueSetting.UnitTests;

using NSubstitute;
using OWP.SyncLeagueSettings.Constants;
using OWP.SyncLeagueSettings.Services;
using SyncLeagueSettings;
using SyncLeagueSettings.Models;
using Xunit;
using Xunit.Asserts.Compare;

public class LeagueSettingsServiceUnitTests
{
    private readonly LeagueSettingsRepository repository = Substitute.For<LeagueSettingsRepository>();

    [Fact]
    public void CreateNewLeaguesSettings_ShouldReturnData()
    {
        //Arrange
        var service = InitLeagueSettingsService();
        var matches = MockMatches();
        var tv = MockTvConditions();
        foreach (var match in matches)
        {
            match.MatchSettings = MockMatchSettings().Where(x => x.OwpMatchId == match.OwpMatchId).ToList();
        }
        var date = new DateTime(2021, 04, 12, 02, 00, 00);

        //Act
        var actual = service.CreateNewLeaguesSettings(date, matches, tv).OrderBy(x => x.OwpLeagueId).ToList();

        //Assert
        var expected = MockLeagueSettings().OrderBy(x => x.OwpLeagueId).ToList();
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
            new() { Label="BET(TV)", Value="BET(TV)"}
        };
        var leagueSettingResponse = new List<InsertLeagueSettingsResponse>
        {
            new() { HasTv=true, LeagueSettingId=1},
            new() { HasTv=false, LeagueSettingId=2}
        };
        repository.InsertNewLeaguesSettings(Arg.Any<List<LeagueSetting>>()).ReturnsForAnyArgs(leagueSettingResponse);
        repository.GetAllTvConditions().Returns(tvCondition);

        //Act
        await service.InsertNewLeagueSettings(date);
    }

    private static LeagueSettingsService InitLeagueSettingsService()
    {
        var matches = MockMatches();
        var matchSettings = MockMatchSettings();
        var queryRepository = Substitute.For<LeagueSettingsRepository>();
        queryRepository.GetMatchesAndSettings(new DateTime(2021, 04, 12, 02, 00, 00)).Returns(Tuple.Create(matches.AsEnumerable(), matchSettings.AsEnumerable()));
        queryRepository.GetAllTvConditions().Returns(MockTvConditions());

        var syncService = new LeagueSettingsServiceImp(queryRepository);
        return syncService;
    }

    private static IList<TvCondition> MockTvConditions()
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

    private static IList<Match> MockMatches()
        => new List<Match>
        {
            new()
            {
                OwpMatchId = 738253,
                EventDateTime = new DateTime(2021, 04, 07, 15, 00, 00),
                OwpLeagueId = 4,
                Tv = "Euro12",
                MatchSettings = new List<MatchSetting>()
            },
            new()
            {
                OwpMatchId = 738254,
                EventDateTime = new DateTime(2021, 04, 07, 15, 00, 00),
                OwpLeagueId = 34,
                Tv = null,
                MatchSettings = new List<MatchSetting>()
            },
            new()
            {
                OwpMatchId = 738283,
                EventDateTime = new DateTime(2021, 04, 10, 15, 00, 00),
                OwpLeagueId = 15,
                Tv = "Euro12",
                MatchSettings = new List<MatchSetting>()
            },
            new()
            {
                OwpMatchId = 738284,
                EventDateTime = new DateTime(2021, 04, 10, 15, 00, 00),
                OwpLeagueId = 15,
                Tv = "Euro61",
                MatchSettings = new List<MatchSetting>()
            },
            new()
            {
                OwpMatchId = 738293,
                EventDateTime = new DateTime(2021, 04, 10, 15, 00, 00),
                OwpLeagueId = 54,
                Tv = "testing",
                MatchSettings = new List<MatchSetting>()
            },
            new()
            {
                OwpMatchId = 738294,
                EventDateTime = new DateTime(2021, 04, 10, 15, 30, 00),
                OwpLeagueId = 54,
                Tv = null,
                MatchSettings = new List<MatchSetting>()
            }
        };

    private static IEnumerable<MatchSetting> MockMatchSettings()
        => new List<MatchSetting>
        {
            new()
            {
                OwpMatchId = 738253,
                Bettype = Bettype.Hdp,
                Spread = 0.2m,
                Maxbet = 100000.00m
            },
            new()
            {
                OwpMatchId = 738253,
                Bettype = Bettype.Ou,
                Spread = 0.15m,
                Maxbet = null
            },
            new()
            {
                OwpMatchId = 738254,
                Bettype = Bettype.Hdp,
                Spread = 0.25m,
                Maxbet = 200000.00m
            },
            new()
            {
                OwpMatchId = 738254,
                Bettype = Bettype.Ou,
                Spread = 0.30m,
                Maxbet = null
            },
            new()
            {
                OwpMatchId = 738283,
                Bettype = Bettype.Hdp,
                Spread = 0.05m,
                Maxbet = 150000.00m
            },
            new()
            {
                OwpMatchId = 738283,
                Bettype = Bettype.Ou,
                Spread = 0.05m,
                Maxbet = null
            },
            new()
            {
                OwpMatchId = 738284,
                Bettype = Bettype.Hdp,
                Spread = 0.5m,
                Maxbet = 180000.00m
            },
            new()
            {
                OwpMatchId = 738284,
                Bettype = Bettype.Ou,
                Spread = 0.40m,
                Maxbet = null
            },
            new()
            {
                OwpMatchId = 738293,
                Bettype = Bettype.Hdp,
                Spread = 0.04m,
                Maxbet = 300000.00m
            },
            new()
            {
                OwpMatchId = 738293,
                Bettype = Bettype.Ou,
                Spread = 0.04m,
                Maxbet = null
            },
            new()
            {
                OwpMatchId = 738294,
                Bettype = Bettype.Hdp,
                Spread = 0.15m,
                Maxbet = 200000.00m
            },
            new()
            {
                OwpMatchId = 738294,
                Bettype = Bettype.Ou,
                Spread = 0.15m,
                Maxbet = null
            }
        };

    private static IEnumerable<LeagueSetting> MockLeagueSettings()
        => new List<LeagueSetting>
        {
            new()
            {
                OwpLeagueId = 4,
                HasTv = true,
                SpreadHdp = 0.2m,
                SpreadOu = 0.15m,
                Spread1Hhdp = 0.2m,
                Spread1Hou = 0.15m,
                MaxbetWdHdp = 100000.00m,
                MaxbetWd1H = 50000.00m,
                MaxbetWdOu = null,
                MaxbetWk1H = null,
                MaxbetWkHdp = null,
                MaxbetWkOu = null
            },
            new()
            {
                OwpLeagueId = 15,
                HasTv = true,
                SpreadHdp = 0.50m,
                SpreadOu = 0.40m,
                Spread1Hhdp = 0.50m,
                Spread1Hou = 0.40m,
                MaxbetWdHdp = null,
                MaxbetWd1H = null,
                MaxbetWdOu = null,
                MaxbetWk1H = 90000.00m,
                MaxbetWkHdp = 180000.00m,
                MaxbetWkOu = null
            },
            new()
            {
                OwpLeagueId = 34,
                HasTv = false,
                SpreadHdp = 0.25m,
                SpreadOu = 0.30m,
                Spread1Hhdp = 0.25m,
                Spread1Hou = 0.30m,
                MaxbetWdHdp = 200000.00m,
                MaxbetWd1H = 100000.00m
            },
            new()
            {
                OwpLeagueId = 54,
                HasTv = false,
                SpreadHdp = 0.04m,
                SpreadOu = 0.04m,
                Spread1Hhdp = 0.10m,
                Spread1Hou = 0.10m,
                MaxbetWkHdp = 300000.00m,
                MaxbetWk1H = 300000.00m
            }
        };
}