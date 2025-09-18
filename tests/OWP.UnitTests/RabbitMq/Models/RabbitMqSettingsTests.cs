using FluentAssertions;
using OWP.RabbitMq.Models;
using Xunit;

namespace OWP.UnitTests.RabbitMq.Models;

public class RabbitMqSettingsTests
{
    [Fact]
    public void RabbitMq_Constant_ShouldReturnCorrectValue()
    {
        // Act & Assert
        RabbitMqSetting.RabbitMq.Should().Be("RabbitMQ");
    }

    [Fact]
    public void RabbitMqSetting_ShouldHaveAllRequiredProperties()
    {
        // Arrange
        var settings = new RabbitMqSetting();

        // Act & Assert
        settings.Should().NotBeNull();
        settings.Hosting.Should().BeNull();
        settings.VirtualHost.Should().BeNull();
        settings.UserName.Should().BeNull();
        settings.Password.Should().BeNull();
        settings.Port.Should().Be(0);
        settings.RequestQueue.Should().BeNull();
    }

    [Fact]
    public void RabbitMqSetting_ShouldAllowSettingProperties()
    {
        // Arrange
        var settings = new RabbitMqSetting
        {
            Hosting = "localhost",
            VirtualHost = "/",
            UserName = "guest",
            Password = "guest",
            Port = 5672,
            RequestQueue = "test.queue"
        };

        // Act & Assert
        settings.Hosting.Should().Be("localhost");
        settings.VirtualHost.Should().Be("/");
        settings.UserName.Should().Be("guest");
        settings.Password.Should().Be("guest");
        settings.Port.Should().Be(5672);
        settings.RequestQueue.Should().Be("test.queue");
    }
}
