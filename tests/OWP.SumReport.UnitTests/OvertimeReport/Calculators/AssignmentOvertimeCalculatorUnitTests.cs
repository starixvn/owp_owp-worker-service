namespace OWP.SumReport.UnitTests.OvertimeReport.Calculators;

using SumReport.OvertimeReport.Calculators;
using SumReport.OvertimeReport.Constants;
using SumReport.OvertimeReport.Models;
using Xunit;
using Xunit.Asserts.Compare;

public class AssignmentOvertimeCalculatorUnitTests
{
    [Fact]
    public void GetLatestAssignmentsInShift_WithInShiftAssignments_ReturnLatestAssignments()
    {
        var mockAssignments = new List<Assignment>
            {
                new()
                {
                    UserId = 1,
                    OwpMatchId = 1,
                    ParentMatchId = null,
                    MinorBettypeId = null,
                    Bettype = 1,
                    EventDateTime = new DateTime(2021, 10, 27, 2, 0, 0)
                },
                new()
                {
                    UserId = 1,
                    OwpMatchId = 1,
                    ParentMatchId = null,
                    MinorBettypeId = null,
                    Bettype = 3,
                    EventDateTime = new DateTime(2021, 10, 27, 2, 0, 0)
                },
                new()
                {
                    UserId = 1,
                    OwpMatchId = 2,
                    ParentMatchId = null,
                    MinorBettypeId = null,
                    Bettype = 1,
                    EventDateTime = new DateTime(2021, 10, 27, 13, 0, 0)
                },
                new()
                {
                    UserId = 1,
                    OwpMatchId = 1880087,
                    ParentMatchId = 2,
                    MinorBettypeId = -88,
                    Bettype = 1,
                    EventDateTime = new DateTime(2021, 10, 27, 13, 0, 0)
                }
            };
        var shiftStartTime = new DateTime(2021, 10, 27, 2, 0, 0);
        var shiftEndTime = new DateTime(2021, 10, 27, 13, 0, 0);

        var expected = mockAssignments.Where(x => x.EventDateTime.Equals(new DateTime(2021, 10, 27, 13, 0, 0)));
        var actual = AssignmentOvertimeCalculator.GetLatestAssignmentsInShift(mockAssignments, shiftStartTime, shiftEndTime);

        DeepAssert.Equal(expected, actual);
    }

    [Fact]
    public void GetLatestAssignmentsInShift_WithOutShiftAssignments_ReturnLatest()
    {
        var mockAssignments = new List<Assignment>
            {
                new()
                {
                    UserId = 1,
                    OwpMatchId = 1,
                    ParentMatchId = null,
                    MinorBettypeId = null,
                    Bettype = 1,
                    EventDateTime = new DateTime(2021, 10, 27, 2, 0, 0)
                },
                new()
                {
                    UserId = 1,
                    OwpMatchId = 1,
                    ParentMatchId = null,
                    MinorBettypeId = null,
                    Bettype = 3,
                    EventDateTime = new DateTime(2021, 10, 27, 2, 0, 0)
                },
                new()
                {
                    UserId = 1,
                    OwpMatchId = 2,
                    ParentMatchId = null,
                    MinorBettypeId = null,
                    Bettype = 1,
                    EventDateTime = new DateTime(2021, 10, 27, 13, 0, 0)
                },
                new()
                {
                    UserId = 1,
                    OwpMatchId = 1880087,
                    ParentMatchId = 2,
                    MinorBettypeId = -88,
                    Bettype = 1,
                    EventDateTime = new DateTime(2021, 10, 27, 13, 0, 0)
                }
            };
        var shiftStartTime = new DateTime(2021, 10, 27, 3, 0, 0);
        var shiftEndTime = new DateTime(2021, 10, 27, 11, 0, 0);

        IEnumerable<Assignment> expected = new List<Assignment>();
        var actual = AssignmentOvertimeCalculator.GetLatestAssignmentsInShift(mockAssignments, shiftStartTime, shiftEndTime);

        DeepAssert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("SABA CLUB FRIENDLY Virtual PES 21 - 15 Mins Play", BetType.FullTimeBetType.Handicap, " ET/PK", 45)]
    [InlineData("SABA INTERNATIONAL FRIENDLY Virtual PES 21 - 20 Mins Play", BetType.FullTimeBetType.Ft1X2, "PK only", 45)]
    [InlineData("SABA CLUB FRIENDLY Virtual FIFA 21 - 16 Mins Play", BetType.FullTimeBetType.Ft1X2, "", 45)]
    [InlineData("SABA CLUB FRIENDLY Virtual FIFA 20 FUTSAL - 6 Mins Play", BetType.FullTimeBetType.Ft1X2, " ET/PK", 60)]
    [InlineData("M88 SABA SPANISH LEAGUE Virtual FIFA 21 Season 8 - 16 Mins Play", BetType.FullTimeBetType.Ft1X2, "PK", 60)]
    [InlineData("SABA ELITE FRIENDLY Virtual PES 21 - 5 Mins Play", BetType.FullTimeBetType.Handicap, "PK", 60)]
    [InlineData("SABA ELITE FRIENDLY Virtual PES 21 - 5 Mins Play", BetType.FullTimeBetType.Handicap, " ET/PK", 60)]
    [InlineData("SABA ELITE FRIENDLY Virtual PES 21 - 5 Mins Play - FAST MARKET", BetType.FullTimeBetType.Handicap, " ET/PK", 60)]
    public void CalculateMatchEndTime_SABAMatchAssignment_ReturnSABAMatchDuration(
        string leagueName,
        int betType,
        string remark,
        int expectedDuration)
    {
        var mockAssignment = new Assignment
        {
            Bettype = betType,
            OwpLeagueName = leagueName,
            EventDateTime = new DateTime(2021, 11, 04, 12, 0, 0),
            Remark = remark
        };
        var expected = mockAssignment.EventDateTime.AddMinutes(expectedDuration);

        var actual = AssignmentOvertimeCalculator.CalculateMatchEndTime(mockAssignment);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("INDONESIA LIGA 1", BetType.FirstHalfBettype.FirstHalfHandicap, " ET/PK", 60)]
    [InlineData("INDONESIA LIGA 1", BetType.FirstHalfBettype.FirstHalf1X2, " ET/PK", 60)]
    [InlineData("INDONESIA LIGA 1", BetType.FullTimeBetType.Handicap, " ET/PK", 150)]
    [InlineData("INDONESIA LIGA 1", BetType.FullTimeBetType.Handicap, " ET", 150)]
    [InlineData("INDONESIA LIGA 1", BetType.FullTimeBetType.Handicap, "PK", 135)]
    [InlineData("INDONESIA LIGA 1", BetType.FullTimeBetType.Handicap, "remark", 120)]
    public void CalculateMatchEndTime_NormalMatchAssignment_ReturnNormalMatchDuration(
        string leagueName,
        int betType,
        string remark,
        int expectedDuration)
    {
        var mockAssignment = new Assignment
        {
            Bettype = betType,
            OwpLeagueName = leagueName,
            EventDateTime = new DateTime(2021, 11, 04, 12, 0, 0),
            Remark = remark
        };
        var expected = mockAssignment.EventDateTime.AddMinutes(expectedDuration);

        var actual = AssignmentOvertimeCalculator.CalculateMatchEndTime(mockAssignment);

        Assert.Equal(expected, actual);
    }
}