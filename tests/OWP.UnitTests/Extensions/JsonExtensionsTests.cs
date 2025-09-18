using FluentAssertions;
using Newtonsoft.Json;
using OWP.Extensions;
using Xunit;

namespace OWP.UnitTests.Extensions;

public class JsonExtensionsTests
{
    private readonly JsonSerializerSettings _settings;

    public JsonExtensionsTests()
    {
        _settings = new JsonSerializerSettings
        {
            Converters = { new TrimmingConverter() }
        };
    }

    [Fact]
    public void TrimmingConverter_CanRead_ShouldReturnTrue()
    {
        // Arrange
        var converter = new TrimmingConverter();

        // Act
        var result = converter.CanRead;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void TrimmingConverter_CanWrite_ShouldReturnFalse()
    {
        // Arrange
        var converter = new TrimmingConverter();

        // Act
        var result = converter.CanWrite;

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(typeof(string), true)]
    [InlineData(typeof(int), false)]
    [InlineData(typeof(object), false)]
    [InlineData(typeof(DateTime), false)]
    public void TrimmingConverter_CanConvert_ShouldReturnCorrectResult(Type objectType, bool expected)
    {
        // Arrange
        var converter = new TrimmingConverter();

        // Act
        var result = converter.CanConvert(objectType);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("  hello  ", "hello")]
    [InlineData("hello", "hello")]
    [InlineData("", "")]
    [InlineData(null, "")]
    [InlineData("   ", "")]
    [InlineData("\t\n\r", "")]
    public void TrimmingConverter_ReadJson_ShouldTrimString(string input, string expected)
    {
        // Arrange
        var converter = new TrimmingConverter();
        var json = input == null ? "null" : $"\"{input}\"";
        var reader = new JsonTextReader(new StringReader(json));
        reader.Read(); // Move to the token

        // Act
        var result = converter.ReadJson(reader, typeof(string), null, JsonSerializer.CreateDefault());

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void TrimmingConverter_WriteJson_ShouldThrowNotImplementedException()
    {
        // Arrange
        var converter = new TrimmingConverter();
        var writer = new JsonTextWriter(new StringWriter());

        // Act & Assert
        var action = () => converter.WriteJson(writer, "test", JsonSerializer.CreateDefault());
        action.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void TrimmingConverter_IntegrationTest_ShouldTrimStringsInObject()
    {
        // Arrange
        var json = "{\"Name\":\"  John Doe  \",\"Age\":30,\"Email\":\"  john@example.com  \"}";
        var settings = new JsonSerializerSettings
        {
            Converters = { new TrimmingConverter() }
        };

        // Act
        var result = JsonConvert.DeserializeObject<TestObject>(json, settings);

        // Assert
        result.Name.Should().Be("John Doe");
        result.Email.Should().Be("john@example.com");
        result.Age.Should().Be(30);
    }

    private class TestObject
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Age { get; set; }
    }
}
