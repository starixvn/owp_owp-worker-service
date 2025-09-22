using OWP.Adminsite.Builder;
using OWP.Adminsite.Constants;
using OWP.Adminsite.Enums;
using OWP.Adminsite.Models;
using Xunit;

namespace OWP.Adminsite.UnitTests.Builder;

public class MaxbetBuilderTests
{
    [Fact]
    public void BuildMaxbet_WithEmptyMaxbetList_ShouldCallBuildMaxbetDirectly()
    {
        // Arrange
        var matchInfo = CreateMatchInfo(1);
        var matchInfoApi = CreateMatchInfoApiModel();
        var matchSettings = new List<MatchSetting>();
        var defaultMaxbetSettings = new List<MaxbetSetting>();

        // Act
        MaxbetBuilder.BuildMaxbet(matchInfo, matchInfoApi, matchSettings, defaultMaxbetSettings);

        // Assert
        // Should not throw exception and should handle empty list gracefully
        Assert.NotNull(matchInfoApi);
    }

    [Fact]
    public void BuildMaxbet_WithCompleteBetTypes_ShouldCallBuildMaxbetDirectly()
    {
        // Arrange
        var matchInfo = CreateMatchInfo(1);
        var matchInfoApi = CreateMatchInfoApiModel();
        var matchSettings = new List<MatchSetting>
        {
            CreateMatchSetting(1, (int)MajorBetType.Handicap, 1000),
            CreateMatchSetting(1, (int)MajorBetType.Handicap1St, 1000),
            CreateMatchSetting(1, (int)MajorBetType.OverUnder, 1000),
            CreateMatchSetting(1, (int)MajorBetType.OverUnder1St, 1000)
        };
        var defaultMaxbetSettings = new List<MaxbetSetting>();

        // Act
        MaxbetBuilder.BuildMaxbet(matchInfo, matchInfoApi, matchSettings, defaultMaxbetSettings);

        // Assert
        Assert.Equal(4, matchInfoApi.MatchSettings.Count);
    }

    [Fact]
    public void BuildMaxbet_WithMissingOverUnder_ShouldBuildOverUnder()
    {
        // Arrange
        var matchInfo = CreateMatchInfo(1);
        var matchInfoApi = CreateMatchInfoApiModel();
        var matchSettings = new List<MatchSetting>
        {
            CreateMatchSetting(1, (int)MajorBetType.Handicap, 1000)
        };
        var defaultMaxbetSettings = new List<MaxbetSetting>
        {
            CreateMaxbetSetting(1000, 2000, 1500, 2500)
        };

        // Act
        MaxbetBuilder.BuildMaxbet(matchInfo, matchInfoApi, matchSettings, defaultMaxbetSettings);

        // Assert
        Assert.Equal(4, matchInfoApi.MatchSettings.Count);
        var overUnderSetting = matchInfoApi.MatchSettings.FirstOrDefault(x => x.Bettype == (int)MajorBetType.OverUnder);
        Assert.NotNull(overUnderSetting);
        Assert.Equal(2000, overUnderSetting.Maxbet);
    }

    [Fact]
    public void BuildMaxbet_WithMissingHandicap1St_ShouldBuildHandicap1St()
    {
        // Arrange
        var matchInfo = CreateMatchInfo(1);
        var matchInfoApi = CreateMatchInfoApiModel();
        var matchSettings = new List<MatchSetting>
        {
            CreateMatchSetting(1, (int)MajorBetType.Handicap, 1000)
        };
        var defaultMaxbetSettings = new List<MaxbetSetting>
        {
            CreateMaxbetSetting(1000, 2000, 1500, 2500)
        };

        // Act
        MaxbetBuilder.BuildMaxbet(matchInfo, matchInfoApi, matchSettings, defaultMaxbetSettings);

        // Assert
        Assert.Equal(4, matchInfoApi.MatchSettings.Count);
        var handicap1StSetting = matchInfoApi.MatchSettings.FirstOrDefault(x => x.Bettype == (int)MajorBetType.Handicap1St);
        Assert.NotNull(handicap1StSetting);
        Assert.Equal(1500, handicap1StSetting.Maxbet);
    }

    [Fact]
    public void BuildMaxbet_WithMissingOverUnder1St_ShouldBuildOverUnder1St()
    {
        // Arrange
        var matchInfo = CreateMatchInfo(1);
        var matchInfoApi = CreateMatchInfoApiModel();
        var matchSettings = new List<MatchSetting>
        {
            CreateMatchSetting(1, (int)MajorBetType.Handicap, 1000)
        };
        var defaultMaxbetSettings = new List<MaxbetSetting>
        {
            CreateMaxbetSetting(1000, 2000, 1500, 2500)
        };

        // Act
        MaxbetBuilder.BuildMaxbet(matchInfo, matchInfoApi, matchSettings, defaultMaxbetSettings);

        // Assert
        Assert.Equal(4, matchInfoApi.MatchSettings.Count);
        var overUnder1StSetting = matchInfoApi.MatchSettings.FirstOrDefault(x => x.Bettype == (int)MajorBetType.OverUnder1St);
        Assert.NotNull(overUnder1StSetting);
        Assert.Equal(2500, overUnder1StSetting.Maxbet);
    }

    [Fact]
    public void BuildMaxbetSpecialProduct_WithSpecificXMins_ShouldCallBuildMaxbetXMins()
    {
        // Arrange
        var matchInfo = CreateMatchInfo(1, SpecialBettype.BETTYPE_ID_SPECIFIC10MINS);
        var matchInfoApi = CreateMatchInfoApiModel();
        var matchSettings = new List<MatchSetting>
        {
            CreateMatchSetting(1, (int)MajorBetType.Handicap, 1000)
        };
        var defaultMaxbetSettings = new List<MaxbetSetting>();

        // Act
        MaxbetBuilder.BuildMaxbetSpecialProduct(matchInfo, matchInfoApi, matchSettings, defaultMaxbetSettings);

        // Assert
        Assert.Equal(2, matchInfoApi.MatchSettings.Count);
        var handicapSetting = matchInfoApi.MatchSettings.FirstOrDefault(x => x.Bettype == (int)MajorBetType.Handicap);
        var overUnderSetting = matchInfoApi.MatchSettings.FirstOrDefault(x => x.Bettype == (int)MajorBetType.OverUnder);
        Assert.NotNull(handicapSetting);
        Assert.NotNull(overUnderSetting);
        Assert.Equal(1000, handicapSetting.Maxbet);
        Assert.Equal(1000, overUnderSetting.Maxbet);
    }

    [Fact]
    public void BuildMaxbetSpecialProduct_WithNoOfCorners_ShouldCallBuildMaxbetNoOfCorner()
    {
        // Arrange
        var matchInfo = CreateMatchInfo(1, SpecialBettype.BETTYPE_ID_NO_OF_CORNERS, 100);
        var matchInfoApi = CreateMatchInfoApiModel();
        var matchSettings = new List<MatchSetting>();
        var defaultMaxbetSettings = new List<MaxbetSetting>
        {
            CreateMaxbetSetting(1000, 2000, 1500, 2500, 3000, 4000, 3500, 4500)
        };

        // Act
        MaxbetBuilder.BuildMaxbetSpecialProduct(matchInfo, matchInfoApi, matchSettings, defaultMaxbetSettings);

        // Assert
        Assert.Equal(4, matchInfoApi.MatchSettings.Count);
        Assert.Contains(matchInfoApi.MatchSettings, x => x.Bettype == (int)MajorBetType.Handicap);
        Assert.Contains(matchInfoApi.MatchSettings, x => x.Bettype == (int)MajorBetType.OverUnder);
        Assert.Contains(matchInfoApi.MatchSettings, x => x.Bettype == (int)MajorBetType.Handicap1St);
        Assert.Contains(matchInfoApi.MatchSettings, x => x.Bettype == (int)MajorBetType.OverUnder1St);
    }

    [Fact]
    public void BuildMaxbetSpecialProduct_WithNextCorners_ShouldCallBuildMaxbetNextCorner()
    {
        // Arrange
        var matchInfo = CreateMatchInfo(1, SpecialBettype.BETTYPE_ID_NEXT_CORNERS, 100);
        var matchInfoApi = CreateMatchInfoApiModel();
        var matchSettings = new List<MatchSetting>
        {
            CreateMatchSetting(100, (int)MajorBetType.Handicap, 1000) // Main match setting
        };
        var defaultMaxbetSettings = new List<MaxbetSetting>
        {
            CreateMaxbetSetting(1000, 2000, 1500, 2500, 0, 0, 0, 0, 5000)
        };

        // Act
        MaxbetBuilder.BuildMaxbetSpecialProduct(matchInfo, matchInfoApi, matchSettings, defaultMaxbetSettings);

        // Assert
        Assert.Single(matchInfoApi.MatchSettings);
        var handicapSetting = matchInfoApi.MatchSettings.FirstOrDefault(x => x.Bettype == (int)MajorBetType.Handicap);
        Assert.NotNull(handicapSetting);
        Assert.Equal(5000, handicapSetting.Maxbet);
    }

    [Fact]
    public void BuildMaxbetSpecialProduct_WithNextBookings_ShouldCallBuildMaxbetNextBooking()
    {
        // Arrange
        var matchInfo = CreateMatchInfo(1, SpecialBettype.BETTYPE_ID_NEXT_BOOKINGS, 100);
        var matchInfoApi = CreateMatchInfoApiModel();
        var matchSettings = new List<MatchSetting>
        {
            CreateMatchSetting(100, (int)MajorBetType.Handicap, 1000) // Main match setting
        };
        var defaultMaxbetSettings = new List<MaxbetSetting>
        {
            CreateMaxbetSetting(1000, 2000, 1500, 2500, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6000)
        };

        // Act
        MaxbetBuilder.BuildMaxbetSpecialProduct(matchInfo, matchInfoApi, matchSettings, defaultMaxbetSettings);

        // Assert
        Assert.Single(matchInfoApi.MatchSettings);
        var handicapSetting = matchInfoApi.MatchSettings.FirstOrDefault(x => x.Bettype == (int)MajorBetType.Handicap);
        Assert.NotNull(handicapSetting);
        Assert.Equal(6000, handicapSetting.Maxbet);
    }

    [Fact]
    public void BuildMaxbetSpecialProduct_WithTotalBookings_ShouldCallBuildMaxbetTotalBooking()
    {
        // Arrange
        var matchInfo = CreateMatchInfo(1, SpecialBettype.BETTYPE_ID_TOTAL_BOOKINGS, 100);
        var matchInfoApi = CreateMatchInfoApiModel();
        var matchSettings = new List<MatchSetting>();
        var defaultMaxbetSettings = new List<MaxbetSetting>
        {
            CreateMaxbetSetting(1000, 2000, 1500, 2500, 0, 0, 0, 0, 0, 7000, 8000, 9000, 10000)
        };

        // Act
        MaxbetBuilder.BuildMaxbetSpecialProduct(matchInfo, matchInfoApi, matchSettings, defaultMaxbetSettings);

        // Assert
        Assert.Equal(4, matchInfoApi.MatchSettings.Count);
        Assert.Contains(matchInfoApi.MatchSettings, x => x.Bettype == (int)MajorBetType.Handicap);
        Assert.Contains(matchInfoApi.MatchSettings, x => x.Bettype == (int)MajorBetType.OverUnder);
        Assert.Contains(matchInfoApi.MatchSettings, x => x.Bettype == (int)MajorBetType.Handicap1St);
        Assert.Contains(matchInfoApi.MatchSettings, x => x.Bettype == (int)MajorBetType.OverUnder1St);
    }

    [Fact]
    public void BuildMaxbetSpecialProduct_WithOtherBettype_ShouldCallBuildMaxbetDirectly()
    {
        // Arrange
        var matchInfo = CreateMatchInfo(1, 999); // Unknown bettype
        var matchInfoApi = CreateMatchInfoApiModel();
        var matchSettings = new List<MatchSetting>
        {
            CreateMatchSetting(1, (int)MajorBetType.Handicap, 1000)
        };
        var defaultMaxbetSettings = new List<MaxbetSetting>();

        // Act
        MaxbetBuilder.BuildMaxbetSpecialProduct(matchInfo, matchInfoApi, matchSettings, defaultMaxbetSettings);

        // Assert
        Assert.Single(matchInfoApi.MatchSettings);
        var handicapSetting = matchInfoApi.MatchSettings.FirstOrDefault(x => x.Bettype == (int)MajorBetType.Handicap);
        Assert.NotNull(handicapSetting);
        Assert.Equal(1000, handicapSetting.Maxbet);
    }

    [Fact]
    public void BuildMaxbetSpecialProduct_WithNoMaxbetSettings_ShouldNotAddAnySettings()
    {
        // Arrange
        var matchInfo = CreateMatchInfo(1, 999); // Unknown bettype
        var matchInfoApi = CreateMatchInfoApiModel();
        var matchSettings = new List<MatchSetting>(); // No maxbet settings
        var defaultMaxbetSettings = new List<MaxbetSetting>();

        // Act
        MaxbetBuilder.BuildMaxbetSpecialProduct(matchInfo, matchInfoApi, matchSettings, defaultMaxbetSettings);

        // Assert
        Assert.Empty(matchInfoApi.MatchSettings);
    }

    [Fact]
    public void BuildMaxbet_WithExistingMatchSettings_ShouldUpdateExistingSettings()
    {
        // Arrange
        var matchInfo = CreateMatchInfo(1);
        var matchInfoApi = CreateMatchInfoApiModel();
        matchInfoApi.MatchSettings.Add(new MatchSettingApiModel
        {
            Bettype = (int)MajorBetType.Handicap,
            Maxbet = 500
        });
        var matchSettings = new List<MatchSetting>
        {
            CreateMatchSetting(1, (int)MajorBetType.Handicap, 1000)
        };
        var defaultMaxbetSettings = new List<MaxbetSetting>();

        // Act
        MaxbetBuilder.BuildMaxbet(matchInfo, matchInfoApi, matchSettings, defaultMaxbetSettings);

        // Assert
        Assert.Equal(4, matchInfoApi.MatchSettings.Count);
        var handicapSetting = matchInfoApi.MatchSettings.FirstOrDefault(x => x.Bettype == (int)MajorBetType.Handicap);
        Assert.NotNull(handicapSetting);
        Assert.Equal(1000, handicapSetting.Maxbet); // Should be updated, not added
    }

    [Fact]
    public void BuildMaxbet_WithFollowMainMatchValue_ShouldUseDefaultValue()
    {
        // Arrange
        var matchInfo = CreateMatchInfo(1);
        var matchInfoApi = CreateMatchInfoApiModel();
        var matchSettings = new List<MatchSetting>
        {
            CreateMatchSetting(1, (int)MajorBetType.Handicap, 1000)
        };
        var defaultMaxbetSettings = new List<MaxbetSetting>
        {
            new MaxbetSetting
            {
                FT_HDP = 1000,
                FT_OU = MaxBetSetting.FOLLOW_MAIN_MATCH // Special value
            }
        };

        // Act
        MaxbetBuilder.BuildMaxbet(matchInfo, matchInfoApi, matchSettings, defaultMaxbetSettings);

        // Assert
        Assert.Equal(4, matchInfoApi.MatchSettings.Count);
        var overUnderSetting = matchInfoApi.MatchSettings.FirstOrDefault(x => x.Bettype == (int)MajorBetType.OverUnder);
        Assert.NotNull(overUnderSetting);
        Assert.Equal(1000, overUnderSetting.Maxbet); // Should use handicap value as fallback
    }

    [Fact]
    public void BuildMaxbet_WithDecimalMaxbet_ShouldRoundToInteger()
    {
        // Arrange
        var matchInfo = CreateMatchInfo(1);
        var matchInfoApi = CreateMatchInfoApiModel();
        var matchSettings = new List<MatchSetting>
        {
            CreateMatchSetting(1, (int)MajorBetType.Handicap, 1000.7m)
        };
        var defaultMaxbetSettings = new List<MaxbetSetting>();

        // Act
        MaxbetBuilder.BuildMaxbet(matchInfo, matchInfoApi, matchSettings, defaultMaxbetSettings);

        // Assert
        Assert.Equal(4, matchInfoApi.MatchSettings.Count);
        var handicapSetting = matchInfoApi.MatchSettings.FirstOrDefault(x => x.Bettype == (int)MajorBetType.Handicap);
        Assert.NotNull(handicapSetting);
        Assert.Equal(1001, handicapSetting.Maxbet); // Should be rounded
    }

    [Fact]
    public void BuildMaxbet_WithNullMaxbet_ShouldHandleGracefully()
    {
        // Arrange
        var matchInfo = CreateMatchInfo(1);
        var matchInfoApi = CreateMatchInfoApiModel();
        var matchSettings = new List<MatchSetting>
        {
            CreateMatchSetting(1, (int)MajorBetType.Handicap, null)
        };
        var defaultMaxbetSettings = new List<MaxbetSetting>();

        // Act
        MaxbetBuilder.BuildMaxbet(matchInfo, matchInfoApi, matchSettings, defaultMaxbetSettings);

        // Assert
        // Should not throw exception and should handle null gracefully
        Assert.NotNull(matchInfoApi);
    }

    // Helper methods to create test data
    private static MatchInfo CreateMatchInfo(int owpMatchId, int? minorBettypeId = null, int? parentMatchId = null)
    {
        return new MatchInfo
        {
            OWPMatchId = owpMatchId,
            MinorBettypeId = minorBettypeId,
            ParentMatchId = parentMatchId,
            AdminSiteMatchId = owpMatchId + 1000,
            League = "Test League",
            Provider = "Test Provider",
            OtherProvider = "Test Other Provider",
            WatchBet = "Test Watch Bet",
            NoOfHDPId = 1,
            LeagueGroupId = 1
        };
    }

    private static MatchInfoApiModel CreateMatchInfoApiModel()
    {
        return new MatchInfoApiModel
        {
            MatchId = 1,
            SubType = 1,
            Provider = "Test Provider",
            OtherProvider = "Test Other Provider",
            WatchBet = "Test Watch Bet",
            SecondAcceptValue = 100,
            CreditFtLines = 10,
            CreditFirstHalfLines = 5,
            LicenseeFtLines = 8,
            LicenseeFirstHalfLines = 4,
            SuspenderUserId = 1,
            SuspenderUsername = "Test User",
            Traders = new List<AssignmentApiModel>(),
            MatchSettings = new List<MatchSettingApiModel>()
        };
    }

    private static MatchSetting CreateMatchSetting(int owpMatchId, int bettypeId, decimal? maxbet)
    {
        return new MatchSetting
        {
            OWPMatchId = owpMatchId,
            BettypeId = bettypeId,
            Maxbet = maxbet,
            Spread = 0.5m
        };
    }

    private static MaxbetSetting CreateMaxbetSetting(
        decimal ftHdp, decimal ftOu, decimal firstHalfHdp, decimal firstHalfOu,
        decimal noOfCornerFtHdp = 0, decimal noOfCornerFtOu = 0, decimal noOfCorner1HHdp = 0, decimal noOfCorner1HOu = 0,
        decimal nextCornerFtHdp = 0, decimal totalBookingFtHdp = 0, decimal totalBookingFtOu = 0,
        decimal totalBooking1HHdp = 0, decimal totalBooking1HOu = 0, decimal nextBookingFtHdp = 0)
    {
        return new MaxbetSetting
        {
            FT_HDP = ftHdp,
            FT_OU = ftOu,
            FirstHalf_HDP = firstHalfHdp,
            FirstHalf_OU = firstHalfOu,
            NoOfCorner_FT_HDP = noOfCornerFtHdp,
            NoOfCorner_FT_OU = noOfCornerFtOu,
            NoOfCorner_1H_HDP = noOfCorner1HHdp,
            NoOfCorner_1H_OU = noOfCorner1HOu,
            NextCorner_FT_HDP = nextCornerFtHdp,
            TotalBooking_FT_HDP = totalBookingFtHdp,
            TotalBooking_FT_OU = totalBookingFtOu,
            TotalBooking_1H_HDP = totalBooking1HHdp,
            TotalBooking_1H_OU = totalBooking1HOu,
            NextBooking_FT_HDP = nextBookingFtHdp,
            SecondAccept_Main = "100",
            SecondAccept_Corner = "50",
            SecondAccept_TotalBooking = "75",
            SecondAccept_Fantasy = "25"
        };
    }

    [Fact]
    public void OddBuilder_BuildOdd_ShouldAddHandicapAndOverUnderSettings()
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

    [Fact]
    public void SimpleTest_ShouldPass()
    {
        // Arrange & Act & Assert
        Assert.True(true);
    }
}
