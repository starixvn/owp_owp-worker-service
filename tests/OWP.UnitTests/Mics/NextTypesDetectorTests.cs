using FluentAssertions;
using OWP.Constants;
using OWP.Mics;
using Xunit;

namespace OWP.UnitTests.Mics;

public class NextTypesDetectorTests
{
    private readonly NextTypesDetector _detector;

    public NextTypesDetectorTests()
    {
        _detector = new NextTypesDetector();
    }

    [Theory]
    [InlineData(2101, 2100)] // Lower bound - next corner
    [InlineData(2150, 2100)] // Middle - next corner
    [InlineData(2199, 2100)] // Upper bound - next corner
    [InlineData(2901, 2900)] // Lower bound - next booking
    [InlineData(2950, 2900)] // Middle - next booking
    [InlineData(2999, 2900)] // Upper bound - next booking
    [InlineData(401, 400)] // Lower bound - next goal
    [InlineData(450, 400)] // Middle - next goal
    [InlineData(499, 400)] // Upper bound - next goal
    public void GetRepresentativeSubtype_WhenInRange_ShouldReturnRepresentativeSubType(int subType, int expected)
    {
        // Act
        var result = _detector.GetRepresentativeSubtype(subType);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(2100)] // Below next corner range
    [InlineData(2200)] // Above next corner range
    [InlineData(2900)] // Below next booking range
    [InlineData(3000)] // Above next booking range
    [InlineData(400)] // Below next goal range
    [InlineData(500)] // Above next goal range
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
        SubType.NextCornerFromSubType.Should().Be(2101);
        SubType.NextCornerToSubType.Should().Be(2199);
        SubType.NextCornerRepresentativeSubType.Should().Be(2100);
        SubType.NextBookingFromSubType.Should().Be(2901);
        SubType.NextBookingToSubType.Should().Be(2999);
        SubType.NextBookingRepresentativeSubType.Should().Be(2900);
        SubType.NextGoalFromSubType.Should().Be(401);
        SubType.NextGoalToSubType.Should().Be(499);
        SubType.NextGoalRepresentativeSubType.Should().Be(400);
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
