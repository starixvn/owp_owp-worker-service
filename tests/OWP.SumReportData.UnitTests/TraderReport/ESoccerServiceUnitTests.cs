using NSubstitute;
using OWP.SumReportData.TraderReport;
using OWP.SumReportData.TraderReport.Models;
using OWP.SumReportData.TraderReport.Services;
using Xunit;
using Xunit.Asserts.Compare;

namespace OWP.SumReportData.UnitTests.TraderReport
{
    public class ESoccerServiceUnitTests
    {
        private readonly ESoccerServiceImp service;

        private readonly TraderRepository repository;

        public ESoccerServiceUnitTests()
        {
            repository = Substitute.For<TraderRepository>();
            service = new ESoccerServiceImp(repository);
        }

        [Fact]
        public async Task SumESoccerFields()
        {
            repository.GetSaBaAssignmentByDate(Arg.Any<DateTime>()).Returns(MockAssignments());

            var result = await service.SumESoccerFields(new DateTime(2022, 1, 20));

            DeepAssert.Equal(MockExpect(), result.ToList());
        }

        private List<ESoccerByUser> MockExpect()
            => new List<ESoccerByUser>
            {
                new ESoccerByUser
                {
                    UserId = 111,
                    ESoccer5Mins = 1,
                    ESoccer6Mins = 1,
                    ESoccer15Mins = 2,
                    ESoccer16Mins = 1,
                    ESoccer20Mins = 2
                },
                new ESoccerByUser
                {
                    UserId = 222,
                    ESoccer5Mins = 1,
                    ESoccer6Mins = 1,
                    ESoccer15Mins = 1,
                    ESoccer16Mins = 0,
                    ESoccer20Mins = 0
                }
            };

        private static List<Assignment> MockAssignments()
            => new List<Assignment>
            {
                new Assignment { UserId = 111, OWPMatchId = 1, OWPLeagueName = "SABA - 5 Mins Play"},
                new Assignment { UserId = 111, OWPMatchId = 2, OWPLeagueName = "SABA - 6 Mins Play"},
                new Assignment { UserId = 111, OWPMatchId = 3, OWPLeagueName = "SABA - 15 Mins Play"},
                new Assignment { UserId = 111, OWPMatchId = 4, OWPLeagueName = "SABA - 15 Mins Play"},
                new Assignment { UserId = 111, OWPMatchId = 5, OWPLeagueName = "SABA - 16 Mins Play"},
                new Assignment { UserId = 111, OWPMatchId = 6, OWPLeagueName = "SABA - 20 Mins Play"},
                new Assignment { UserId = 111, OWPMatchId = 7, OWPLeagueName = "SABA - 20 Mins Play"},
                new Assignment { UserId = 111, OWPMatchId = 7, OWPLeagueName = "SABA - 20 Mins Play"},
                new Assignment { UserId = 111, OWPMatchId = 9, OWPLeagueName = "SABA"},
                new Assignment { UserId = 222, OWPMatchId = 10, OWPLeagueName = "SABA - 5 Mins Play"},
                new Assignment { UserId = 222, OWPMatchId = 11, OWPLeagueName = "SABA - 6 Mins Play"},
                new Assignment { UserId = 222, OWPMatchId = 12, OWPLeagueName = "SABA - 15 Mins Play"}
            };
    }
}