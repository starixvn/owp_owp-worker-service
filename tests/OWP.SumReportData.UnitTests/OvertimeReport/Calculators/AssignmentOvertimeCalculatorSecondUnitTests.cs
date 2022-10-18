using OWP.SumReportData.OvertimeReport.Calculators;
using OWP.SumReportData.OvertimeReport.Constants;
using OWP.SumReportData.OvertimeReport.Models;
using Xunit;

namespace OWP.SumReportData.UnitTests.OvertimeReport.Calculators;

public class AssignmentOvertimeCalculatorSecondUnitTests
{
    [Fact]
    public void CalculateOvertimeData_ReturnTotalOvertime()
    {
        var assignments = MockFTAssignmentsHavingETPK();
        var workingShift = new EmployeeWorkingShift
        {
            ShiftStartTime = new DateTime(2021, 10, 27, 2, 0, 0),
            ShiftEndTime = new DateTime(2021, 10, 27, 13, 0, 0)
        };
        var mainMatchRemarks = new List<MainMatch>()
            {
                new MainMatch
                {
                    OWPMatchId = 2,
                    Remark = " ET/PK"
                }
            };

        var actual = AssignmentOvertimeCalculator.CalculateOvertimeData(assignments, workingShift, mainMatchRemarks);

        Assert.Equal(2.5m, actual);
    }

    [Fact]
    public void GetLatestMatchEndTime_WithFTAssignments_ReturnMaxFTDuration()
    {
        var assignments = MockFTAssignmentsHavingETPK();
        var workingShift = new EmployeeWorkingShift
        {
            ShiftStartTime = new DateTime(2021, 10, 27, 2, 0, 0),
            ShiftEndTime = new DateTime(2021, 10, 27, 13, 0, 0)
        };
        var mainMatchRemarks = new List<MainMatch>()
            {
                new MainMatch
                {
                    OWPMatchId = 2,
                    Remark = " ET/PK"
                }
            };
        var expected = workingShift.ShiftEndTime.AddMinutes(150);

        var actual = AssignmentOvertimeCalculator.GetLatestMatchEndTime(
            assignments,
            workingShift,
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
        var mainMatchRemarks = new List<MainMatch>()
            {
                new MainMatch
                {
                    OWPMatchId = 2,
                    Remark = "ET/PK",
                    EventDateTime = new DateTime(2021, 10, 27, 13, 0, 0)
                }
            };
        var expected = workingShift.ShiftEndTime.AddMinutes(60);

        var actual = AssignmentOvertimeCalculator.GetLatestMatchEndTime(
            assignments,
            workingShift,
            mainMatchRemarks);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetLatestMatchEndTime_WithCornerAssignments_ReturnMaxCornerDuration()
    {
        var assignments = MockCornerAssignmentsWithETPKMainMatch();
        var workingShift = new EmployeeWorkingShift
        {
            ShiftStartTime = new DateTime(2021, 10, 27, 2, 0, 0),
            ShiftEndTime = new DateTime(2021, 10, 27, 13, 0, 0)
        };
        var mainMatchRemarks = new List<MainMatch>()
            {
                new MainMatch
                {
                    OWPMatchId = 2,
                    Remark = " ET/PK",
                    EventDateTime = new DateTime(2021, 10, 27, 13, 0, 0)
                }
            };
        var expected = workingShift.ShiftEndTime.AddMinutes(150);

        var actual = AssignmentOvertimeCalculator.GetLatestMatchEndTime(
            assignments,
            workingShift,
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
        var mainMatchRemarks = new List<MainMatch>()
            {
                new MainMatch
                {
                    OWPMatchId = 2,
                    Remark = "ET/PK"
                }
            };

        var actual = AssignmentOvertimeCalculator.GetLatestMatchEndTime(
            assignments,
            workingShift,
            mainMatchRemarks);

        Assert.Equal(workingShift.ShiftEndTime, actual);
    }

    private IEnumerable<Assignment> MockFTAssignmentsHavingETPK()
        => new List<Assignment>
        {
                new Assignment
                {
                    UserId = 1,
                    OWPMatchId = 1,
                    ParentMatchId = null,
                    MinorBettypeId = null,
                    Bettype = BetType.FullTimeBetType.Handicap,
                    EventDateTime = new DateTime(2021, 10, 27, 2, 0, 0),
                    OWPLeagueName = "SABA",
                    Remark = " ET/PK"
                },
                new Assignment
                {
                    UserId = 1,
                    OWPMatchId = 2,
                    ParentMatchId = null,
                    MinorBettypeId = null,
                    Bettype = BetType.FullTimeBetType.Handicap,
                    EventDateTime = new DateTime(2021, 10, 27, 13, 0, 0),
                    OWPLeagueName = "CLeague",
                    Remark = " ET/PK"
                },
                new Assignment
                {
                    UserId = 1,
                    OWPMatchId = 169368,
                    ParentMatchId = 2,
                    MinorBettypeId = -4,
                    Bettype = BetType.FirstHalfBettype.FirstHalfHandicap,
                    EventDateTime = new DateTime(2021, 10, 27, 13, 0, 0),
                    OWPLeagueName = "CLeague",
                    Remark = " ET/PK"
                }
        };

    private IEnumerable<Assignment> Mock1HAssignments()
        => new List<Assignment>
        {
                new Assignment
                {
                    UserId = 1,
                    OWPMatchId = 1,
                    ParentMatchId = null,
                    MinorBettypeId = null,
                    Bettype = BetType.FullTimeBetType.Handicap,
                    EventDateTime = new DateTime(2021, 10, 27, 2, 0, 0),
                    OWPLeagueName = "SABA",
                    Remark = "ET/PK"
                },
                new Assignment
                {
                    UserId = 1,
                    OWPMatchId = 169368,
                    ParentMatchId = 2,
                    MinorBettypeId = -4,
                    Bettype = BetType.FirstHalfBettype.FirstHalfHandicap,
                    EventDateTime = new DateTime(2021, 10, 27, 13, 0, 0),
                    OWPLeagueName = "CLeague",
                    Remark = ""
                }
        };

    private IEnumerable<Assignment> MockCornerAssignmentsWithETPKMainMatch()
        => new List<Assignment>
        {
                new Assignment
                {
                    UserId = 1,
                    OWPMatchId = 1,
                    ParentMatchId = null,
                    MinorBettypeId = null,
                    Bettype = BetType.FullTimeBetType.Handicap,
                    EventDateTime = new DateTime(2021, 10, 27, 2, 0, 0),
                    OWPLeagueName = "SABA",
                    Remark = "ET/PK"
                },
                new Assignment
                {
                    UserId = 1,
                    OWPMatchId = 169368,
                    ParentMatchId = 2,
                    MinorBettypeId = BetType.CornerMinorBettype.Next,
                    Bettype = BetType.FullTimeBetType.Handicap,
                    EventDateTime = new DateTime(2021, 10, 27, 13, 0, 0),
                    OWPLeagueName = "CLeague",
                    Remark = ""
                }
        };

    private IEnumerable<Assignment> MockOutShiftAssignments()
        => new List<Assignment>
        {
                new Assignment
                {
                    UserId = 1,
                    OWPMatchId = 1,
                    ParentMatchId = null,
                    MinorBettypeId = null,
                    Bettype = BetType.FullTimeBetType.Handicap,
                    EventDateTime = new DateTime(2021, 10, 26, 2, 0, 0),
                    OWPLeagueName = "SABA",
                    Remark = " ET/PK"
                },
                new Assignment
                {
                    UserId = 1,
                    OWPMatchId = 169368,
                    ParentMatchId = 2,
                    MinorBettypeId = BetType.CornerMinorBettype.Next,
                    Bettype = BetType.FullTimeBetType.Handicap,
                    EventDateTime = new DateTime(2021, 10, 29, 13, 0, 0),
                    OWPLeagueName = "CLeague",
                    Remark = ""
                }
        };
}