namespace OWP.SumReport.UnitTests.Prematch.Services.Match;

using NSubstitute;
using PrematchReport.Models;
using OWP.SumReport.PrematchReport.Services;
using OWP.SumReport.PrematchReport.Services.Match;
using PrematchReport;
using Xunit;
using Xunit.Asserts.Compare;

public class DataMatchReportServiceUnitests
{
    private readonly Repository repository;
    private readonly DbWriterService dbWriterService;

    public DataMatchReportServiceUnitests()
    {
        repository = Substitute.For<Repository>();
        dbWriterService = Substitute.For<DbWriterService>();
    }

    [Fact]
    public async Task BuildMatchReport_Always_ReturnDataReport()
    {
        //Arrange
        var matchservice = new DataMatchReportServiceImp(repository, dbWriterService);
        repository.GetTotalMatchByLeagueIds(Arg.Compat.Any<IList<int>>(), Arg.Compat.Any<DateTime>(), Arg.Compat.Any<DateTime>(), Arg.Compat.Any<DateTime>())
            .Returns(MockTotalDataMatch());
        var leagueDetailInfos = MockLeagueDetailInfo();

        //Act
        var actual = await matchservice.BuildMatchReport(leagueDetailInfos, DateTime.Now, null, 1);

        //Assert
        var expected = MockExpected();
        DeepAssert.Equal(expected, actual);
    }

    public List<LeagueDetailInfo> MockLeagueDetailInfo()
        => new()
        {
            new LeagueDetailInfo
            {
                EventDate = new DateTime(2020, 10, 2),
                GroupId = 0,
                IsDay = true,
                OwpLeagueId = 1,
                PreMatchLeagueId = 1,
                UserId = 8
            },
            new LeagueDetailInfo
            {
                EventDate = new DateTime(2020, 10, 2),
                GroupId = 0,
                IsDay = false,
                OwpLeagueId = 1,
                PreMatchLeagueId = 1,
                UserId = 8
            },
            new LeagueDetailInfo
            {
                EventDate = new DateTime(2020, 10, 2),
                GroupId = 0,
                IsDay = false,
                OwpLeagueId = 10,
                PreMatchLeagueId = 1,
                UserId = 8
            },
            new LeagueDetailInfo
            {
                EventDate = new DateTime(2020, 10, 2),
                GroupId = 0,
                IsDay = true,
                OwpLeagueId = 2,
                PreMatchLeagueId = 1,
                UserId = 8
            },
            new LeagueDetailInfo
            {
                EventDate = new DateTime(2020, 10, 2),
                GroupId = 0,
                IsDay = true,
                OwpLeagueId = 1,
                PreMatchLeagueId = 1,
                UserId = 12
            },
            new LeagueDetailInfo
            {
                EventDate = new DateTime(2020, 10, 2),
                GroupId = 1,
                IsDay = true,
                OwpLeagueId = 3,
                PreMatchLeagueId = 1,
                UserId = 8
            },
            new LeagueDetailInfo
            {
                EventDate = new DateTime(2020, 10, 2),
                GroupId = 2,
                IsDay = false,
                OwpLeagueId = 4,
                PreMatchLeagueId = 3,
                UserId = 8
            }
        };

    public List<TotalDataMatch> MockTotalDataMatch()
         => new()
         {
             new TotalDataMatch
             {
                 OwpLeagueId = 1,
                 IsMatchDay = true,
                 Total = 1
             },
             new TotalDataMatch
             {
                 OwpLeagueId = 1,
                 IsMatchDay = false,
                 Total = 2
             },
             new TotalDataMatch
             {
                 OwpLeagueId = 10,
                 Total = 8,
                 IsMatchDay = true
             },
             new TotalDataMatch
             {
                 OwpLeagueId = 10,
                 Total = 9,
                 IsMatchDay = false
             },
             new TotalDataMatch
             {
                 OwpLeagueId = 2,
                 Total = 1,
                 IsMatchDay = true
             },
             new TotalDataMatch
             {
                 OwpLeagueId = 2,
                 Total = 5,
                 IsMatchDay = false
             },
             new TotalDataMatch
             {
                 OwpLeagueId = 3,
                 IsMatchDay = true,
                 Total = 2
             },
             new TotalDataMatch
             {
                 OwpLeagueId = 3,
                 IsMatchDay = false,
                 Total = 6
             },
             new TotalDataMatch
             {
                 OwpLeagueId = 4,
                 IsMatchDay = true,
                 Total = 3
             },
             new TotalDataMatch
             {
                 OwpLeagueId = 4,
                 IsMatchDay = false,
                 Total = 30
             }
         };

    public List<DataReport> MockExpected()
     => new()
     {
         new DataReport
         {
             UserId = 8,
             Amount = 11,
             BetType = 1,
             EventDate = new DateTime(2020, 10, 2),
             GroupId = 0
         },
         new DataReport
         {
             UserId = 12,
             Amount = 1,
             BetType = 1,
             EventDate = new DateTime(2020, 10, 2),
             GroupId = 0
         },
         new DataReport
         {
             UserId = 8,
             Amount = 2,
             BetType = 1,
             GroupId = 1,
             EventDate = new DateTime(2020, 10, 2)
         },
         new DataReport
         {
             UserId = 8,
             Amount = 30,
             BetType = 1,
             GroupId = 2,
             EventDate = new DateTime(2020, 10, 2)
         }
     };
}