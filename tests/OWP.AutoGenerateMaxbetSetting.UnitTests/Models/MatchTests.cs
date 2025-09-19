using OWP.AutoGenerateMaxbetSetting.Models;
using Match = OWP.AutoGenerateMaxbetSetting.Models.Match;

namespace OWP.AutoGenerateMaxbetSetting.UnitTests.Models;

public class MatchTests
{
    [Fact]
    public void Constructor_ShouldCreateInstanceWithDefaultValues()
    {
        // Act
        var match = new Match();

        // Assert
        match.Should().NotBeNull();
        match.OwpLeagueId.Should().Be(0);
        match.OwpMatchId.Should().Be(0);
        match.EventDateTime.Should().Be(default(DateTime));
        match.Tv.Should().BeNull();
        match.NoOfHDP.Should().BeNull();
        match.MatchSettings.Should().BeNull();
    }

    [Theory]
    [InlineData(1, 100, "2024-01-15 14:30:00", "TV1", "2")]
    [InlineData(2, 200, "2024-01-16 16:00:00", "TV2", "3")]
    [InlineData(3, 300, "2024-01-17 18:30:00", "", "")]
    public void Properties_ShouldSetAndGetCorrectValues(int owpLeagueId, int owpMatchId, string eventDateTimeStr, string tv, string noOfHDP)
    {
        // Arrange
        var eventDateTime = DateTime.Parse(eventDateTimeStr);
        var match = new Match();
        var matchSettings = new List<MatchSetting>
        {
            new() { OwpMatchId = owpMatchId, Bettype = 1, Spread = 1.5m, Maxbet = 1000m }
        };

        // Act
        match.OwpLeagueId = owpLeagueId;
        match.OwpMatchId = owpMatchId;
        match.EventDateTime = eventDateTime;
        match.Tv = tv;
        match.NoOfHDP = noOfHDP;
        match.MatchSettings = matchSettings;

        // Assert
        match.OwpLeagueId.Should().Be(owpLeagueId);
        match.OwpMatchId.Should().Be(owpMatchId);
        match.EventDateTime.Should().Be(eventDateTime);
        match.Tv.Should().Be(tv);
        match.NoOfHDP.Should().Be(noOfHDP);
        match.MatchSettings.Should().BeEquivalentTo(matchSettings);
    }

    [Fact]
    public void Properties_ShouldAllowNullValues()
    {
        // Arrange
        var match = new Match();

        // Act
        match.Tv = null;
        match.NoOfHDP = null;
        match.MatchSettings = null;

        // Assert
        match.Tv.Should().BeNull();
        match.NoOfHDP.Should().BeNull();
        match.MatchSettings.Should().BeNull();
    }

    [Fact]
    public void ObjectInitializer_ShouldWorkCorrectly()
    {
        // Arrange
        var eventDateTime = new DateTime(2024, 1, 15, 14, 30, 0);
        var matchSettings = new List<MatchSetting>
        {
            new() { OwpMatchId = 100, Bettype = 1, Spread = 1.5m, Maxbet = 1000m }
        };

        // Act
        var match = new Match
        {
            OwpLeagueId = 1,
            OwpMatchId = 100,
            EventDateTime = eventDateTime,
            Tv = "TV1",
            NoOfHDP = "2",
            MatchSettings = matchSettings
        };

        // Assert
        match.OwpLeagueId.Should().Be(1);
        match.OwpMatchId.Should().Be(100);
        match.EventDateTime.Should().Be(eventDateTime);
        match.Tv.Should().Be("TV1");
        match.NoOfHDP.Should().Be("2");
        match.MatchSettings.Should().BeEquivalentTo(matchSettings);
    }

    [Fact]
    public void MatchSettings_ShouldBeMutable()
    {
        // Arrange
        var match = new Match();
        var initialSettings = new List<MatchSetting>
        {
            new() { OwpMatchId = 100, Bettype = 1, Spread = 1.5m, Maxbet = 1000m }
        };

        // Act
        match.MatchSettings = initialSettings;
        match.MatchSettings.Add(new MatchSetting { OwpMatchId = 100, Bettype = 3, Spread = 2.0m, Maxbet = 2000m });

        // Assert
        match.MatchSettings.Should().HaveCount(2);
    }
}
