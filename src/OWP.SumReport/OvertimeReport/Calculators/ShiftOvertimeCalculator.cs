using OWP.SumReport.OvertimeReport.Models;

namespace OWP.SumReport.OvertimeReport.Calculators;

public static class ShiftOvertimeCalculator
{
    private const int NormalWorkingTime = 9;

    private const decimal FifteenMinutesInHour = 0.25M;

    private const decimal HalfAnHour = 0.5M;

    public static decimal CalculateTimeShiftOvertimeData(EmployeeWorkingShift workingShift)
    {
        var actualWorkingTime = GetActualWorkingShift(workingShift.ShiftStartTime, workingShift.ShiftEndTime);

        if (actualWorkingTime <= NormalWorkingTime)
        {
            return 0;
        }

        return actualWorkingTime - NormalWorkingTime;
    }

    public static decimal CalculateOffShiftOvertimeData(EmployeeWorkingShift workingShift)
        => GetActualWorkingShift(workingShift.ShiftStartTime, workingShift.ShiftEndTime);

    private static decimal GetActualWorkingShift(DateTime shiftStartTime, DateTime shiftEndTime)
    {
        var workingTime = (decimal)(shiftEndTime - shiftStartTime).TotalHours;

        return workingTime % HalfAnHour == FifteenMinutesInHour ? workingTime - FifteenMinutesInHour : workingTime;
    }
}