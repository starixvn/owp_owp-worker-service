using OWP.AutoGenerateMaxbetSetting.Models;

namespace OWP.AutoGenerateMaxbetSetting.UnitTests.Models;

public class LeagueSettingTests
{
    [Fact]
    public void Constructor_ShouldCreateInstanceWithDefaultValues()
    {
        // Act
        var leagueSetting = new LeagueSetting();

        // Assert
        leagueSetting.Should().NotBeNull();
        leagueSetting.OwpLeagueId.Should().Be(0);
        leagueSetting.HasTv.Should().BeNull();
        leagueSetting.MaxbetWd1H.Should().BeNull();
        leagueSetting.MaxbetWdHdp.Should().BeNull();
        leagueSetting.MaxbetWdOu.Should().BeNull();
        leagueSetting.MaxbetWk1H.Should().BeNull();
        leagueSetting.MaxbetWkHdp.Should().BeNull();
        leagueSetting.MaxbetWkOu.Should().BeNull();
        leagueSetting.Spread1Hhdp.Should().BeNull();
        leagueSetting.Spread1Hou.Should().BeNull();
        leagueSetting.SpreadHdp.Should().BeNull();
        leagueSetting.SpreadOu.Should().BeNull();
        leagueSetting.WDNoOfHDP.Should().BeNull();
        leagueSetting.WKNoOfHDP.Should().BeNull();
    }

    [Theory]
    [InlineData(1, true, "1000.0", "2000.0", "3000.0", "4000.0", "5000.0", "6000.0", "1.5", "2.0", "3.0", "4.0", "2", "3")]
    [InlineData(2, false, "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "", "")]
    [InlineData(3, false, "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "0.0", "", "")]
    public void Properties_ShouldSetAndGetCorrectValues(int owpLeagueId, bool? hasTv, string maxbetWd1HStr, string maxbetWdHdpStr,
        string maxbetWdOuStr, string maxbetWk1HStr, string maxbetWkHdpStr, string maxbetWkOuStr,
        string spread1HhdpStr, string spread1HouStr, string spreadHdpStr, string spreadOuStr,
        string wdNoOfHDP, string wkNoOfHDP)
    {
        // Arrange
        var leagueSetting = new LeagueSetting();
        var maxbetWd1H = string.IsNullOrEmpty(maxbetWd1HStr) ? (decimal?)null : decimal.Parse(maxbetWd1HStr);
        var maxbetWdHdp = string.IsNullOrEmpty(maxbetWdHdpStr) ? (decimal?)null : decimal.Parse(maxbetWdHdpStr);
        var maxbetWdOu = string.IsNullOrEmpty(maxbetWdOuStr) ? (decimal?)null : decimal.Parse(maxbetWdOuStr);
        var maxbetWk1H = string.IsNullOrEmpty(maxbetWk1HStr) ? (decimal?)null : decimal.Parse(maxbetWk1HStr);
        var maxbetWkHdp = string.IsNullOrEmpty(maxbetWkHdpStr) ? (decimal?)null : decimal.Parse(maxbetWkHdpStr);
        var maxbetWkOu = string.IsNullOrEmpty(maxbetWkOuStr) ? (decimal?)null : decimal.Parse(maxbetWkOuStr);
        var spread1Hhdp = string.IsNullOrEmpty(spread1HhdpStr) ? (decimal?)null : decimal.Parse(spread1HhdpStr);
        var spread1Hou = string.IsNullOrEmpty(spread1HouStr) ? (decimal?)null : decimal.Parse(spread1HouStr);
        var spreadHdp = string.IsNullOrEmpty(spreadHdpStr) ? (decimal?)null : decimal.Parse(spreadHdpStr);
        var spreadOu = string.IsNullOrEmpty(spreadOuStr) ? (decimal?)null : decimal.Parse(spreadOuStr);

        // Act
        leagueSetting.OwpLeagueId = owpLeagueId;
        leagueSetting.HasTv = hasTv;
        leagueSetting.MaxbetWd1H = maxbetWd1H;
        leagueSetting.MaxbetWdHdp = maxbetWdHdp;
        leagueSetting.MaxbetWdOu = maxbetWdOu;
        leagueSetting.MaxbetWk1H = maxbetWk1H;
        leagueSetting.MaxbetWkHdp = maxbetWkHdp;
        leagueSetting.MaxbetWkOu = maxbetWkOu;
        leagueSetting.Spread1Hhdp = spread1Hhdp;
        leagueSetting.Spread1Hou = spread1Hou;
        leagueSetting.SpreadHdp = spreadHdp;
        leagueSetting.SpreadOu = spreadOu;
        leagueSetting.WDNoOfHDP = wdNoOfHDP;
        leagueSetting.WKNoOfHDP = wkNoOfHDP;

        // Assert
        leagueSetting.OwpLeagueId.Should().Be(owpLeagueId);
        leagueSetting.HasTv.Should().Be(hasTv);
        leagueSetting.MaxbetWd1H.Should().Be(maxbetWd1H);
        leagueSetting.MaxbetWdHdp.Should().Be(maxbetWdHdp);
        leagueSetting.MaxbetWdOu.Should().Be(maxbetWdOu);
        leagueSetting.MaxbetWk1H.Should().Be(maxbetWk1H);
        leagueSetting.MaxbetWkHdp.Should().Be(maxbetWkHdp);
        leagueSetting.MaxbetWkOu.Should().Be(maxbetWkOu);
        leagueSetting.Spread1Hhdp.Should().Be(spread1Hhdp);
        leagueSetting.Spread1Hou.Should().Be(spread1Hou);
        leagueSetting.SpreadHdp.Should().Be(spreadHdp);
        leagueSetting.SpreadOu.Should().Be(spreadOu);
        leagueSetting.WDNoOfHDP.Should().Be(wdNoOfHDP);
        leagueSetting.WKNoOfHDP.Should().Be(wkNoOfHDP);
    }

    [Fact]
    public void Properties_ShouldAllowNullValues()
    {
        // Arrange
        var leagueSetting = new LeagueSetting();

        // Act
        leagueSetting.HasTv = null;
        leagueSetting.MaxbetWd1H = null;
        leagueSetting.MaxbetWdHdp = null;
        leagueSetting.MaxbetWdOu = null;
        leagueSetting.MaxbetWk1H = null;
        leagueSetting.MaxbetWkHdp = null;
        leagueSetting.MaxbetWkOu = null;
        leagueSetting.Spread1Hhdp = null;
        leagueSetting.Spread1Hou = null;
        leagueSetting.SpreadHdp = null;
        leagueSetting.SpreadOu = null;
        leagueSetting.WDNoOfHDP = null;
        leagueSetting.WKNoOfHDP = null;

        // Assert
        leagueSetting.HasTv.Should().BeNull();
        leagueSetting.MaxbetWd1H.Should().BeNull();
        leagueSetting.MaxbetWdHdp.Should().BeNull();
        leagueSetting.MaxbetWdOu.Should().BeNull();
        leagueSetting.MaxbetWk1H.Should().BeNull();
        leagueSetting.MaxbetWkHdp.Should().BeNull();
        leagueSetting.MaxbetWkOu.Should().BeNull();
        leagueSetting.Spread1Hhdp.Should().BeNull();
        leagueSetting.Spread1Hou.Should().BeNull();
        leagueSetting.SpreadHdp.Should().BeNull();
        leagueSetting.SpreadOu.Should().BeNull();
        leagueSetting.WDNoOfHDP.Should().BeNull();
        leagueSetting.WKNoOfHDP.Should().BeNull();
    }

    [Fact]
    public void ObjectInitializer_ShouldWorkCorrectly()
    {
        // Act
        var leagueSetting = new LeagueSetting
        {
            OwpLeagueId = 1,
            HasTv = true,
            MaxbetWd1H = 1000.0m,
            MaxbetWdHdp = 2000.0m,
            MaxbetWdOu = 3000.0m,
            MaxbetWk1H = 4000.0m,
            MaxbetWkHdp = 5000.0m,
            MaxbetWkOu = 6000.0m,
            Spread1Hhdp = 1.5m,
            Spread1Hou = 2.0m,
            SpreadHdp = 3.0m,
            SpreadOu = 4.0m,
            WDNoOfHDP = "2",
            WKNoOfHDP = "3"
        };

        // Assert
        leagueSetting.OwpLeagueId.Should().Be(1);
        leagueSetting.HasTv.Should().BeTrue();
        leagueSetting.MaxbetWd1H.Should().Be(1000.0m);
        leagueSetting.MaxbetWdHdp.Should().Be(2000.0m);
        leagueSetting.MaxbetWdOu.Should().Be(3000.0m);
        leagueSetting.MaxbetWk1H.Should().Be(4000.0m);
        leagueSetting.MaxbetWkHdp.Should().Be(5000.0m);
        leagueSetting.MaxbetWkOu.Should().Be(6000.0m);
        leagueSetting.Spread1Hhdp.Should().Be(1.5m);
        leagueSetting.Spread1Hou.Should().Be(2.0m);
        leagueSetting.SpreadHdp.Should().Be(3.0m);
        leagueSetting.SpreadOu.Should().Be(4.0m);
        leagueSetting.WDNoOfHDP.Should().Be("2");
        leagueSetting.WKNoOfHDP.Should().Be("3");
    }
}
