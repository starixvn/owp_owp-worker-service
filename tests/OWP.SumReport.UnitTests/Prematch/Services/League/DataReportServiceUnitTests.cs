namespace OWP.SumReport.PrematchReport.UnitTests.Services.League;

using Constants;
using Models;
using NSubstitute;
using PrematchReport.Services;
using PrematchReport.Services.League;
using Xunit;
using Xunit.Asserts.Compare;

public class DataReportServiceUnitTests
{
    private readonly DbWriterService dbWriterService;

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
        var actual = service.BuildSingleLeagueGroupReport(leagueInfos, Constant.HDPID);

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
              OWPLeagueId = 1,
              UserId = 8,
          },
          new()
          {
              EventDate = new DateTime(2020, 10, 2),
              GroupId = 0,
              OWPLeagueId = 1,
              UserId = 12,
          },
          new()
          {
              EventDate = new DateTime(2020, 10, 2),
              GroupId = 0,
              OWPLeagueId = 10,
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