using FluentAssertions;
using OWP.RabbitMq.Consts;
using Xunit;

namespace OWP.UnitTests.RabbitMq.Consts;

public class RoutingKeysTests
{
    [Fact]
    public void LinkedLgaMatch_ShouldReturnCorrectValue()
    {
        // Act & Assert
        RoutingKeys.LinkedLgaMatch.Should().Be("linked-lga-match");
    }

    [Fact]
    public void PrematchNewLeague_ShouldReturnCorrectValue()
    {
        // Act & Assert
        RoutingKeys.PrematchNewLeague.Should().Be("prematch-new-league");
    }

    [Fact]
    public void AllRoutingKeys_ShouldBeUnique()
    {
        // Arrange
        var routingKeys = new[]
        {
            RoutingKeys.LinkedLgaMatch,
            RoutingKeys.PrematchNewLeague
        };

        // Act & Assert
        routingKeys.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void AllRoutingKeys_ShouldNotBeNullOrEmpty()
    {
        // Arrange
        var routingKeys = new[]
        {
            RoutingKeys.LinkedLgaMatch,
            RoutingKeys.PrematchNewLeague
        };

        // Act & Assert
        routingKeys.Should().NotContainNulls();
        routingKeys.Should().NotContain("");
    }

    [Fact]
    public void AllRoutingKeys_ShouldBeValidRoutingKeyFormat()
    {
        // Arrange
        var routingKeys = new[]
        {
            RoutingKeys.LinkedLgaMatch,
            RoutingKeys.PrematchNewLeague
        };

        // Act & Assert
        foreach (var routingKey in routingKeys)
        {
            routingKey.Should().MatchRegex("^[a-z-]+$", "routing keys should be lowercase with hyphens");
            routingKey.Should().NotStartWith("-", "routing keys should not start with hyphen");
            routingKey.Should().NotEndWith("-", "routing keys should not end with hyphen");
        }
    }
}
