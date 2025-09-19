using OWP.AutoGenerateMaxbetSetting.Helpers;
using OWP.AutoGenerateMaxbetSetting.Models;
using Match = OWP.AutoGenerateMaxbetSetting.Models.Match;

namespace OWP.AutoGenerateMaxbetSetting.UnitTests.Helpers;

public class MatchHelpersTests
{
    [Theory]
    [InlineData("2024-01-15 14:30:00", "2024-01-15 10:00:00", "2024-01-15 18:00:00", true)]
    [InlineData("2024-01-15 14:30:00", "2024-01-15 15:00:00", "2024-01-15 18:00:00", false)]
    [InlineData("2024-01-15 14:30:00", "2024-01-15 10:00:00", "2024-01-15 14:00:00", false)]
    [InlineData("2024-01-15 14:30:00", "2024-01-15 14:30:00", "2024-01-15 18:00:00", true)]
    [InlineData("2024-01-15 14:30:00", "2024-01-15 10:00:00", "2024-01-15 14:30:00", true)]
    public void IsInRangeMatch_ShouldReturnCorrectResult(string eventDateTimeStr, string fromStr, string toStr, bool expected)
    {
        // Arrange
        var eventDateTime = DateTime.Parse(eventDateTimeStr);
        var from = DateTime.Parse(fromStr);
        var to = DateTime.Parse(toStr);
        var match = new Match { EventDateTime = eventDateTime };

        // Act
        var result = MatchHelpers.IsInRangeMatch(match, from, to);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("2024-01-15 14:30:00", "2024-01-16")]
    [InlineData("2024-01-15 10:30:00", "2024-01-15")]
    [InlineData("2024-01-15 12:00:00", "2024-01-16")]
    [InlineData("2024-01-15 11:59:59", "2024-01-15")]
    public void GetMatchTime_ShouldReturnCorrectDate(string eventDateTimeStr, string expectedDateStr)
    {
        // Arrange
        var eventDateTime = DateTime.Parse(eventDateTimeStr);
        var expectedDate = DateTime.Parse(expectedDateStr);
        var match = new Match { EventDateTime = eventDateTime };

        // Act
        var result = MatchHelpers.GetMatchTime(match);

        // Assert
        result.Should().Be(expectedDate);
    }

    [Fact]
    public void FindLatestDay_WithEmptyList_ShouldReturnDefaultDate()
    {
        // Arrange
        var matches = new List<Match>();
        var expectedDate = new DateTime(2021, 01, 01);

        // Act
        var result = MatchHelpers.FindLatestDay(matches);

        // Assert
        result.Should().Be(expectedDate);
    }

    [Fact]
    public void FindLatestDay_WithMatches_ShouldReturnLatestDate()
    {
        // Arrange
        var matches = new List<Match>
        {
            new() { EventDateTime = new DateTime(2024, 1, 15, 10, 0, 0) }, // 2024-01-15
            new() { EventDateTime = new DateTime(2024, 1, 16, 14, 0, 0) }, // 2024-01-17
            new() { EventDateTime = new DateTime(2024, 1, 17, 10, 0, 0) }  // 2024-01-17
        };
        var expectedDate = new DateTime(2024, 1, 17);

        // Act
        var result = MatchHelpers.FindLatestDay(matches.ToList());

        // Assert
        result.Should().Be(expectedDate);
    }

    [Fact]
    public void FindMatches_ShouldReturnMatchingMatches()
    {
        // Arrange
        var matches = new List<Match>
        {
            new() { OwpLeagueId = 1, EventDateTime = new DateTime(2024, 1, 15, 10, 0, 0) },
            new() { OwpLeagueId = 1, EventDateTime = new DateTime(2024, 1, 15, 14, 0, 0) },
            new() { OwpLeagueId = 2, EventDateTime = new DateTime(2024, 1, 15, 12, 0, 0) },
            new() { OwpLeagueId = 1, EventDateTime = new DateTime(2024, 1, 16, 10, 0, 0) }
        };
        var leagueId = 1;
        var from = new DateTime(2024, 1, 15, 8, 0, 0);
        var to = new DateTime(2024, 1, 15, 18, 0, 0);

        // Act
        var result = MatchHelpers.FindMatches(matches.AsEnumerable(), leagueId, from, to);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(m => m.OwpLeagueId == leagueId);
        result.Should().OnlyContain(m => m.EventDateTime >= from && m.EventDateTime <= to);
    }

    [Theory]
    [InlineData(1, "1.5")]
    [InlineData(3, "2.0")]
    [InlineData(7, "1.0")]
    [InlineData(8, "2.5")]
    public void GetSpread_ShouldReturnCorrectSpread(int betType, string expectedSpreadStr)
    {
        // Arrange
        var match = new Match
        {
            MatchSettings = new List<MatchSetting>
            {
                new() { Bettype = 1, Spread = 1.5m },
                new() { Bettype = 3, Spread = 2.0m },
                new() { Bettype = 7, Spread = 1.0m },
                new() { Bettype = 8, Spread = 2.5m }
            }
        };

        // Act
        var result = MatchHelpers.GetSpread(match, betType);
        var expectedSpread = decimal.Parse(expectedSpreadStr);

        // Assert
        result.Should().Be(expectedSpread);
    }

    [Fact]
    public void GetSpread_WithNullMatchSettings_ShouldReturnNull()
    {
        // Arrange
        var match = new Match { MatchSettings = null };

        // Act
        var result = MatchHelpers.GetSpread(match, 1);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(1, "1000.0")]
    [InlineData(3, "2000.0")]
    [InlineData(7, "1500.0")]
    [InlineData(8, "2500.0")]
    public void GetMaxBet_ShouldReturnCorrectMaxBet(int betType, string expectedMaxBetStr)
    {
        // Arrange
        var match = new Match
        {
            MatchSettings = new List<MatchSetting>
            {
                new() { Bettype = 1, Maxbet = 1000.0m },
                new() { Bettype = 3, Maxbet = 2000.0m },
                new() { Bettype = 7, Maxbet = 1500.0m },
                new() { Bettype = 8, Maxbet = 2500.0m }
            }
        };

        // Act
        var result = MatchHelpers.GetMaxBet(match, betType);
        var expectedMaxBet = decimal.Parse(expectedMaxBetStr);

        // Assert
        result.Should().Be(expectedMaxBet);
    }

    [Fact]
    public void GetMaxBet_WithNullMatch_ShouldReturnNull()
    {
        // Act
        var result = MatchHelpers.GetMaxBet(null, 1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetMatchWithLargestMaxbet_WithEmptyList_ShouldReturnNull()
    {
        // Arrange
        var matches = new List<Match>();

        // Act
        var result = MatchHelpers.GetMatchWithLargestMaxbet(matches.ToList());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetMatchWithLargestMaxbet_WithMatches_ShouldReturnMatchWithLargestMaxbet()
    {
        // Arrange
        var matches = new List<Match>
        {
            new()
            {
                OwpMatchId = 1,
                MatchSettings = new List<MatchSetting>
                {
                    new() { Maxbet = 1000.0m },
                    new() { Maxbet = 2000.0m }
                }
            },
            new()
            {
                OwpMatchId = 2,
                MatchSettings = new List<MatchSetting>
                {
                    new() { Maxbet = 1500.0m },
                    new() { Maxbet = 3000.0m }
                }
            },
            new()
            {
                OwpMatchId = 3,
                MatchSettings = new List<MatchSetting>
                {
                    new() { Maxbet = 500.0m },
                    new() { Maxbet = 1000.0m }
                }
            }
        };

        // Act
        var result = MatchHelpers.GetMatchWithLargestMaxbet(matches.ToList());

        // Assert
        result.Should().NotBeNull();
        result.OwpMatchId.Should().Be(2);
    }

    [Fact]
    public void GetMatchWithLargestMaxbet_WithNullMaxbets_ShouldHandleGracefully()
    {
        // Arrange
        var matches = new List<Match>
        {
            new()
            {
                OwpMatchId = 1,
                MatchSettings = new List<MatchSetting>
                {
                    new() { Maxbet = null },
                    new() { Maxbet = 1000.0m }
                }
            }
        };

        // Act
        var result = MatchHelpers.GetMatchWithLargestMaxbet(matches.ToList());

        // Assert
        result.Should().NotBeNull();
        result.OwpMatchId.Should().Be(1);
    }
}
