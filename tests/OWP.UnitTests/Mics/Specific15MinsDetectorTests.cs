using FluentAssertions;
using OWP.Constants;
using OWP.Mics;
using Xunit;

namespace OWP.UnitTests.Mics;

public class Specific15MinsDetectorTests
{
    private readonly Specific15MinsDetector _detector;

    public Specific15MinsDetectorTests()
    {
        _detector = new Specific15MinsDetector();
    }

    [Theory]
    [InlineData(1800, 1800)] // Lower bound - goals
    [InlineData(1837, 1800)] // Middle - goals
    [InlineData(1875, 1800)] // Upper bound - goals
    [InlineData(2700, 2700)] // Lower bound - corners
    [InlineData(2737, 2700)] // Middle - corners
    [InlineData(2775, 2700)] // Upper bound - corners
    [InlineData(3500, 3500)] // Lower bound - bookings
    [InlineData(3537, 3500)] // Middle - bookings
    [InlineData(3575, 3500)] // Upper bound - bookings
    public void GetRepresentativeSubtype_WhenInRange_ShouldReturnRepresentativeSubType(int subType, int expected)
    {
        // Act
        var result = _detector.GetRepresentativeSubtype(subType);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(1799)] // Below goals range
    [InlineData(1876)] // Above goals range
    [InlineData(2699)] // Below corners range
    [InlineData(2776)] // Above corners range
    [InlineData(3499)] // Below bookings range
    [InlineData(3576)] // Above bookings range
    [InlineData(1000)] // Far below
    [InlineData(4000)] // Far above
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
        SubType.Specific15MinsFromSubType.Should().Be(1800);
        SubType.Specific15MinsToSubType.Should().Be(1875);
        SubType.Specific15MinsCornersFromSubType.Should().Be(2700);
        SubType.Specific15MinsCornersToSubType.Should().Be(2775);
        SubType.Specific15MinsTotalBookingFromSubType.Should().Be(3500);
        SubType.Specific15MinsTotalBookingToSubType.Should().Be(3575);
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
