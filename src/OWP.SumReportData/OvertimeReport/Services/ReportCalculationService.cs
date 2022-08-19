using OWP.SumReportData.OvertimeReport.Calculators;
using OWP.SumReportData.OvertimeReport.Helpers;
using OWP.SumReportData.OvertimeReport.Models;

namespace OWP.SumReportData.OvertimeReport.Services;

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

    public async Task SumOvertimeReport(DateTime date, bool isGenerateData)
    {
        var reportData = await CalculateOvertimeReport(date, isGenerateData);

        await repository.SaveData(date, reportData);
    }

    public async Task<IEnumerable<OvertimeData>> CalculateOvertimeReport(DateTime date, bool isGenerateData)
    {
        var workingShifts = await repository.GetEmployeeWorkingShift(date, isGenerateData);

        if (workingShifts.Any())
        {
            var userIds = workingShifts.Select(x => x.EmployeeId);
            var (assignments, mainMatchRemarks) = await repository.GetTradersAssignments(date, userIds, isGenerateData);
            var timeShifts = workingShifts.Where(x => !x.RefShiftId.HasValue);
            var offShifts = workingShifts.Where(x => x.RefShiftId.HasValue);
            var timeShiftUsers = timeShifts.Select(x => x.EmployeeId);
            var offShiftUsers = offShifts.Select(x => x.EmployeeId);
            var timeShiftAssignments = assignments.Where(x => timeShiftUsers.Contains(x.UserId));
            var offShiftAssignments = assignments.Where(x => offShiftUsers.Contains(x.UserId));

            var timeShiftBatch = CalculateForTimeShifts(timeShifts, timeShiftAssignments, mainMatchRemarks);
            var offShiftBatch = CalculateForOffShifts(offShifts, offShiftAssignments, mainMatchRemarks);

            return timeShiftBatch.Concat(offShiftBatch);
        }

        return new List<OvertimeData>();
    }

    public IEnumerable<OvertimeData> CalculateForTimeShifts(
        IEnumerable<EmployeeWorkingShift> timeShifts,
        IEnumerable<Assignment> assignments,
        IEnumerable<MainMatch> mainMatchRemarks)
    {
        foreach (var shift in timeShifts)
        {
            var userAssignments = assignments.Where(x => x.UserId == shift.EmployeeId);

            var shiftOt = ShiftOvertimeCalculator.CalculateTimeShiftOvertimeData(shift);
            var assignmentOt =
                AssignmentOvertimeCalculator.CalculateOvertimeData(userAssignments, shift, mainMatchRemarks);
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
        IEnumerable<EmployeeWorkingShift> offShift,
        IEnumerable<Assignment> assignments,
        IEnumerable<MainMatch> mainMatchRemarks)
    {
        foreach (var shift in offShift)
        {
            var userAssignments = assignments.Where(x => x.UserId == shift.EmployeeId);

            var shiftOt = ShiftOvertimeCalculator.CalculateOffShiftOvertimeData(shift);
            var assignmentOt =
                AssignmentOvertimeCalculator.CalculateOvertimeData(userAssignments, shift, mainMatchRemarks);
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