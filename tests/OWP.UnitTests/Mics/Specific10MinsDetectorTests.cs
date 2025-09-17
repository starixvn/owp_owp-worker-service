using FluentAssertions;
using OWP.Constants;
using OWP.Mics;
using Xunit;

namespace OWP.UnitTests.Mics;

public class Specific10MinsDetectorTests
{
    private readonly Specific10MinsDetector _detector;

    public Specific10MinsDetectorTests()
    {
        _detector = new Specific10MinsDetector();
    }

    [Theory]
    [InlineData(1900, 1900)] // Lower bound
    [InlineData(1940, 1900)] // Middle
    [InlineData(1980, 1900)] // Upper bound
    [InlineData(1950, 1900)] // Random in range
    public void GetRepresentativeSubtype_WhenInRange_ShouldReturnRepresentativeSubType(int subType, int expected)
    {
        // Act
        var result = _detector.GetRepresentativeSubtype(subType);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(1899)] // Below range
    [InlineData(1981)] // Above range
    [InlineData(1000)] // Far below
    [InlineData(3000)] // Far above
    [InlineData(0)] // Zero
    [InlineData(-1)] // Negative
    public void GetRepresentativeSubtype_WhenOutOfRange_ShouldDelegateToNextType(int subType)
    {
        // Arrange
        var nextDetector = new MockRepresentativeSubTypeDetector(999);
        _detector.NextType = nextDetector;

        // Act
        var result = _detector.GetRepresentativeSubtype(subType);

        // Assert
        result.Should().Be(999);
    }

    [Fact]
    public void GetRepresentativeSubtype_WhenOutOfRangeAndNoNextType_ShouldReturnOriginalSubType()
    {
        // Arrange
        var subType = 1000;

        // Act
        var result = _detector.GetRepresentativeSubtype(subType);

        // Assert
        result.Should().Be(subType);
    }

    [Fact]
    public void GetRepresentativeSubtype_ShouldUseCorrectRangeConstants()
    {
        // Act & Assert
        SubType.Specific10MinsFromSubType.Should().Be(1900);
        SubType.Specific10MinsToSubType.Should().Be(1980);
    }

    private class MockRepresentativeSubTypeDetector : RepresentativeSubTypeDetector
    {
        private readonly int _returnValue;

        public MockRepresentativeSubTypeDetector(int returnValue)
        {
            _returnValue = returnValue;
        }

        public override int GetRepresentativeSubtype(int subType)
        {
            return _returnValue;
        }
    }
}
