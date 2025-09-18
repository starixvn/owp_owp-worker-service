using FluentAssertions;
using OWP.RabbitMq.Models;
using Xunit;

namespace OWP.UnitTests.RabbitMq.Models;

public class PrematchNewLeagueNotifyRequestTests
{
    [Fact]
    public void Constructor_ShouldSetCorrectRoutingKey()
    {
        // Act
        var request = new PrematchNewLeagueNotifyRequest();

        // Assert
        request.RoutingKey.Should().Be("prematch-new-league");
    }

    [Fact]
    public void PrematchNewLeagueNotifyRequest_ShouldHaveAllRequiredProperties()
    {
        // Arrange
        var request = new PrematchNewLeagueNotifyRequest();

        // Act & Assert
        request.Should().NotBeNull();
        request.SessionId.Should().BeNull();
        request.FromDate.Should().Be(default(DateTime));
    }

    [Fact]
    public void PrematchNewLeagueNotifyRequest_ShouldAllowSettingProperties()
    {
        // Arrange
        var sessionId = "test-session-456";
        var fromDate = new DateTime(2024, 1, 15, 14, 30, 0);
        var request = new PrematchNewLeagueNotifyRequest
        {
            SessionId = sessionId,
            FromDate = fromDate
        };

        // Act & Assert
        request.SessionId.Should().Be(sessionId);
        request.FromDate.Should().Be(fromDate);
        request.RoutingKey.Should().Be("prematch-new-league");
    }

    [Fact]
    public void PrematchNewLeagueNotifyRequest_ShouldInheritFromRequestBaseModel()
    {
        // Act
        var request = new PrematchNewLeagueNotifyRequest();

        // Assert
        request.Should().BeAssignableTo<RequestBaseModel>();
    }
}
