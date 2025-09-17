using FluentAssertions;
using OWP.Constants;
using OWP.Mics;
using Xunit;

namespace OWP.UnitTests.Integration;

public class DetectorChainIntegrationTests
{
    [Fact]
    public void DetectorChain_ShouldWorkCorrectly()
    {
        // Arrange - Create a chain of detectors
        var specific10Mins = new Specific10MinsDetector();
        var specific15Mins = new Specific15MinsDetector();
        var nextTypes = new NextTypesDetector();
        var singleTeamPenalty = new SingleTeamPenaltyShootOut();
        var winnerAndNextRound = new WinnerAndNextRound();

        // Chain them together
        specific10Mins.NextType = specific15Mins;
        specific15Mins.NextType = nextTypes;
        nextTypes.NextType = singleTeamPenalty;
        singleTeamPenalty.NextType = winnerAndNextRound;

        // Act & Assert - Test various sub-types
        specific10Mins.GetRepresentativeSubtype(1950).Should().Be(1900); // 10 mins
        specific10Mins.GetRepresentativeSubtype(1850).Should().Be(1800); // 15 mins goals
        specific10Mins.GetRepresentativeSubtype(2750).Should().Be(2700); // 15 mins corners
        specific10Mins.GetRepresentativeSubtype(3550).Should().Be(3500); // 15 mins bookings
        specific10Mins.GetRepresentativeSubtype(2150).Should().Be(2100); // Next corner
        specific10Mins.GetRepresentativeSubtype(2950).Should().Be(2900); // Next booking
        specific10Mins.GetRepresentativeSubtype(450).Should().Be(400); // Next goal
        specific10Mins.GetRepresentativeSubtype(1125).Should().Be(1100); // Home penalty
        specific10Mins.GetRepresentativeSubtype(1175).Should().Be(1150); // Away penalty
        specific10Mins.GetRepresentativeSubtype(650).Should().Be(600); // Winner
        specific10Mins.GetRepresentativeSubtype(850).Should().Be(800); // Next round
        specific10Mins.GetRepresentativeSubtype(9999).Should().Be(9999); // No match
    }

    [Fact]
    public void DetectorChain_WithPartialChain_ShouldWorkCorrectly()
    {
        // Arrange - Create a partial chain
        var specific10Mins = new Specific10MinsDetector();
        var nextTypes = new NextTypesDetector();

        specific10Mins.NextType = nextTypes;

        // Act & Assert
        specific10Mins.GetRepresentativeSubtype(1950).Should().Be(1900); // 10 mins (handled by first)
        specific10Mins.GetRepresentativeSubtype(2150).Should().Be(2100); // Next corner (handled by second)
        specific10Mins.GetRepresentativeSubtype(9999).Should().Be(9999); // No match (delegated to end)
    }

    [Fact]
    public void DetectorChain_WithEmptyChain_ShouldReturnOriginalSubType()
    {
        // Arrange
        var detector = new RepresentativeSubTypeDetector();

        // Act & Assert
        detector.GetRepresentativeSubtype(1234).Should().Be(1234);
    }

    [Fact]
    public void DetectorChain_ShouldHandleAllSubTypeRanges()
    {
        // Arrange
        var detector = CreateFullDetectorChain();

        // Act & Assert - Test boundary values
        // 10 mins range
        detector.GetRepresentativeSubtype(1900).Should().Be(1900);
        detector.GetRepresentativeSubtype(1980).Should().Be(1900);

        // 15 mins goals range
        detector.GetRepresentativeSubtype(1800).Should().Be(1800);
        detector.GetRepresentativeSubtype(1875).Should().Be(1800);

        // 15 mins corners range
        detector.GetRepresentativeSubtype(2700).Should().Be(2700);
        detector.GetRepresentativeSubtype(2775).Should().Be(2700);

        // 15 mins bookings range
        detector.GetRepresentativeSubtype(3500).Should().Be(3500);
        detector.GetRepresentativeSubtype(3575).Should().Be(3500);

        // Next corner range
        detector.GetRepresentativeSubtype(2101).Should().Be(2100);
        detector.GetRepresentativeSubtype(2199).Should().Be(2100);

        // Next booking range
        detector.GetRepresentativeSubtype(2901).Should().Be(2900);
        detector.GetRepresentativeSubtype(2999).Should().Be(2900);

        // Next goal range
        detector.GetRepresentativeSubtype(401).Should().Be(400);
        detector.GetRepresentativeSubtype(499).Should().Be(400);

        // Home penalty range
        detector.GetRepresentativeSubtype(1101).Should().Be(1100);
        detector.GetRepresentativeSubtype(1149).Should().Be(1100);

        // Away penalty range
        detector.GetRepresentativeSubtype(1151).Should().Be(1150);
        detector.GetRepresentativeSubtype(1199).Should().Be(1150);

        // Winner range
        detector.GetRepresentativeSubtype(601).Should().Be(600);
        detector.GetRepresentativeSubtype(699).Should().Be(600);

        // Next round range
        detector.GetRepresentativeSubtype(801).Should().Be(800);
        detector.GetRepresentativeSubtype(899).Should().Be(800);
    }

    private static RepresentativeSubTypeDetector CreateFullDetectorChain()
    {
        var specific10Mins = new Specific10MinsDetector();
        var specific15Mins = new Specific15MinsDetector();
        var nextTypes = new NextTypesDetector();
        var singleTeamPenalty = new SingleTeamPenaltyShootOut();
        var winnerAndNextRound = new WinnerAndNextRound();

        specific10Mins.NextType = specific15Mins;
        specific15Mins.NextType = nextTypes;
        nextTypes.NextType = singleTeamPenalty;
        singleTeamPenalty.NextType = winnerAndNextRound;

        return specific10Mins;
    }
}
