using FluentAssertions;
using OWP.Extensions;
using Xunit;

namespace OWP.UnitTests.Extensions;

public class DictionaryExtensionsTests
{
    [Fact]
    public void GetValue_WhenKeyExists_ShouldReturnValue()
    {
        // Arrange
        var dictionary = new Dictionary<string, int> { { "key1", 42 } };
        var defaultValue = 0;

        // Act
        var result = dictionary.GetValue("key1", defaultValue);

        // Assert
        result.Should().Be(42);
    }

    [Fact]
    public void GetValue_WhenKeyDoesNotExist_ShouldReturnDefaultValue()
    {
        // Arrange
        var dictionary = new Dictionary<string, int> { { "key1", 42 } };
        var defaultValue = 99;

        // Act
        var result = dictionary.GetValue("nonexistent", defaultValue);

        // Assert
        result.Should().Be(defaultValue);
    }

    [Fact]
    public void GetValue_WhenDictionaryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        Dictionary<string, int> dictionary = null!;
        var defaultValue = 0;

        // Act & Assert
        var action = () => dictionary.GetValue("key1", defaultValue);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("dictionary");
    }

    [Fact]
    public void LookupByKey_WhenExactMatchExists_ShouldReturnValue()
    {
        // Arrange
        var dictionary = new Dictionary<string, int> { { "test", 42 }, { "example", 99 } };

        // Act
        var result = dictionary.LookupByKey("test", 0);

        // Assert
        result.Should().Be(42);
    }

    [Fact]
    public void LookupByKey_WhenPartialMatchExists_ShouldReturnValue()
    {
        // Arrange
        var dictionary = new Dictionary<string, int> { { "test", 42 }, { "example", 99 } };

        // Act
        var result = dictionary.LookupByKey("testing", 0);

        // Assert
        result.Should().Be(42);
    }

    [Fact]
    public void LookupByKey_WhenCaseInsensitiveMatchExists_ShouldReturnValue()
    {
        // Arrange
        var dictionary = new Dictionary<string, int> { { "Test", 42 }, { "Example", 99 } };

        // Act
        var result = dictionary.LookupByKey("test", 0);

        // Assert
        result.Should().Be(42);
    }

    [Fact]
    public void LookupByKey_WhenNoMatchExists_ShouldReturnDefaultValue()
    {
        // Arrange
        var dictionary = new Dictionary<string, int> { { "test", 42 }, { "example", 99 } };
        var defaultValue = 999;

        // Act
        var result = dictionary.LookupByKey("nonexistent", defaultValue);

        // Assert
        result.Should().Be(defaultValue);
    }

    [Fact]
    public void LookupByKey_WhenKeyIsNull_ShouldThrowException()
    {
        // Arrange
        var dictionary = new Dictionary<string, int> { { "test", 42 } };
        var defaultValue = 999;

        // Act & Assert
        var action = () => dictionary.LookupByKey(null!, defaultValue);
        action.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void LookupByKey_WhenKeyIsEmpty_ShouldReturnDefaultValue()
    {
        // Arrange
        var dictionary = new Dictionary<string, int> { { "test", 42 } };
        var defaultValue = 999;

        // Act
        var result = dictionary.LookupByKey("", defaultValue);

        // Assert
        result.Should().Be(defaultValue);
    }
}
