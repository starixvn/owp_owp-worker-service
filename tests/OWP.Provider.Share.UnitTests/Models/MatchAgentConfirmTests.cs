using OWP.Provider.Share.AgentConfirm;

namespace OWP.Provider.Share.UnitTests.Models;

public class MatchAgentConfirmTests
{
    [Fact]
    public void Constructor_ShouldCreateInstanceWithDefaultValues()
    {
        // Act
        var agentConfirm = new MatchAgentConfirm();

        // Assert
        agentConfirm.Should().NotBeNull();
        agentConfirm.OWPMatchId.Should().Be(0);
        agentConfirm.MatchId.Should().Be(0);
        agentConfirm.Agent.Should().BeNull();
    }

    [Theory]
    [InlineData(1, 100, "Streaming")]
    [InlineData(2, 200, "Cancel")]
    [InlineData(3, 300, "Betradar")]
    [InlineData(4, 400, "365GV")]
    [InlineData(5, 500, "BG")]
    public void Properties_ShouldSetAndGetCorrectValues(int owpMatchId, int matchId, string agent)
    {
        // Arrange
        var agentConfirm = new MatchAgentConfirm();

        // Act
        agentConfirm.OWPMatchId = owpMatchId;
        agentConfirm.MatchId = matchId;
        agentConfirm.Agent = agent;

        // Assert
        agentConfirm.OWPMatchId.Should().Be(owpMatchId);
        agentConfirm.MatchId.Should().Be(matchId);
        agentConfirm.Agent.Should().Be(agent);
    }

    [Fact]
    public void Agent_ShouldAllowNullValue()
    {
        // Arrange
        var agentConfirm = new MatchAgentConfirm();

        // Act
        agentConfirm.Agent = null;

        // Assert
        agentConfirm.Agent.Should().BeNull();
    }

    [Fact]
    public void Agent_ShouldAllowEmptyString()
    {
        // Arrange
        var agentConfirm = new MatchAgentConfirm();

        // Act
        agentConfirm.Agent = "";

        // Assert
        agentConfirm.Agent.Should().Be("");
    }

    [Fact]
    public void ObjectInitializer_ShouldWorkCorrectly()
    {
        // Act
        var agentConfirm = new MatchAgentConfirm
        {
            OWPMatchId = 123,
            MatchId = 456,
            Agent = "Streaming"
        };

        // Assert
        agentConfirm.OWPMatchId.Should().Be(123);
        agentConfirm.MatchId.Should().Be(456);
        agentConfirm.Agent.Should().Be("Streaming");
    }

    [Theory]
    [InlineData(0, 0, "")]
    [InlineData(1, 1, "")]
    [InlineData(int.MaxValue, int.MaxValue, "Test")]
    [InlineData(int.MinValue, int.MinValue, "Another Test")]
    public void Properties_ShouldHandleEdgeCases(int owpMatchId, int matchId, string agent)
    {
        // Arrange
        var agentConfirm = new MatchAgentConfirm();

        // Act
        agentConfirm.OWPMatchId = owpMatchId;
        agentConfirm.MatchId = matchId;
        agentConfirm.Agent = agent;

        // Assert
        agentConfirm.OWPMatchId.Should().Be(owpMatchId);
        agentConfirm.MatchId.Should().Be(matchId);
        agentConfirm.Agent.Should().Be(agent);
    }
}

