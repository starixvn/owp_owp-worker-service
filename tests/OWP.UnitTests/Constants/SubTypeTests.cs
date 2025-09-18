using FluentAssertions;
using OWP.Constants;
using Xunit;

namespace OWP.UnitTests.Constants;

public class SubTypeTests
{
    [Fact]
    public void Specific10Mins_ShouldHaveCorrectRange()
    {
        // Act & Assert
        SubType.Specific10MinsFromSubType.Should().Be(1900);
        SubType.Specific10MinsToSubType.Should().Be(1980);
        SubType.Specific10MinsToSubType.Should().BeGreaterThan(SubType.Specific10MinsFromSubType);
    }

    [Fact]
    public void Specific15Mins_ShouldHaveCorrectRange()
    {
        // Act & Assert
        SubType.Specific15MinsFromSubType.Should().Be(1800);
        SubType.Specific15MinsToSubType.Should().Be(1875);
        SubType.Specific15MinsToSubType.Should().BeGreaterThan(SubType.Specific15MinsFromSubType);
    }

    [Fact]
    public void Specific15MinsCorners_ShouldHaveCorrectRange()
    {
        // Act & Assert
        SubType.Specific15MinsCornersFromSubType.Should().Be(2700);
        SubType.Specific15MinsCornersToSubType.Should().Be(2775);
        SubType.Specific15MinsCornersToSubType.Should().BeGreaterThan(SubType.Specific15MinsCornersFromSubType);
    }

    [Fact]
    public void Specific15MinsTotalBooking_ShouldHaveCorrectRange()
    {
        // Act & Assert
        SubType.Specific15MinsTotalBookingFromSubType.Should().Be(3500);
        SubType.Specific15MinsTotalBookingToSubType.Should().Be(3575);
        SubType.Specific15MinsTotalBookingToSubType.Should().BeGreaterThan(SubType.Specific15MinsTotalBookingFromSubType);
    }

    [Fact]
    public void NextCorner_ShouldHaveCorrectRange()
    {
        // Act & Assert
        SubType.NextCornerFromSubType.Should().Be(2101);
        SubType.NextCornerToSubType.Should().Be(2199);
        SubType.NextCornerRepresentativeSubType.Should().Be(2100);
        SubType.NextCornerToSubType.Should().BeGreaterThan(SubType.NextCornerFromSubType);
        SubType.NextCornerRepresentativeSubType.Should().BeLessThan(SubType.NextCornerFromSubType);
    }

    [Fact]
    public void NextBooking_ShouldHaveCorrectRange()
    {
        // Act & Assert
        SubType.NextBookingFromSubType.Should().Be(2901);
        SubType.NextBookingToSubType.Should().Be(2999);
        SubType.NextBookingRepresentativeSubType.Should().Be(2900);
        SubType.NextBookingToSubType.Should().BeGreaterThan(SubType.NextBookingFromSubType);
        SubType.NextBookingRepresentativeSubType.Should().BeLessThan(SubType.NextBookingFromSubType);
    }

    [Fact]
    public void HomeSingleTeamPenaltyShootOut_ShouldHaveCorrectRange()
    {
        // Act & Assert
        SubType.HomeSingleTeamPenaltyShootOutFromSubType.Should().Be(1101);
        SubType.HomeSingleTeamPenaltyShootOutToSubType.Should().Be(1149);
        SubType.HomeSingleTeamPenaltyShootOutRepresentativeSubType.Should().Be(1100);
        SubType.HomeSingleTeamPenaltyShootOutToSubType.Should().BeGreaterThan(SubType.HomeSingleTeamPenaltyShootOutFromSubType);
        SubType.HomeSingleTeamPenaltyShootOutRepresentativeSubType.Should().BeLessThan(SubType.HomeSingleTeamPenaltyShootOutFromSubType);
    }

    [Fact]
    public void AwaySingleTeamPenaltyShootOut_ShouldHaveCorrectRange()
    {
        // Act & Assert
        SubType.AwaySingleTeamPenaltyShootOutFromSubType.Should().Be(1151);
        SubType.AwaySingleTeamPenaltyShootOutToSubType.Should().Be(1199);
        SubType.AwaySingleTeamPenaltyShootOutRepresentativeSubType.Should().Be(1150);
        SubType.AwaySingleTeamPenaltyShootOutToSubType.Should().BeGreaterThan(SubType.AwaySingleTeamPenaltyShootOutFromSubType);
        SubType.AwaySingleTeamPenaltyShootOutRepresentativeSubType.Should().BeLessThan(SubType.AwaySingleTeamPenaltyShootOutFromSubType);
    }

    [Fact]
    public void NextGoal_ShouldHaveCorrectRange()
    {
        // Act & Assert
        SubType.NextGoalFromSubType.Should().Be(401);
        SubType.NextGoalToSubType.Should().Be(499);
        SubType.NextGoalRepresentativeSubType.Should().Be(400);
        SubType.NextGoalToSubType.Should().BeGreaterThan(SubType.NextGoalFromSubType);
        SubType.NextGoalRepresentativeSubType.Should().BeLessThan(SubType.NextGoalFromSubType);
    }

    [Fact]
    public void Winner_ShouldHaveCorrectRange()
    {
        // Act & Assert
        SubType.WinnerFromSubType.Should().Be(601);
        SubType.WinnerToSubType.Should().Be(699);
        SubType.WinnerRepresentativeSubType.Should().Be(600);
        SubType.WinnerToSubType.Should().BeGreaterThan(SubType.WinnerFromSubType);
        SubType.WinnerRepresentativeSubType.Should().BeLessThan(SubType.WinnerFromSubType);
    }

    [Fact]
    public void NextRound_ShouldHaveCorrectRange()
    {
        // Act & Assert
        SubType.NextRoundFromSubType.Should().Be(801);
        SubType.NextRoundToSubType.Should().Be(899);
        SubType.NextRoundRepresentativeSubType.Should().Be(800);
        SubType.NextRoundToSubType.Should().BeGreaterThan(SubType.NextRoundFromSubType);
        SubType.NextRoundRepresentativeSubType.Should().BeLessThan(SubType.NextRoundFromSubType);
    }

    [Fact]
    public void AllRanges_ShouldNotOverlap()
    {
        // Arrange
        var ranges = new[]
        {
            (SubType.Specific10MinsFromSubType, SubType.Specific10MinsToSubType),
            (SubType.Specific15MinsFromSubType, SubType.Specific15MinsToSubType),
            (SubType.Specific15MinsCornersFromSubType, SubType.Specific15MinsCornersToSubType),
            (SubType.Specific15MinsTotalBookingFromSubType, SubType.Specific15MinsTotalBookingToSubType),
            (SubType.NextCornerFromSubType, SubType.NextCornerToSubType),
            (SubType.NextBookingFromSubType, SubType.NextBookingToSubType),
            (SubType.HomeSingleTeamPenaltyShootOutFromSubType, SubType.HomeSingleTeamPenaltyShootOutToSubType),
            (SubType.AwaySingleTeamPenaltyShootOutFromSubType, SubType.AwaySingleTeamPenaltyShootOutToSubType),
            (SubType.NextGoalFromSubType, SubType.NextGoalToSubType),
            (SubType.WinnerFromSubType, SubType.WinnerToSubType),
            (SubType.NextRoundFromSubType, SubType.NextRoundToSubType)
        };

        // Act & Assert
        for (int i = 0; i < ranges.Length; i++)
        {
            for (int j = i + 1; j < ranges.Length; j++)
            {
                var range1 = ranges[i];
                var range2 = ranges[j];

                // Check if ranges overlap
                var overlap = range1.Item1 <= range2.Item2 && range2.Item1 <= range1.Item2;
                overlap.Should().BeFalse($"Ranges {range1} and {range2} should not overlap");
            }
        }
    }
}
