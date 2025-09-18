using FluentAssertions;
using OWP.Constants;
using OWP.Mics;
using Xunit;

namespace OWP.UnitTests.Mics;

public class WinnerAndNextRoundTests
{
    private readonly WinnerAndNextRound _detector;

    public WinnerAndNextRoundTests()
    {
        _detector = new WinnerAndNextRound();
    }

    [Theory]
    [InlineData(601, 600)] // Lower bound - winner
    [InlineData(650, 600)] // Middle - winner
    [InlineData(699, 600)] // Upper bound - winner
    [InlineData(801, 800)] // Lower bound - next round
    [InlineData(850, 800)] // Middle - next round
    [InlineData(899, 800)] // Upper bound - next round
    public void GetRepresentativeSubtype_WhenInRange_ShouldReturnRepresentativeSubType(int subType, int expected)
    {
        // Act
        var result = _detector.GetRepresentativeSubtype(subType);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(600)] // Below winner range
    [InlineData(700)] // Above winner range
    [InlineData(800)] // Below next round range
    [InlineData(900)] // Above next round range
    [InlineData(1000)] // Far below
    [InlineData(2000)] // Far above
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
        SubType.WinnerFromSubType.Should().Be(601);
        SubType.WinnerToSubType.Should().Be(699);
        SubType.WinnerRepresentativeSubType.Should().Be(600);
        SubType.NextRoundFromSubType.Should().Be(801);
        SubType.NextRoundToSubType.Should().Be(899);
        SubType.NextRoundRepresentativeSubType.Should().Be(800);
    }

    [Fact]
    public void GetRepresentativeSubtype_ShouldHaveGapBetweenWinnerAndNextRoundRanges()
    {
        // Act & Assert
        SubType.NextRoundFromSubType.Should().Be(801);
        SubType.WinnerToSubType.Should().Be(699);

        // There should be a gap between winner and next round ranges
        (SubType.NextRoundFromSubType - SubType.WinnerToSubType)
            .Should().Be(102);
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
