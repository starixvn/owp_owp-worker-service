namespace OWP.SumReport.UnitTests.OvertimeReport.Calculators;

using SumReport.OvertimeReport.Calculators;
using SumReport.OvertimeReport.Constants;
using SumReport.OvertimeReport.Models;
using Xunit;

public class AssignmentOvertimeCalculatorSecondUnitTests
{
    [Fact]
    public void CalculateOvertimeData_ReturnTotalOvertime()
    {
        var assignments = MockFtAssignmentsHavingEtpk();
        var workingShift = new EmployeeWorkingShift
        {
            ShiftStartTime = new DateTime(2021, 10, 27, 2, 0, 0),
            ShiftEndTime = new DateTime(2021, 10, 27, 13, 0, 0)
        };
        var mainMatchRemarks = new List<MainMatch>
        {
                new()
                {
                    OwpMatchId = 2,
                    Remark = " ET/PK"
                }
            };

        var actual = AssignmentOvertimeCalculator.CalculateOvertimeData(workingShift, assignments,  mainMatchRemarks);

        Assert.Equal(2.5m, actual);
    }

    [Fact]
    public void GetLatestMatchEndTime_WithFTAssignments_ReturnMaxFTDuration()
    {
        var assignments = MockFtAssignmentsHavingEtpk();
        var workingShift = new EmployeeWorkingShift
        {
            ShiftStartTime = new DateTime(2021, 10, 27, 2, 0, 0),
            ShiftEndTime = new DateTime(2021, 10, 27, 13, 0, 0)
        };
        var mainMatchRemarks = new List<MainMatch>
        {
                new()
                {
                    OwpMatchId = 2,
                    Remark = " ET/PK"
                }
            };
        var expected = workingShift.ShiftEndTime.AddMinutes(150);

        var actual = AssignmentOvertimeCalculator.GetLatestMatchEndTime(
            workingShift,
            assignments,
            mainMatchRemarks);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetLatestMatchEndTime_With1HAssignments_ReturnMax1HDuration()
    {
        var assignments = Mock1HAssignments();
        var workingShift = new EmployeeWorkingShift
        {
            ShiftStartTime = new DateTime(2021, 10, 27, 2, 0, 0),
            ShiftEndTime = new DateTime(2021, 10, 27, 13, 0, 0)
        };
        var mainMatchRemarks = new List<MainMatch>
        {
                new()
                {
                    OwpMatchId = 2,
                    Remark = "ET/PK",
                    EventDateTime = new DateTime(2021, 10, 27, 13, 0, 0)
                }
            };
        var expected = workingShift.ShiftEndTime.AddMinutes(60);

        var actual = AssignmentOvertimeCalculator.GetLatestMatchEndTime(
            workingShift,
            assignments,
            mainMatchRemarks);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetLatestMatchEndTime_WithCornerAssignments_ReturnMaxCornerDuration()
    {
        var assignments = MockCornerAssignmentsWithEtpkMainMatch();
        var workingShift = new EmployeeWorkingShift
        {
            ShiftStartTime = new DateTime(2021, 10, 27, 2, 0, 0),
            ShiftEndTime = new DateTime(2021, 10, 27, 13, 0, 0)
        };
        var mainMatchRemarks = new List<MainMatch>
        {
                new()
                {
                    OwpMatchId = 2,
                    Remark = " ET/PK",
                    EventDateTime = new DateTime(2021, 10, 27, 13, 0, 0)
                }
            };
        var expected = workingShift.ShiftEndTime.AddMinutes(150);

        var actual = AssignmentOvertimeCalculator.GetLatestMatchEndTime(
            workingShift,
            assignments,
            mainMatchRemarks);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetLatestMatchEndTime_WithNoShiftRangeAssignments_ReturnShiftEndTime()
    {
        var assignments = MockOutShiftAssignments();
        var workingShift = new EmployeeWorkingShift
        {
            ShiftStartTime = new DateTime(2021, 10, 27, 2, 0, 0),
            ShiftEndTime = new DateTime(2021, 10, 27, 13, 0, 0)
        };
        var mainMatchRemarks = new List<MainMatch>
        {
                new()
                {
                    OwpMatchId = 2,
                    Remark = "ET/PK"
                }
            };

        var actual = AssignmentOvertimeCalculator.GetLatestMatchEndTime(
            workingShift,
            assignments,
            mainMatchRemarks);

        Assert.Equal(workingShift.ShiftEndTime, actual);
    }

    private static IList<Assignment> MockFtAssignmentsHavingEtpk()
        => new List<Assignment>
        {
                new()
                {
                    UserId = 1,
                    OwpMatchId = 1,
                    ParentMatchId = null,
                    MinorBettypeId = null,
                    Bettype = BetType.FullTimeBetType.Handicap,
                    EventDateTime = new DateTime(2021, 10, 27, 2, 0, 0),
                    OwpLeagueName = "SABA",
                    Remark = " ET/PK"
                },
                new()
                {
                    UserId = 1,
                    OwpMatchId = 2,
                    ParentMatchId = null,
                    MinorBettypeId = null,
                    Bettype = BetType.FullTimeBetType.Handicap,
                    EventDateTime = new DateTime(2021, 10, 27, 13, 0, 0),
                    OwpLeagueName = "CLeague",
                    Remark = " ET/PK"
                },
                new()
                {
                    UserId = 1,
                    OwpMatchId = 169368,
                    ParentMatchId = 2,
                    MinorBettypeId = -4,
                    Bettype = BetType.FirstHalfBettype.FirstHalfHandicap,
                    EventDateTime = new DateTime(2021, 10, 27, 13, 0, 0),
                    OwpLeagueName = "CLeague",
                    Remark = " ET/PK"
                }
        };

    private static IList<Assignment> Mock1HAssignments()
        => new List<Assignment>
        {
                new()
                {
                    UserId = 1,
                    OwpMatchId = 1,
                    ParentMatchId = null,
                    MinorBettypeId = null,
                    Bettype = BetType.FullTimeBetType.Handicap,
                    EventDateTime = new DateTime(2021, 10, 27, 2, 0, 0),
                    OwpLeagueName = "SABA",
                    Remark = "ET/PK"
                },
                new()
                {
                    UserId = 1,
                    OwpMatchId = 169368,
                    ParentMatchId = 2,
                    MinorBettypeId = -4,
                    Bettype = BetType.FirstHalfBettype.FirstHalfHandicap,
                    EventDateTime = new DateTime(2021, 10, 27, 13, 0, 0),
                    OwpLeagueName = "CLeague",
                    Remark = ""
                }
        };

    private static IList<Assignment> MockCornerAssignmentsWithEtpkMainMatch()
        => new List<Assignment>
        {
                new()
                {
                    UserId = 1,
                    OwpMatchId = 1,
                    ParentMatchId = null,
                    MinorBettypeId = null,
                    Bettype = BetType.FullTimeBetType.Handicap,
                    EventDateTime = new DateTime(2021, 10, 27, 2, 0, 0),
                    OwpLeagueName = "SABA",
                    Remark = "ET/PK"
                },
                new()
                {
                    UserId = 1,
                    OwpMatchId = 169368,
                    ParentMatchId = 2,
                    MinorBettypeId = BetType.CornerMinorBettype.Next,
                    Bettype = BetType.FullTimeBetType.Handicap,
                    EventDateTime = new DateTime(2021, 10, 27, 13, 0, 0),
                    OwpLeagueName = "CLeague",
                    Remark = ""
                }
        };

    private static IList<Assignment> MockOutShiftAssignments()
        => new List<Assignment>
        {
                new()
                {
                    UserId = 1,
                    OwpMatchId = 1,
                    ParentMatchId = null,
                    MinorBettypeId = null,
                    Bettype = BetType.FullTimeBetType.Handicap,
                    EventDateTime = new DateTime(2021, 10, 26, 2, 0, 0),
                    OwpLeagueName = "SABA",
                    Remark = " ET/PK"
                },
                new()
                {
                    UserId = 1,
                    OwpMatchId = 169368,
                    ParentMatchId = 2,
                    MinorBettypeId = BetType.CornerMinorBettype.Next,
                    Bettype = BetType.FullTimeBetType.Handicap,
                    EventDateTime = new DateTime(2021, 10, 29, 13, 0, 0),
                    OwpLeagueName = "CLeague",
                    Remark = ""
                }
        };
}