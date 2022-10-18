using NSubstitute;
using OWP.SumReportData.OvertimeReport;
using OWP.SumReportData.OvertimeReport.Models;
using OWP.SumReportData.OvertimeReport.Services;
using Xunit;

namespace OWP.SumReportData.UnitTests.OvertimeReport.Services
{
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
        public async Task SumOvertimeReport_CorectModel()
        {
            var date = new DateTime(2022, 10, 18);
            var mockedEmployeeWorkingShift = new List<EmployeeWorkingShift>{
                new() { EmployeeId = 11, RefShiftId = 100 },
                new() { EmployeeId = 22, RefShiftId = null }
            };
            var mockedAssignment = new List<Assignment>
            {
                new(){ OWPMatchId = 111, UserId = 11, ParentMatchId = null },
                new(){ OWPMatchId = 222, UserId = 2, Bettype = -1 },
            };
            var mockedMainMatch = new List<MainMatch>
            {
                new() { OWPMatchId = 111, Remark = ""  }
            };

            repository.GetEmployeeWorkingShift(date, false).Returns(mockedEmployeeWorkingShift);
            repository.GetTradersAssignments(date, new List<int> { 11, 22 }, false)
                .Returns(x => (mockedAssignment, mockedMainMatch));

            var actual = service.SumOvertimeReport(date);
        }
    }
}
