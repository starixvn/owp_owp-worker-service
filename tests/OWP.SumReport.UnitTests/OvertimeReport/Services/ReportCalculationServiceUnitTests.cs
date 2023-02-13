using NSubstitute;
using OWP.SumReport.OvertimeReport;
using OWP.SumReport.OvertimeReport.Models;
using OWP.SumReport.OvertimeReport.Services;
using Xunit;
using Xunit.Asserts.Compare;

namespace OWP.SumReport.UnitTests.OvertimeReport.Services;

public class ReportCalculationServiceUnitTests
{
    private readonly OvertimeRepository repository;
    private readonly ReportCalculationServiceImp service;

    public ReportCalculationServiceUnitTests()
    {
        repository = Substitute.For<OvertimeRepository>();
        service = new ReportCalculationServiceImp(repository);
    }

    [Fact]
    public async Task SumOvertimeReport_CorrectModel()
    {
        var date = new DateTime(2022, 10, 18);
        var mockedEmployeeWorkingShift = new List<EmployeeWorkingShift>{
            new() { EmployeeId = 11, RefShiftId = null, ShiftDate = date, ShiftStartTime=date.AddHours(8), ShiftEndTime=date.AddHours(20) },
            new() { EmployeeId = 22, RefShiftId = 300, ShiftDate = date }
        };
        var mockedAssignment = new List<Assignment>
        {
            new(){ OWPMatchId = 111, UserId = 11, ParentMatchId = null, OWPLeagueName="league", Remark="PK" },
        };
        var mockedMainMatch = new List<MainMatch>
        {
            new() { OWPMatchId = 111, Remark = "", EventDateTime = date.AddHours(10)  }
        };
        var expected = new List<OvertimeData>
        {
            new()
            {
                Hour=3m,
                IsOffDay=false,
                ReportDate=new DateTime(2022,10,18),
                UserId=11
            }
        };

        repository.GetEmployeeWorkingShift(date, false).ReturnsForAnyArgs(mockedEmployeeWorkingShift);
        repository.GetTradersAssignments(date, Arg.Any<List<int>>(), false)
            .ReturnsForAnyArgs((mockedAssignment, mockedMainMatch));

        var actual = await service.CalculateOvertimeReport(date, false);
        DeepAssert.Equal(expected, actual.ToList());
    }
}