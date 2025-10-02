using FluentAssertions;
using OWP.Constants;
using Xunit;

namespace OWP.UnitTests.Constants;

public class ProviderConstTests
{
    [Theory]
    [InlineData(3, "365GV")]
    [InlineData(15, "IMG")]
    [InlineData(17, "RB2")]
    [InlineData(18, "RTS")]
    [InlineData(19, "BG")]
    [InlineData(20, "BET")]
    [InlineData(202, "BET2")]
    public void Provider_ShouldContainCorrectMappings(int key, string expectedValue)
    {
        // Act
        var result = ProviderConst.Provider[key];

        // Assert
        result.Should().Be(expectedValue);
    }

    [Fact]
    public void Provider_ShouldContainAllExpectedProviders()
    {
        // Arrange
        var expectedProviders = new Dictionary<int, string>
        {
            { 3, "365GV" },
            { 15, "IMG" },
            { 17, "RB2" },
            { 18, "RTS" },
            { 19, "BG" },
            { 20, "BET" },
            { 202, "BET2" }
        };

        // Act & Assert
        foreach (var expected in expectedProviders)
        {
            ProviderConst.Provider.Should().ContainKey(expected.Key);
            ProviderConst.Provider[expected.Key].Should().Be(expected.Value);
        }
    }

    [Fact]
    public void Provider_ShouldHaveCorrectCount()
    {
        // Act & Assert
        ProviderConst.Provider.Should().HaveCount(7);
    }

    [Fact]
    public void Provider_ShouldBeImmutable()
    {
        // Act & Assert
        ProviderConst.Provider.Should().BeOfType<System.Collections.Immutable.ImmutableDictionary<int, string>>();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(21)]
    [InlineData(999)]
    public void Provider_ShouldNotContainUnexpectedKeys(int key)
    {
        // Act & Assert
        ProviderConst.Provider.Should().NotContainKey(key);
    }

    [Fact]
    public void Provider_Values_ShouldNotBeNullOrEmpty()
    {
        // Act & Assert
        ProviderConst.Provider.Values.Should().NotContainNulls();
        ProviderConst.Provider.Values.Should().NotContain("");
    }
}
