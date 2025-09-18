using FluentAssertions;
using OWP.RabbitMq.Models;
using Xunit;

namespace OWP.UnitTests.RabbitMq.Models;

public class RequestBaseModelTests
{
    [Fact]
    public void RequestBaseModel_ShouldHaveRoutingKeyProperty()
    {
        // Arrange
        var model = new TestRequestBaseModel();

        // Act & Assert
        model.Should().NotBeNull();
        model.RoutingKey.Should().NotBeNull();
    }

    [Fact]
    public void RequestBaseModel_RoutingKey_ShouldBeProtectedSet()
    {
        // Arrange
        var model = new TestRequestBaseModel();

        // Act & Assert
        // The RoutingKey property should be accessible for reading
        model.RoutingKey.Should().Be("test-routing-key");
    }

    private class TestRequestBaseModel : RequestBaseModel
    {
        public TestRequestBaseModel()
        {
            RoutingKey = "test-routing-key";
        }
    }
}
