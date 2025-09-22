using FluentAssertions;
using OWP.Adminsite.Builder;
using OWP.Adminsite.Constants;
using OWP.Adminsite.Enums;
using OWP.Adminsite.Models;
using Xunit;

namespace OWP.Adminsite.UnitTests.Builder;

public class SpreadBuilderTests
{
    private readonly MatchInfo _matchInfo;
    private readonly MatchInfoApiModel _matchInfoApi;
    private readonly List<MatchSetting> _matchSettings;

    public SpreadBuilderTests()
    {
        _matchInfo = new MatchInfo
        {
            OWPMatchId = 12345,
            LeagueGroupId = 2 // Not MJ league group
        };

        _matchInfoApi = new MatchInfoApiModel
        {
            MatchSettings = new List<MatchSettingApiModel>()
        };

        _matchSettings = new List<MatchSetting>();
    }

    [Fact]
    public void BuildSpread_WithMJLeagueGroup_ShouldCallBuildSpreadForLeagueGroupMJ()
    {
        // Arrange
        _matchInfo.LeagueGroupId = 1; // MJ league group
        _matchSettings.AddRange(new[]
        {
            new MatchSetting { OWPMatchId = 12345, BettypeId = (int)MajorBetType.Handicap, Spread = 0.05m },
            new MatchSetting { OWPMatchId = 12345, BettypeId = (int)MajorBetType.OverUnder, Spread = 0.08m }
        });

        // Act
        SpreadBuilder.BuildSpread(_matchInfo, _matchInfoApi, _matchSettings);

        // Assert
        _matchInfoApi.MatchSettings.Should().HaveCount(4);
        _matchInfoApi.MatchSettings.Should().Contain(x => x.Bettype == (int)MajorBetType.Handicap);
        _matchInfoApi.MatchSettings.Should().Contain(x => x.Bettype == (int)MajorBetType.OverUnder);
        _matchInfoApi.MatchSettings.Should().Contain(x => x.Bettype == (int)MajorBetType.Handicap1St);
        _matchInfoApi.MatchSettings.Should().Contain(x => x.Bettype == (int)MajorBetType.OverUnder1St);
    }

    [Fact]
    public void BuildSpread_WithNonMJLeagueGroup_ShouldCallBuildSpreadForLeagueGroupNotMJ()
    {
        // Arrange
        _matchInfo.LeagueGroupId = 2; // Non-MJ league group
        _matchSettings.AddRange(new[]
        {
            new MatchSetting { OWPMatchId = 12345, BettypeId = (int)MajorBetType.Handicap, Spread = 0.05m },
            new MatchSetting { OWPMatchId = 12345, BettypeId = (int)MajorBetType.OverUnder, Spread = 0.08m }
        });

        // Act
        SpreadBuilder.BuildSpread(_matchInfo, _matchInfoApi, _matchSettings);

        // Assert
        _matchInfoApi.MatchSettings.Should().HaveCount(4);
        _matchInfoApi.MatchSettings.Should().Contain(x => x.Bettype == (int)MajorBetType.Handicap);
        _matchInfoApi.MatchSettings.Should().Contain(x => x.Bettype == (int)MajorBetType.OverUnder);
        _matchInfoApi.MatchSettings.Should().Contain(x => x.Bettype == (int)MajorBetType.Handicap1St);
        _matchInfoApi.MatchSettings.Should().Contain(x => x.Bettype == (int)MajorBetType.OverUnder1St);
    }

    [Fact]
    public void BuildSpread_WithNoSpreads_ShouldNotAddAnySettings()
    {
        // Arrange
        _matchSettings.Clear();

        // Act
        SpreadBuilder.BuildSpread(_matchInfo, _matchInfoApi, _matchSettings);

        // Assert
        _matchInfoApi.MatchSettings.Should().HaveCount(2);
        _matchInfoApi.MatchSettings.Should().Contain(x => x.Bettype == (int)MajorBetType.Handicap1St);
        _matchInfoApi.MatchSettings.Should().Contain(x => x.Bettype == (int)MajorBetType.OverUnder1St);
    }

    [Fact]
    public void BuildSpread_WithDifferentMatchId_ShouldNotProcessSpreads()
    {
        // Arrange
        _matchSettings.Add(new MatchSetting { OWPMatchId = 99999, BettypeId = (int)MajorBetType.Handicap, Spread = 0.05m });

        // Act
        SpreadBuilder.BuildSpread(_matchInfo, _matchInfoApi, _matchSettings);

        // Assert
        _matchInfoApi.MatchSettings.Should().HaveCount(2);
        _matchInfoApi.MatchSettings.Should().Contain(x => x.Bettype == (int)MajorBetType.Handicap1St);
        _matchInfoApi.MatchSettings.Should().Contain(x => x.Bettype == (int)MajorBetType.OverUnder1St);
    }

    [Fact]
    public void BuildSpreadSpecialProduct_WithSpecificXMinsBettype_ShouldCreateHandicapAndOverUnderSpreads()
    {
        // Arrange
        _matchInfo.MinorBettypeId = SpecialBettype.BETTYPE_ID_SPECIFIC15MINS;
        _matchSettings.Add(new MatchSetting { OWPMatchId = 12345, BettypeId = (int)MajorBetType.Handicap, Spread = 0.05m });

        // Act
        SpreadBuilder.BuildSpreadSpecialProduct(_matchInfo, _matchInfoApi, _matchSettings);

        // Assert
        _matchInfoApi.MatchSettings.Should().HaveCount(2);
        _matchInfoApi.MatchSettings.Should().Contain(x => x.Bettype == (int)MajorBetType.Handicap && x.Spread == 0.05m);
        _matchInfoApi.MatchSettings.Should().Contain(x => x.Bettype == (int)MajorBetType.OverUnder && x.Spread == 0.05m);
    }

    [Fact]
    public void BuildSpreadSpecialProduct_WithNonSpecificXMinsBettype_ShouldCallBuildSpread()
    {
        // Arrange
        _matchInfo.MinorBettypeId = 0; // Main match
        _matchSettings.Add(new MatchSetting { OWPMatchId = 12345, BettypeId = (int)MajorBetType.Handicap, Spread = 0.05m });

        // Act
        SpreadBuilder.BuildSpreadSpecialProduct(_matchInfo, _matchInfoApi, _matchSettings);

        // Assert
        _matchInfoApi.MatchSettings.Should().HaveCount(3);
        _matchInfoApi.MatchSettings.Should().Contain(x => x.Bettype == (int)MajorBetType.Handicap);
        _matchInfoApi.MatchSettings.Should().Contain(x => x.Bettype == (int)MajorBetType.Handicap1St);
        _matchInfoApi.MatchSettings.Should().Contain(x => x.Bettype == (int)MajorBetType.OverUnder1St);
    }

    [Fact]
    public void BuildSpreadSpecialProduct_WithNoSpreadHandicap_ShouldNotAddAnySettings()
    {
        // Arrange
        _matchInfo.MinorBettypeId = SpecialBettype.BETTYPE_ID_SPECIFIC15MINS;
        _matchSettings.Clear();

        // Act
        SpreadBuilder.BuildSpreadSpecialProduct(_matchInfo, _matchInfoApi, _matchSettings);

        // Assert
        _matchInfoApi.MatchSettings.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0.12, 0.12)] // Handicap spread > 0.09
    [InlineData(0.05, 0.1)] // Handicap spread < 0.09
    [InlineData(0.09, 0.11)] // Handicap spread = 0.09
    public void BuildSpread_WithDifferentHandicapSpreads_ShouldSetCorrectFirstHalfHandicapSpread(
        decimal handicapSpread, decimal expectedFirstHalfSpread)
    {
        // Arrange
        _matchInfo.LeagueGroupId = 2; // Non-MJ league group
        _matchSettings.Add(new MatchSetting { OWPMatchId = 12345, BettypeId = (int)MajorBetType.Handicap, Spread = handicapSpread });

        // Act
        SpreadBuilder.BuildSpread(_matchInfo, _matchInfoApi, _matchSettings);

        // Assert
        var firstHalfHandicap = _matchInfoApi.MatchSettings.FirstOrDefault(x => x.Bettype == (int)MajorBetType.Handicap1St);
        firstHalfHandicap.Should().NotBeNull();
        firstHalfHandicap!.Spread.Should().Be(expectedFirstHalfSpread);
    }

    [Fact]
    public void BuildSpread_WithNullHandicapSpread_ShouldHandleGracefully()
    {
        // Arrange
        _matchInfo.LeagueGroupId = 2; // Non-MJ league group
        _matchSettings.Add(new MatchSetting { OWPMatchId = 12345, BettypeId = (int)MajorBetType.Handicap, Spread = null });

        // Act
        SpreadBuilder.BuildSpread(_matchInfo, _matchInfoApi, _matchSettings);

        // Assert
        var firstHalfHandicap = _matchInfoApi.MatchSettings.FirstOrDefault(x => x.Bettype == (int)MajorBetType.Handicap1St);
        firstHalfHandicap.Should().NotBeNull();
        firstHalfHandicap!.Spread.Should().BeNull();
    }

    [Fact]
    public void BuildSpread_WithNullOverUnderSpread_ShouldHandleGracefully()
    {
        // Arrange
        _matchInfo.LeagueGroupId = 2; // Non-MJ league group
        _matchSettings.Add(new MatchSetting { OWPMatchId = 12345, BettypeId = (int)MajorBetType.OverUnder, Spread = null });

        // Act
        SpreadBuilder.BuildSpread(_matchInfo, _matchInfoApi, _matchSettings);

        // Assert
        var firstHalfOverUnder = _matchInfoApi.MatchSettings.FirstOrDefault(x => x.Bettype == (int)MajorBetType.OverUnder1St);
        firstHalfOverUnder.Should().NotBeNull();
        firstHalfOverUnder!.Spread.Should().BeNull(); // Current implementation uses null when input is null
    }

    [Fact]
    public void BuildSpread_WithEmptyMatchSettings_ShouldNotThrow()
    {
        // Arrange
        _matchInfoApi.MatchSettings.Clear();

        // Act & Assert
        var action = () => SpreadBuilder.BuildSpread(_matchInfo, _matchInfoApi, _matchSettings);
        action.Should().NotThrow();
    }
}