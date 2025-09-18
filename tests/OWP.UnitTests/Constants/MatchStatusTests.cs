using FluentAssertions;
using OWP.Constants;
using Xunit;

namespace OWP.UnitTests.Constants;

public class MatchStatusTests
{
    [Fact]
    public void Live_ShouldReturnCorrectValue()
    {
        // Act & Assert
        MatchStatus.Live.Should().Be("Live");
    }

    [Fact]
    public void Cancel_ShouldReturnCorrectValue()
    {
        // Act & Assert
        MatchStatus.Cancel.Should().Be("Cancel");
    }

    [Fact]
    public void Suspended_ShouldReturnCorrectValue()
    {
        // Act & Assert
        MatchStatus.Suspended.Should().Be("Suspended");
    }

    [Fact]
    public void Postponed_ShouldReturnCorrectValue()
    {
        // Act & Assert
        MatchStatus.Postponed.Should().Be("Postponed");
    }

    [Fact]
    public void AllConstants_ShouldBeUnique()
    {
        // Arrange
        var constants = new[]
        {
            MatchStatus.Live,
            MatchStatus.Cancel,
            MatchStatus.Suspended,
            MatchStatus.Postponed
        };

        // Act & Assert
        constants.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void AllConstants_ShouldNotBeNullOrEmpty()
    {
        // Arrange
        var constants = new[]
        {
            MatchStatus.Live,
            MatchStatus.Cancel,
            MatchStatus.Suspended,
            MatchStatus.Postponed
        };

        // Act & Assert
        constants.Should().NotContainNulls();
        constants.Should().NotContain("");
    }
}
