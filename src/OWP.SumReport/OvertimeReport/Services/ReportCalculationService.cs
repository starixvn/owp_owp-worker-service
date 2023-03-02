namespace OWP.SumReport.OvertimeReport.Services;

using Calculators;
using Helpers;
using Models;

public interface ReportCalculationService
{
    Task SumOvertimeReport(DateTime date, bool isGenerateData = false);
}

public class ReportCalculationServiceImp : ReportCalculationService
{
    private readonly OvertimeRepository repository;

    public ReportCalculationServiceImp(OvertimeRepository repository)
    {
        this.repository = repository;
    }

    public async Task SumOvertimeReport(DateTime date, bool isGenerateData = false)
    {
        var reportData = await CalculateOvertimeReport(date, isGenerateData);

        await repository.SaveData(date, reportData);
    }

    public async Task<IEnumerable<OvertimeData>> CalculateOvertimeReport(DateTime date, bool isGenerateData)
    {
        var workingShifts = (await repository.GetEmployeeWorkingShift(date, isGenerateData)).ToList();

        if (!workingShifts.Any())
        {
            return new List<OvertimeData>();
        }

        var userIds = workingShifts.Select(x => x.EmployeeId);
        var tradersAssignments = await repository.GetTradersAssignments(date, userIds, isGenerateData);
        var (assignments, mainMatchRemarks) = (tradersAssignments.Item1.ToList(), tradersAssignments.Item2.ToList());
        var timeShifts = workingShifts.Where(x => !x.RefShiftId.HasValue).ToList();
        var offShifts = workingShifts.Where(x => x.RefShiftId.HasValue).ToList();
        var timeShiftUsers = timeShifts.Select(x => x.EmployeeId);
        var offShiftUsers = offShifts.Select(x => x.EmployeeId);
        var timeShiftAssignments = assignments.Where(x => timeShiftUsers.Contains(x.UserId)).ToList();
        var offShiftAssignments = assignments.Where(x => offShiftUsers.Contains(x.UserId)).ToList();

        var timeShiftBatch = CalculateForTimeShifts(timeShifts, timeShiftAssignments, mainMatchRemarks);
        var offShiftBatch = CalculateForOffShifts(offShifts, offShiftAssignments, mainMatchRemarks);

        return timeShiftBatch.Concat(offShiftBatch);

    }

    public IEnumerable<OvertimeData> CalculateForTimeShifts(
        IList<EmployeeWorkingShift> timeShifts,
        IList<Assignment> assignments,
        IList<MainMatch> mainMatchRemarks)
    {
        foreach (var shift in timeShifts)
        {
            var userAssignments = assignments.Where(x => x.UserId == shift.EmployeeId).ToList();

            var shiftOt = ShiftOvertimeCalculator.CalculateTimeShiftOvertimeData(shift);
            var assignmentOt =
                AssignmentOvertimeCalculator.CalculateOvertimeData(shift, userAssignments, mainMatchRemarks);
            var otTime = WorkingShiftHelper.RoundUpOvertime(shiftOt + assignmentOt);

            if (otTime > 0)
            {
                yield return new OvertimeData
                {
                    UserId = shift.EmployeeId,
                    IsOffDay = false,
                    ReportDate = shift.ShiftDate,
                    Hour = otTime
                };
            }
        }
    }

    public IEnumerable<OvertimeData> CalculateForOffShifts(
        IList<EmployeeWorkingShift> offShift,
        IList<Assignment> assignments,
        IList<MainMatch> mainMatchRemarks)
    {
        foreach (var shift in offShift)
        {
            var userAssignments = assignments.Where(x => x.UserId == shift.EmployeeId).ToList();

            var shiftOt = ShiftOvertimeCalculator.CalculateOffShiftOvertimeData(shift);
            var assignmentOt =
                AssignmentOvertimeCalculator.CalculateOvertimeData(shift, userAssignments,  mainMatchRemarks);
            var otTime = WorkingShiftHelper.RoundUpOvertime(shiftOt + assignmentOt);

            if (otTime > 0)
            {
                yield return new OvertimeData
                {
                    UserId = shift.EmployeeId,
                    IsOffDay = true,
                    ReportDate = shift.ShiftDate,
                    Hour = otTime
                };
            }
        }
    }
}