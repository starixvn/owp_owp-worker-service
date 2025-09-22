using FluentAssertions;
using OWP.Adminsite.Builder;
using OWP.Adminsite.Enums;
using OWP.Adminsite.Models;
using Xunit;

namespace OWP.Adminsite.UnitTests.Builder;

public class OddBuilderTests
{
    [Fact]
    public void BuildOdd_ShouldAddHandicapAndOverUnderSettings()
    {
        // Arrange
        var matchInfo = new MatchInfo { OWPMatchId = 12345 };
        var matchInfoApi = new MatchInfoApiModel { MatchSettings = new List<MatchSettingApiModel>() };
        var recommendOdds = new List<MatchRecommendOdd>();
        var recommendOddDetails = new List<MatchRecommendOddDetail>();

        // Act
        OddBuilder.BuildOdd(matchInfo, matchInfoApi, recommendOdds, recommendOddDetails);

        // Assert
        matchInfoApi.MatchSettings.Should().HaveCount(2);
        matchInfoApi.MatchSettings.Should().Contain(x => x.Bettype == (int)MajorBetType.Handicap);
        matchInfoApi.MatchSettings.Should().Contain(x => x.Bettype == (int)MajorBetType.OverUnder);
    }
}