using OWP.SumReportData.OvertimeReport.Helpers;
using OWP.SumReportData.OvertimeReport.Models;
using Xunit;
using static OWP.SumReportData.OvertimeReport.Constants.BetType;

namespace OWP.SumReportData.UnitTests.OvertimeReport;

public class MatchHelperUnitTests
{
    [Theory]
    [InlineData(FirstHalfBettype.FirstHalfHandicap, "", 60)]
    [InlineData(FirstHalfBettype.FirstHalfOverUnder, "", 60)]
    [InlineData(FullTimeBetType.Handicap, "", 120)]
    [InlineData(FullTimeBetType.Handicap, " ETPK", 150)]
    [InlineData(FullTimeBetType.FT1X2, " ET", 150)]
    [InlineData(FullTimeBetType.OverUnder, "PK", 135)]
    public void CalculateNormalMatchDuration_WithFTBetTypes_ReturnCorrectValue(int bettype, string remark, int expected)
    {
        var assignment = new Assignment
        {
            Bettype = bettype,
            Remark = remark
        };
        var actual = MatchHelper.CalculateNormalMatchDuration(assignment);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(FullTimeBetType.FM, "", 120)]
    [InlineData(FullTimeBetType.AIO, " ET/PK", 150)]
    [InlineData(-8, " ET/PK", 0)]
    public void CalculateNormalMatchDuration_WithFTMinorBetTypes_ReturnCorrectValue(int bettype, string remark, int expected)
    {
        var assignment = new Assignment
        {
            Bettype = 1,
            MinorBettypeId = bettype,
            Remark = remark
        };
        var actual = MatchHelper.CalculateNormalMatchDuration(assignment);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("SABA CLUB FRIENDLY Virtual PES 21 - 15 Mins Play", "", 30)]
    [InlineData("SABA CLUB FRIENDLY Virtual PES 21 - 15 Mins Play", " ETPK", 45)]
    [InlineData("SABA INTERNATIONAL FRIENDLY Virtual PES 21 - 20 Mins Play", "PK", 45)]
    [InlineData("SABA INTERNATIONAL FRIENDLY Virtual PES 21 - 20 Mins Play", " ET", 30)]
    public void CalculateSABAPESMatchDuration_ReturnCorrectValue(string leagueName, string remark, int expected)
    {
        var assignment = new Assignment { OWPLeagueName = leagueName, Remark = remark };
        var actual = MatchHelper.CalculateSABAPESMatchDuration(assignment);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("", 60)]
    [InlineData(" ETPK", 60)]
    [InlineData("PK", 60)]
    [InlineData(" ET", 60)]
    public void CalculateSABAPES5MinsMatchDuration_ReturnCorrectValue(string remark, int expected)
    {
        var assignment = new Assignment { Remark = remark };
        var actual = MatchHelper.CalculateSABAPES5MinsMatchDuration(assignment);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("", 45)]
    [InlineData("ETPK", 60)]
    [InlineData("PK", 60)]
    [InlineData("ET", 45)]
    public void CalculateSABAFIFAMatchDuration_ReturnCorrectValue(string remark, int expected)
    {
        var assignment = new Assignment { Remark = remark };
        var actual = MatchHelper.CalculateSABAFIFAMatchDuration(assignment);

        Assert.Equal(expected, actual);
    }
}