namespace OWP.SumReport.OvertimeReport.Models;

public class EmployeeWorkingShift
{
    public int EmployeeId { get; set; }

    public DateTime ShiftDate { get; set; }

    public DateTime ShiftStartTime { get; set; }

    public DateTime ShiftEndTime { get; set; }

    public int? RefShiftId { get; set; }
}