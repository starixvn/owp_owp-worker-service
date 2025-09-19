using NSubstitute;
using OWP.SumReport.PrematchReport.Constants;
using OWP.SumReport.PrematchReport.Models;
using OWP.SumReport.PrematchReport.Services;
using OWP.SumReport.PrematchReport.Services.League;
using Xunit;
using Xunit.Asserts.Compare;

namespace OWP.SumReport.PrematchReport.UnitTests.Services.League
{
    public class DataReportServiceUnitTests
    {
        private DbWriterService dbWriterService;

        public DataReportServiceUnitTests()
        {
            dbWriterService = Substitute.For<DbWriterService>();
        }

        [Fact]
        public void BuildLeagueReport_Always_ReturnDataReport()
        {
            //Arrange
            var service = new DataReportBuilderServiceImp(dbWriterService);
            var leagueInfos = MockLeagueInfo();

            //Act
            var actual = service.BuildSingleLeagueGroupReport(leagueInfos, Constant.HdpId);

            //Assert
            var expected = MockExpected();
            DeepAssert.Equal(expected, actual);
        }

        public List<LeagueInfo> MockLeagueInfo()
          => new()
          {
              new()
              {
                  EventDate = new DateTime(2020, 10, 2),
                  GroupId = 0,
                  OwpLeagueId = 1,
                  UserId = 8,
              },
              new()
              {
                  EventDate = new DateTime(2020, 10, 2),
                  GroupId = 0,
                  OwpLeagueId = 1,
                  UserId = 12,
              },
              new()
              {
                  EventDate = new DateTime(2020, 10, 2),
                  GroupId = 0,
                  OwpLeagueId = 10,
                  UserId = 8,
              }
          };

        public List<DataReport> MockExpected()
          => new()
          {
              new()
              {
                  UserId = 8,
                  Amount = 2,
                  BetType = 1,
                  EventDate = new DateTime(2020, 10, 2),
                  GroupId = 0,
              },
              new()
              {
                  UserId = 12,
                  Amount = 1,
                  BetType = 1,
                  EventDate = new DateTime(2020, 10, 2),
                  GroupId = 0,
              },
          };
    }
}