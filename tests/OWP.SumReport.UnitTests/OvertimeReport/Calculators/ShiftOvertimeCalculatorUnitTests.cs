namespace OWP.SumReport.UnitTests.OvertimeReport.Calculators;

using SumReport.OvertimeReport.Calculators;
using SumReport.OvertimeReport.Models;
using Xunit;

public class ShiftOvertimeCalculatorUnitTests
{
    [Theory]
    [InlineData(16, 15, 2)]
    [InlineData(16, 45, 2.5)]
    [InlineData(16, 0, 2)]
    [InlineData(16, 12, 2.2)]
    public void CalculateOffShiftOvertimeData_ReturnCorrectTime(int endHour, int endMin, decimal expected)
    {
        var workingShift = new EmployeeWorkingShift
        {
            ShiftStartTime = new DateTime(2021, 11, 2, 14, 0, 0),
            ShiftEndTime = new DateTime(2021, 11, 2, endHour, endMin, 0)
        };
        var actual = ShiftOvertimeCalculator.CalculateOffShiftOvertimeData(workingShift);

        Assert.Equal(actual, expected);
    }

    [Theory]
    [InlineData(15, 0, 0)]
    [InlineData(20, 45, 1.5)]
    [InlineData(20, 0, 1)]
    public void CalculateTimeShiftOvertimeData_ReturnCorrectTime(int endHour, int endMin, decimal expected)
    {
        var workingShift = new EmployeeWorkingShift
        {
            ShiftStartTime = new DateTime(2021, 11, 2, 10, 0, 0),
            ShiftEndTime = new DateTime(2021, 11, 2, endHour, endMin, 0)
        };
        var actual = ShiftOvertimeCalculator.CalculateTimeShiftOvertimeData(workingShift);

        Assert.Equal(actual, expected);
    }
}