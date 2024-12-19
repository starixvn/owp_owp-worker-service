namespace OWP.InfositeIdSync.UnitTests;

using Helpers;
using Models;
using Xunit;
using Xunit.Asserts.Compare;

public class InfoSiteHelperUnitTests
{
    [Fact]
    public void MapMatchWithSameLeagueHomeAndAway()
    {
        // Arrange
        var unmappingOwpMatch = GetUnmappingOwpMatch()[0];
        var infositeMatch = GetInfositeMatch();
        var expected = new MatchMappingWithInfosite(1, 123456, 0, new DateTime(2020, 08, 28));

        // Act
        var actual = InfoSiteHelper.MapMatchWithSameLeagueHomeAndAway(unmappingOwpMatch, infositeMatch);

        // Assert
        Assert.Single(actual);
        DeepAssert.Equal(expected, actual[0]);
    }

    [Fact]
    public void MapMatchWithSameLeagueKickoffTimeAndSwapTeam_SameKOHomeEqualAway()
    {
        // Arrange
        var unmappingOwpMatch = GetUnmappingOwpMatch()[1];
        var infositeMatch = GetInfositeMatch();
        var expected = new MatchMappingWithInfosite(2, 2000, 0, new DateTime(2020, 08, 27, 22, 00, 00));

        // Act
        var actual = InfoSiteHelper.MapMatchWithSameLeagueKickoffTimeAndSwapTeam(unmappingOwpMatch, infositeMatch);

        // Assert
        Assert.Single(actual);
        DeepAssert.Equal(expected, actual[0]);
    }

    [Fact]
    public void MapMatchWithSameLeagueHomeAndAway_SameKOHomeEqualAwayAndAwayEqualHome()
    {
        // Arrange
        var unmappingOwpMatch = GetUnmappingOwpMatch()[2];
        var infositeMatch = GetInfositeMatch();
        var expected = new MatchMappingWithInfosite(3, 5000, 0, new DateTime(2020, 08, 27, 23, 20, 00));

        // Act
        var actual = InfoSiteHelper.MapMatchWithSameLeagueHomeAndAway(unmappingOwpMatch, infositeMatch);

        // Assert
        Assert.Single(actual);
        DeepAssert.Equal(expected, actual[0]);
    }

    [Fact]
    public void MapMatchWithSameLeagueKickoffTimeAndSwapTeam_SameLeagueNotSameKO_ReturnEmpty()
    {
        // Arrange
        var unmappingOwpMatch = GetUnmappingOwpMatch()[3];
        var infositeMatch = GetInfositeMatch();

        // Act
        var actual = InfoSiteHelper.MapMatchWithSameLeagueHomeAndAway(unmappingOwpMatch, infositeMatch);

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public void Map_CountIsOne_SingleMatch()
    {
        // Arrange
        var unmappingOwpMatch = new OwpMatch
        {
            League = "SABA league",
            Home = "home",
            Away = "away",
            OwpMatchId = 123,
            EventDateTime = new DateTime(2020, 08, 27, 22, 00, 00)
        };
        var infositeMatch = new List<InfositeMatch>
            {
                new()
                {
                    LeagueName = "SABA league",
                    HomeName = "away",
                    AwayName = "home",
                    EventDateTime = new DateTime(2020, 08, 29, 22, 00, 00),
                    InfoSiteMatchId = 999,
                    AdminSiteMatchId = 100
                }
            };
        var expected = new MatchMappingWithInfosite(123, 999, 100, new DateTime(2020, 08, 27, 22, 00, 00));

        // Act
        var actual = InfoSiteHelper.Map(unmappingOwpMatch, infositeMatch);

        // Assert
        Assert.Single(actual);
        DeepAssert.Equal(expected, actual[0]);
    }

    [Fact]
    public void MapMatchWithSameLeagueHomeAndAwayKOTime_SameKOHomeEqualAwayAndAwayEqualHome()
    {
        // Arrange
        var unmappingOwpMatch = new OwpMatch
        {
            League = "SABA league",
            Home = "home",
            Away = "away",
            OwpMatchId = 123,
            EventDateTime = new DateTime(2020, 08, 27, 22, 00, 00)
        };
        var infositeMatch = new List<InfositeMatch>
            {
                new()
                {
                    LeagueName = "SABA league",
                    HomeName = "home",
                    AwayName = "away",
                    EventDateTime = new DateTime(2020, 08, 27, 22, 00, 00),
                    InfoSiteMatchId = 999,
                    AdminSiteMatchId = 100
                }
            };
        var expected = new MatchMappingWithInfosite(123, 999, 100, new DateTime(2020, 08, 27, 22, 00, 00));

        // Act
        var actual = InfoSiteHelper.MapMatchWithSameLeagueHomeAndAwayKoTime(unmappingOwpMatch, infositeMatch);

        // Assert
        Assert.Single(actual);
        DeepAssert.Equal(expected, actual[0]);
    }

    [Fact]
    public void MapMatchWithSameLeagueHomeAndAway_LowerEqualUppercase_ReturnEmpty()
    {
        // Arrange
        var unmappingOwpMatch = new OwpMatch
        {
            OwpMatchId = 1,
            EventDateTime = new DateTime(2020, 08, 28),
            League = "south africa PREMIERSHIP",
            Home = "Kaizer Chiefs",
            Away = "Mamelodi Sundowns"
        };
        var infositeMatch = GetInfositeMatch();
        var expected = new MatchMappingWithInfosite(1, 123456, 0, new DateTime(2020, 08, 28));

        // Act
        var actual = InfoSiteHelper.MapMatchWithSameLeagueHomeAndAway(unmappingOwpMatch, infositeMatch);

        // Assert
        Assert.Single(actual);
        DeepAssert.Equal(expected, actual[0]);
    }

    [Fact]
    public void Map_NotFoundMatches_ReturnEmpty()
    {
        // Arrange
        var unmappingOwpMatch = GetNotFoundOwpMatch();
        var infositeMatch = GetInfositeMatch();

        // Act
        var actual = InfoSiteHelper.Map(unmappingOwpMatch, infositeMatch);

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public void Map_MapMatchWithSameLeagueHomeAndAway_ReturnMapInfositeId()
    {
        // Arrange
        var unmappingOwpMatch = GetMatchWithSameLeagueHomeAndAway();
        var infositeMatch = GetInfositeMatch();
        var expected = new List<MatchMappingWithInfosite>
            {
                new(6, 2221, 0, new DateTime(2020,08,27,23,20,00))
            };

        // Act
        var actual = InfoSiteHelper.Map(unmappingOwpMatch, infositeMatch);

        // Assert
        Assert.Single(actual);
        DeepAssert.Equal(expected, actual);
    }

    [Fact]
    public void Map_MatchesWithSameLeagueKickoffTimeAndSwapTeam_ReturnMapInfositeId()
    {
        // Arrange
        var unmappingOwpMatch = GetMatchWithSameLeagueKickoffTimeAndSwapTeam();
        var infositeMatch = GetInfositeMatch();

        var expected = new List<MatchMappingWithInfosite>
            {
                new(7, 2222, 0, new DateTime(2020,08,27,23,20,00))
            };

        // Act
        var actual = InfoSiteHelper.Map(unmappingOwpMatch, infositeMatch);

        // Assert
        Assert.Single(actual);
        DeepAssert.Equal(expected, actual);
    }

    [Fact]
    public void Map_Always_ReturnMapInfositeId()
    {
        // Arrange
        var unmappingOwpMatch = GetUnmappingOwpMatch();
        var infositeMatch = GetInfositeMatch();
        var expected = new List<MatchMappingWithInfosite>
            {
                new(1, 123456, 0, new DateTime(2020, 08, 28)),
                new(2, 2000, 0, new DateTime(2020, 08, 27, 22, 00, 00)),
                new(3, 5000, 0, new DateTime(2020, 08, 27, 23, 20, 00))
            };

        // Act
        var actual = InfoSiteHelper.Map(unmappingOwpMatch, infositeMatch);

        // Assert
        Assert.Equal(3, actual.Count);
        DeepAssert.Equal(expected, actual);
    }

    [Fact]
    public void Map_IsSaba_SameKOTimeLeagueHomeAway_SuccessfulMapInfositeID()
    {
        // Arrange
        var unmappingOwpMatch = new OwpMatch
        {
            League = "M88 SABA league",
            Home = "home",
            Away = "away",
            OwpMatchId = 123,
            EventDateTime = new DateTime(2020, 08, 27, 22, 00, 00)
        };
        var infositeMatch = new List<InfositeMatch>
            {
                new()
                {
                    LeagueName = "M88 SABA league",
                    HomeName = "away",
                    AwayName = "home",
                    EventDateTime = new DateTime(2020, 08, 29, 22, 00, 00),
                    InfoSiteMatchId = 999,
                    AdminSiteMatchId = 100
                }
            };
        var expected = new MatchMappingWithInfosite(123, 999, 100, new DateTime(2020, 08, 27, 22, 00, 00));

        // Act
        var actual = InfoSiteHelper.Map(unmappingOwpMatch, infositeMatch);

        // Assert
        Assert.Single(actual);
        DeepAssert.Equal(expected, actual[0]);
    }

    private static OwpMatch GetNotFoundOwpMatch()
    {
        return new OwpMatch
        {
            OwpMatchId = 7,
            EventDateTime = new DateTime(2020, 08, 27, 23, 20, 00),
            League = "Not found",
            Home = "A",
            Away = "B"
        };
    }

    private static OwpMatch GetMatchWithSameLeagueHomeAndAway()
    {
        return new OwpMatch
        {
            OwpMatchId = 6,
            EventDateTime = new DateTime(2020, 08, 27, 23, 20, 00),
            League = "MatchWithSameLeagueHomeAndAway",
            Home = "A",
            Away = "B"
        };
    }

    private static OwpMatch GetMatchWithSameLeagueKickoffTimeAndSwapTeam()
    {
        return new OwpMatch
        {
            OwpMatchId = 7,
            EventDateTime = new DateTime(2020, 08, 27, 23, 20, 00),
            League = "MatchWithSameLeagueKickoffTimeAndSwapTeam",
            Home = "A",
            Away = "B"
        };
    }

    private static List<OwpMatch> GetUnmappingOwpMatch()
    {
        return
        [
            new OwpMatch
            {
                OwpMatchId = 1,
                EventDateTime = new DateTime(2020, 08, 28),
                League = "SOUTH AFRICA PREMIERSHIP",
                Home = "Kaizer Chiefs",
                Away = "Mamelodi Sundowns"
            },

            new OwpMatch
            {
                OwpMatchId = 2,
                EventDateTime = new DateTime(2020, 08, 27, 22, 00, 00),
                League = "UZBEKISTAN SUPER LEAGUE",
                Home = "Pakhtakor Tashkent",
                Away = "Pakhtakor Tashkent"
            },

            new OwpMatch
            {
                OwpMatchId = 3,
                EventDateTime = new DateTime(2020, 08, 27, 23, 20, 00),
                League = "BULGARIA U19 LEAGUE",
                Home = "Levski Sofia U19",
                Away = "Pirin Blagoevgrad U19"
            },

            new OwpMatch
            {
                OwpMatchId = 4,
                EventDateTime = new DateTime(2020, 08, 27, 02, 20, 00),
                League = "ABC",
                Home = "a",
                Away = "a"
            },

            new OwpMatch
            {
                OwpMatchId = 6,
                EventDateTime = new DateTime(2020, 08, 27, 02, 20, 00),
                League = "LEAGUE Test Count 2",
                Away = "home Test Count 2",
                Home = "away Test Count 2"
            }
        ];
    }

    private static List<InfositeMatch> GetInfositeMatch()
    {
        return
        [
            new InfositeMatch
            {
                InfoSiteMatchId = 123456,
                AdminSiteMatchId = 0,
                EventDateTime = new DateTime(2020, 08, 28),
                LeagueName = "SOUTH AFRICA PREMIERSHIP",
                HomeName = "Kaizer Chiefs",
                AwayName = "Mamelodi Sundowns"
            },

            new InfositeMatch
            {
                InfoSiteMatchId = 2000,
                AdminSiteMatchId = 0,
                EventDateTime = new DateTime(2020, 08, 20, 22, 00, 00),
                LeagueName = "UZBEKISTAN SUPER LEAGUE",
                HomeName = "Pakhtakor Tashkent",
                AwayName = "Pakhtakor Tashkent"
            },

            new InfositeMatch
            {
                InfoSiteMatchId = 5000,
                AdminSiteMatchId = 0,
                EventDateTime = new DateTime(2020, 08, 27, 23, 20, 00),
                LeagueName = "BULGARIA U19 LEAGUE",
                HomeName = "Levski Sofia U19",
                AwayName = "Pirin Blagoevgrad U19"
            },

            new InfositeMatch
            {
                InfoSiteMatchId = 5555,
                AdminSiteMatchId = 0,
                EventDateTime = new DateTime(2020, 08, 27, 11, 20, 00),
                LeagueName = "ABC",
                HomeName = "h",
                AwayName = "a"
            },

            new InfositeMatch
            {
                InfoSiteMatchId = 2221,
                AdminSiteMatchId = 0,
                EventDateTime = new DateTime(2020, 08, 27, 23, 20, 00),
                LeagueName = "LEAGUE Test Count 2",
                HomeName = "home Test Count 2",
                AwayName = "away Test Count 2"
            },

            new InfositeMatch
            {
                InfoSiteMatchId = 2222,
                AdminSiteMatchId = 0,
                EventDateTime = new DateTime(2020, 08, 27, 11, 20, 00),
                LeagueName = "LEAGUE Test Count 2",
                HomeName = "home Test Count 2",
                AwayName = "away Test Count 2"
            },

            new InfositeMatch
            {
                InfoSiteMatchId = 2221,
                AdminSiteMatchId = 0,
                EventDateTime = new DateTime(2020, 08, 27, 23, 20, 00),
                LeagueName = "MatchWithSameLeagueHomeAndAway",
                HomeName = "A",
                AwayName = "B"
            },

            new InfositeMatch
            {
                InfoSiteMatchId = 2222,
                AdminSiteMatchId = 0,
                EventDateTime = new DateTime(2020, 08, 27, 23, 20, 00),
                LeagueName = "MatchWithSameLeagueKickoffTimeAndSwapTeam",
                HomeName = "B",
                AwayName = "A"
            }
        ];
    }
}