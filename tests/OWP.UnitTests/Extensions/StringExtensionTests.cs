using FluentAssertions;
using OWP.Extensions;
using Xunit;

namespace OWP.UnitTests.Extensions;

public class StringExtensionTests
{
    [Theory]
    [InlineData("Hello World", "world", true)]
    [InlineData("Hello World", "WORLD", true)]
    [InlineData("Hello World", "hello", true)]
    [InlineData("Hello World", "HELLO", true)]
    [InlineData("Hello World", "xyz", false)]
    [InlineData("Hello World", "", true)]
    [InlineData("", "test", false)]
    [InlineData(null, "test", false)]
    public void IsContainsIgnoreCase_ShouldReturnCorrectResult(string source, string value, bool expected)
    {
        // Act & Assert
        if (source == null)
        {
            var action = () => source.IsContainsIgnoreCase(value);
            action.Should().Throw<NullReferenceException>();
        }
        else
        {
            var result = source.IsContainsIgnoreCase(value);
            result.Should().Be(expected);
        }
    }

    [Theory]
    [InlineData("  hello  ", "hello")]
    [InlineData("hello", "hello")]
    [InlineData("", "")]
    [InlineData(null, "")]
    [InlineData("   ", "")]
    public void ConvertNullToEmpty_ShouldReturnCorrectResult(string input, string expected)
    {
        // Act
        var result = input.ConvertNullToEmpty();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("Hello World", "world", StringComparison.OrdinalIgnoreCase, true)]
    [InlineData("Hello World", "world", StringComparison.Ordinal, false)]
    [InlineData("Hello World", "WORLD", StringComparison.OrdinalIgnoreCase, true)]
    [InlineData("Hello World", "WORLD", StringComparison.Ordinal, false)]
    [InlineData("Hello World", "xyz", StringComparison.OrdinalIgnoreCase, false)]
    [InlineData("Hello World", "", StringComparison.OrdinalIgnoreCase, true)]
    [InlineData("", "test", StringComparison.OrdinalIgnoreCase, false)]
    [InlineData(null, "test", StringComparison.OrdinalIgnoreCase, false)]
    public void Contains_WithStringComparison_ShouldReturnCorrectResult(string source, string toCheck, StringComparison comparison, bool expected)
    {
        // Act
        var result = source?.Contains(toCheck, comparison) ?? false;

        // Assert
        result.Should().Be(expected);
    }
}
