using FluentAssertions;
using OWP.Constants;
using OWP.Mics;
using Xunit;

namespace OWP.UnitTests.Mics;

public class SingleTeamPenaltyShootOutTests
{
    private readonly SingleTeamPenaltyShootOut _detector;

    public SingleTeamPenaltyShootOutTests()
    {
        _detector = new SingleTeamPenaltyShootOut();
    }

    [Theory]
    [InlineData(1101, 1100)] // Lower bound - home
    [InlineData(1125, 1100)] // Middle - home
    [InlineData(1149, 1100)] // Upper bound - home
    [InlineData(1151, 1150)] // Lower bound - away
    [InlineData(1175, 1150)] // Middle - away
    [InlineData(1199, 1150)] // Upper bound - away
    public void GetRepresentativeSubtype_WhenInRange_ShouldReturnRepresentativeSubType(int subType, int expected)
    {
        // Act
        var result = _detector.GetRepresentativeSubtype(subType);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(1100)] // Below home range
    [InlineData(1150)] // Between ranges (gap)
    [InlineData(1200)] // Above away range
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
        SubType.HomeSingleTeamPenaltyShootOutFromSubType.Should().Be(1101);
        SubType.HomeSingleTeamPenaltyShootOutToSubType.Should().Be(1149);
        SubType.HomeSingleTeamPenaltyShootOutRepresentativeSubType.Should().Be(1100);
        SubType.AwaySingleTeamPenaltyShootOutFromSubType.Should().Be(1151);
        SubType.AwaySingleTeamPenaltyShootOutToSubType.Should().Be(1199);
        SubType.AwaySingleTeamPenaltyShootOutRepresentativeSubType.Should().Be(1150);
    }

    [Fact]
    public void GetRepresentativeSubtype_ShouldHaveGapBetweenHomeAndAwayRanges()
    {
        // Act & Assert
        SubType.AwaySingleTeamPenaltyShootOutFromSubType.Should().Be(1151);
        SubType.HomeSingleTeamPenaltyShootOutToSubType.Should().Be(1149);

        // There should be a gap of 1 between home and away ranges
        (SubType.AwaySingleTeamPenaltyShootOutFromSubType - SubType.HomeSingleTeamPenaltyShootOutToSubType)
            .Should().Be(2);
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
