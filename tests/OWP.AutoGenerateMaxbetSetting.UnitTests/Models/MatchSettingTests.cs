using OWP.AutoGenerateMaxbetSetting.Models;

namespace OWP.AutoGenerateMaxbetSetting.UnitTests.Models;

public class MatchSettingTests
{
    [Fact]
    public void Constructor_ShouldCreateInstanceWithDefaultValues()
    {
        // Act
        var matchSetting = new MatchSetting();

        // Assert
        matchSetting.Should().NotBeNull();
        matchSetting.OwpMatchId.Should().Be(0);
        matchSetting.Bettype.Should().Be(0);
        matchSetting.Spread.Should().BeNull();
        matchSetting.Maxbet.Should().BeNull();
    }

    [Theory]
    [InlineData(1, 100, "1.5", "1000.0")]
    [InlineData(2, 200, "2.0", "2000.0")]
    [InlineData(3, 300, null, null)]
    [InlineData(4, 400, "0.5", "500.0")]
    public void Properties_ShouldSetAndGetCorrectValues(int owpMatchId, int bettype, string? spreadStr, string? maxbetStr)
    {
        // Arrange
        var matchSetting = new MatchSetting();
        var spread = string.IsNullOrEmpty(spreadStr) ? (decimal?)null : decimal.Parse(spreadStr);
        var maxbet = string.IsNullOrEmpty(maxbetStr) ? (decimal?)null : decimal.Parse(maxbetStr);

        // Act
        matchSetting.OwpMatchId = owpMatchId;
        matchSetting.Bettype = bettype;
        matchSetting.Spread = spread;
        matchSetting.Maxbet = maxbet;

        // Assert
        matchSetting.OwpMatchId.Should().Be(owpMatchId);
        matchSetting.Bettype.Should().Be(bettype);
        matchSetting.Spread.Should().Be(spread);
        matchSetting.Maxbet.Should().Be(maxbet);
    }

    [Fact]
    public void Properties_ShouldAllowNullValues()
    {
        // Arrange
        var matchSetting = new MatchSetting();

        // Act
        matchSetting.Spread = null;
        matchSetting.Maxbet = null;

        // Assert
        matchSetting.Spread.Should().BeNull();
        matchSetting.Maxbet.Should().BeNull();
    }

    [Fact]
    public void ObjectInitializer_ShouldWorkCorrectly()
    {
        // Act
        var matchSetting = new MatchSetting
        {
            OwpMatchId = 100,
            Bettype = 1,
            Spread = 1.5m,
            Maxbet = 1000m
        };

        // Assert
        matchSetting.OwpMatchId.Should().Be(100);
        matchSetting.Bettype.Should().Be(1);
        matchSetting.Spread.Should().Be(1.5m);
        matchSetting.Maxbet.Should().Be(1000m);
    }

    [Theory]
    [InlineData(0, 0, null, null)]
    [InlineData(int.MaxValue, int.MaxValue, "9999999999999999999999999999", "9999999999999999999999999999")]
    [InlineData(int.MinValue, int.MinValue, "-9999999999999999999999999999", "-9999999999999999999999999999")]
    public void Properties_ShouldHandleEdgeCases(int owpMatchId, int bettype, string? spreadStr, string? maxbetStr)
    {
        // Arrange
        var matchSetting = new MatchSetting();
        var spread = string.IsNullOrEmpty(spreadStr) ? (decimal?)null : decimal.Parse(spreadStr);
        var maxbet = string.IsNullOrEmpty(maxbetStr) ? (decimal?)null : decimal.Parse(maxbetStr);

        // Act
        matchSetting.OwpMatchId = owpMatchId;
        matchSetting.Bettype = bettype;
        matchSetting.Spread = spread;
        matchSetting.Maxbet = maxbet;

        // Assert
        matchSetting.OwpMatchId.Should().Be(owpMatchId);
        matchSetting.Bettype.Should().Be(bettype);
        matchSetting.Spread.Should().Be(spread);
        matchSetting.Maxbet.Should().Be(maxbet);
    }
}
