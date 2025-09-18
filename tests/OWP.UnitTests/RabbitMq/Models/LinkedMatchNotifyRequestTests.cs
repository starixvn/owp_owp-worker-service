using FluentAssertions;
using OWP.RabbitMq.Models;
using Xunit;

namespace OWP.UnitTests.RabbitMq.Models;

public class LinkedMatchNotifyRequestTests
{
    [Fact]
    public void Constructor_ShouldSetCorrectRoutingKey()
    {
        // Act
        var request = new LinkedMatchNotifyRequest();

        // Assert
        request.RoutingKey.Should().Be("linked-lga-match");
    }

    [Fact]
    public void LinkedMatchNotifyRequest_ShouldHaveAllRequiredProperties()
    {
        // Arrange
        var request = new LinkedMatchNotifyRequest();

        // Act & Assert
        request.Should().NotBeNull();
        request.SessionId.Should().BeNull();
        request.EventDate.Should().Be(default(DateTime));
    }

    [Fact]
    public void LinkedMatchNotifyRequest_ShouldAllowSettingProperties()
    {
        // Arrange
        var sessionId = "test-session-123";
        var eventDate = new DateTime(2024, 1, 15, 14, 30, 0);
        var request = new LinkedMatchNotifyRequest
        {
            SessionId = sessionId,
            EventDate = eventDate
        };

        // Act & Assert
        request.SessionId.Should().Be(sessionId);
        request.EventDate.Should().Be(eventDate);
        request.RoutingKey.Should().Be("linked-lga-match");
    }

    [Fact]
    public void LinkedMatchNotifyRequest_ShouldInheritFromRequestBaseModel()
    {
        // Act
        var request = new LinkedMatchNotifyRequest();

        // Assert
        request.Should().BeAssignableTo<RequestBaseModel>();
    }
}
