using OWP.Provider.Share.AgentConfirm.Model;

namespace OWP.Provider.Share.UnitTests.Models;

public class AgentMatchInfoTests
{
    [Fact]
    public void Constructor_ShouldCreateInstanceWithDefaultValues()
    {
        // Act
        var matchInfo = new AgentMatchInfo();

        // Assert
        matchInfo.Should().NotBeNull();
        matchInfo.OWPMatchId.Should().Be(0);
        matchInfo.MatchId.Should().Be(0);
        matchInfo.MatchStatus.Should().BeNull();
        matchInfo.Provider.Should().BeNull();
        matchInfo.IsSaba.Should().BeFalse();
        matchInfo.InhouseProvider.Should().BeNull();
    }

    [Theory]
    [InlineData(1, 100, "Live", "365GV", true, "Betradar")]
    [InlineData(2, 200, "Cancel", "BG", false, "FC")]
    [InlineData(3, 300, "Postponed", "IMG", true, "RB2")]
    [InlineData(4, 400, "Suspended", "RTS", false, "SLR")]
    public void Properties_ShouldSetAndGetCorrectValues(int owpMatchId, int matchId, string matchStatus, 
        string provider, bool isSaba, string inhouseProvider)
    {
        // Arrange
        var matchInfo = new AgentMatchInfo();

        // Act
        matchInfo.OWPMatchId = owpMatchId;
        matchInfo.MatchId = matchId;
        matchInfo.MatchStatus = matchStatus;
        matchInfo.Provider = provider;
        matchInfo.IsSaba = isSaba;
        matchInfo.InhouseProvider = inhouseProvider;

        // Assert
        matchInfo.OWPMatchId.Should().Be(owpMatchId);
        matchInfo.MatchId.Should().Be(matchId);
        matchInfo.MatchStatus.Should().Be(matchStatus);
        matchInfo.Provider.Should().Be(provider);
        matchInfo.IsSaba.Should().Be(isSaba);
        matchInfo.InhouseProvider.Should().Be(inhouseProvider);
    }

    [Fact]
    public void Properties_ShouldAllowNullValues()
    {
        // Arrange
        var matchInfo = new AgentMatchInfo();

        // Act
        matchInfo.MatchStatus = null;
        matchInfo.Provider = null;
        matchInfo.InhouseProvider = null;

        // Assert
        matchInfo.MatchStatus.Should().BeNull();
        matchInfo.Provider.Should().BeNull();
        matchInfo.InhouseProvider.Should().BeNull();
    }

    [Fact]
    public void Properties_ShouldAllowEmptyStrings()
    {
        // Arrange
        var matchInfo = new AgentMatchInfo();

        // Act
        matchInfo.MatchStatus = "";
        matchInfo.Provider = "";
        matchInfo.InhouseProvider = "";

        // Assert
        matchInfo.MatchStatus.Should().Be("");
        matchInfo.Provider.Should().Be("");
        matchInfo.InhouseProvider.Should().Be("");
    }

    [Fact]
    public void IsSaba_ShouldDefaultToFalse()
    {
        // Act
        var matchInfo = new AgentMatchInfo();

        // Assert
        matchInfo.IsSaba.Should().BeFalse();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsSaba_ShouldSetCorrectValue(bool isSaba)
    {
        // Arrange
        var matchInfo = new AgentMatchInfo();

        // Act
        matchInfo.IsSaba = isSaba;

        // Assert
        matchInfo.IsSaba.Should().Be(isSaba);
    }

    [Fact]
    public void ObjectInitializer_ShouldWorkCorrectly()
    {
        // Act
        var matchInfo = new AgentMatchInfo
        {
            OWPMatchId = 123,
            MatchId = 456,
            MatchStatus = "Live",
            Provider = "365GV",
            IsSaba = true,
            InhouseProvider = "Betradar"
        };

        // Assert
        matchInfo.OWPMatchId.Should().Be(123);
        matchInfo.MatchId.Should().Be(456);
        matchInfo.MatchStatus.Should().Be("Live");
        matchInfo.Provider.Should().Be("365GV");
        matchInfo.IsSaba.Should().BeTrue();
        matchInfo.InhouseProvider.Should().Be("Betradar");
    }
}

