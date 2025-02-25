namespace OWP.SumReport.UnitTests.Prematch.Services.League;

using NSubstitute;
using OWP.SumReport.PrematchReport.Constants;
using OWP.SumReport.PrematchReport.Services;
using OWP.SumReport.PrematchReport.Services.League;
using PrematchReport.Models;
using Xunit;
using Xunit.Asserts.Compare;

public class DataReportServiceUnitTests
{
    private readonly DbWriterService dbWriterService = Substitute.For<DbWriterService>();

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
      =>
      [
          new()
          {
              EventDate = new DateTime(2020, 10, 2),
              GroupId = 0,
              OwpLeagueId = 1,
              UserId = 8
          },

          new()
          {
              EventDate = new DateTime(2020, 10, 2),
              GroupId = 0,
              OwpLeagueId = 1,
              UserId = 12
          },

          new()
          {
              EventDate = new DateTime(2020, 10, 2),
              GroupId = 0,
              OwpLeagueId = 10,
              UserId = 8
          }
      ];

    public List<DataReport> MockExpected()
      =>
      [
          new()
          {
              UserId = 8,
              Amount = 2,
              BetType = 1,
              EventDate = new DateTime(2020, 10, 2),
              GroupId = 0
          },

          new()
          {
              UserId = 12,
              Amount = 1,
              BetType = 1,
              EventDate = new DateTime(2020, 10, 2),
              GroupId = 0
          }
      ];
}