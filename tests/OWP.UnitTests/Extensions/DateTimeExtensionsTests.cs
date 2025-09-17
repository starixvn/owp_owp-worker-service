using FluentAssertions;
using OWP.Extensions;
using Xunit;

namespace OWP.UnitTests.Extensions;

public class DateTimeExtensionsTests
{
    [Fact]
    public void ToUtc8_ShouldAdd8Hours()
    {
        // Arrange
        var utc0 = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var expected = new DateTime(2024, 1, 1, 20, 0, 0, DateTimeKind.Utc);

        // Act
        var result = utc0.ToUtc8();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ToUtc0_ShouldSubtract8Hours()
    {
        // Arrange
        var utc8 = new DateTime(2024, 1, 1, 20, 0, 0, DateTimeKind.Utc);
        var expected = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        // Act
        var result = utc8.ToUtc0();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ToMax_WhenInstanceIsGreater_ShouldReturnInstance()
    {
        // Arrange
        var instance = new DateTime(2024, 1, 2, 12, 0, 0);
        var other = new DateTime(2024, 1, 1, 12, 0, 0);

        // Act
        var result = instance.ToMax(other);

        // Assert
        result.Should().Be(instance);
    }

    [Fact]
    public void ToMax_WhenOtherIsGreater_ShouldReturnOther()
    {
        // Arrange
        var instance = new DateTime(2024, 1, 1, 12, 0, 0);
        var other = new DateTime(2024, 1, 2, 12, 0, 0);

        // Act
        var result = instance.ToMax(other);

        // Assert
        result.Should().Be(other);
    }

    [Fact]
    public void ToMax_WhenEqual_ShouldReturnInstance()
    {
        // Arrange
        var instance = new DateTime(2024, 1, 1, 12, 0, 0);
        var other = new DateTime(2024, 1, 1, 12, 0, 0);

        // Act
        var result = instance.ToMax(other);

        // Assert
        result.Should().Be(instance);
    }
}
