using OWP.Mics;
using Xunit;

namespace OWP.InfositeIdSync.UnitTests.Mics;

public class RepresentativeSubTypeDetectorUnitTests
{
    [Theory]
    [InlineData(1870, 1800)]
    [InlineData(2760, 2700)]
    [InlineData(3555, 3500)]
    [InlineData(3400, 3400)]
    public void Specific15MinsDetector(int value, int expected)
    {
        var actual = new Specific15MinsDetector().GetRepresentativeSubtype(value);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(2950, 2900)]
    [InlineData(2150, 2100)]
    [InlineData(3555, 3555)]
    [InlineData(402, 400)]
    public void NextTypesDetector(int value, int expected)
    {
        var actual = new NextTypesDetector().GetRepresentativeSubtype(value);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(1105, 1100)]
    [InlineData(1170, 1150)]
    [InlineData(2000, 2000)]
    public void SingleTeamPenaltyShootOut(int value, int expected)
    {
        var actual = new SingleTeamPenaltyShootOut().GetRepresentativeSubtype(value);

        Assert.Equal(expected, actual);
    }
}