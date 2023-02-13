namespace OWP.SumReport.PrematchReport.UnitTests.Services.Match;

using Models;
using NSubstitute;
using PrematchReport.Services;
using PrematchReport.Services.Match;
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
            new()
            {
                EventDate = new DateTime(2020, 10, 2),
                GroupId = 0,
                IsDay = true,
                OWPLeagueId = 1,
                PreMatchLeagueId = 1,
                UserId = 8,
            },
            new()
            {
                EventDate = new DateTime(2020, 10, 2),
                GroupId = 0,
                IsDay = false,
                OWPLeagueId = 1,
                PreMatchLeagueId = 1,
                UserId = 8,
            },
            new()
            {
                EventDate = new DateTime(2020, 10, 2),
                GroupId = 0,
                IsDay = false,
                OWPLeagueId = 10,
                PreMatchLeagueId = 1,
                UserId = 8,
            },
            new()
            {
                EventDate = new DateTime(2020, 10, 2),
                GroupId = 0,
                IsDay = true,
                OWPLeagueId = 2,
                PreMatchLeagueId = 1,
                UserId = 8,
            },
            new()
            {
                EventDate = new DateTime(2020, 10, 2),
                GroupId = 0,
                IsDay = true,
                OWPLeagueId = 1,
                PreMatchLeagueId = 1,
                UserId = 12,
            },
            new()
            {
                EventDate = new DateTime(2020, 10, 2),
                GroupId = 1,
                IsDay = true,
                OWPLeagueId = 3,
                PreMatchLeagueId = 1,
                UserId = 8,
            },
            new()
            {
                EventDate = new DateTime(2020, 10, 2),
                GroupId = 2,
                IsDay = false,
                OWPLeagueId = 4,
                PreMatchLeagueId = 3,
                UserId = 8,
            },
        };

    public List<TotalDataMatch> MockTotalDataMatch()
         => new()
         {
             new()
             {
                 OWPLeagueId = 1,
                 IsMatchDay = true,
                 Total = 1
             },
             new()
             {
                 OWPLeagueId = 1,
                 IsMatchDay = false,
                 Total = 2
             },
             new()
             {
                 OWPLeagueId = 10,
                 Total = 8,
                 IsMatchDay = true,
             },
             new()
             {
                 OWPLeagueId = 10,
                 Total = 9,
                 IsMatchDay = false,
             },
             new()
             {
                 OWPLeagueId = 2,
                 Total = 1,
                 IsMatchDay = true,
             },
             new()
             {
                 OWPLeagueId = 2,
                 Total = 5,
                 IsMatchDay = false,
             },
             new()
             {
                 OWPLeagueId = 3,
                 IsMatchDay = true,
                 Total = 2,
             },
             new()
             {
                 OWPLeagueId = 3,
                 IsMatchDay = false,
                 Total = 6,
             },
             new()
             {
                 OWPLeagueId = 4,
                 IsMatchDay = true,
                 Total = 3,
             },
             new()
             {
                 OWPLeagueId = 4,
                 IsMatchDay = false,
                 Total = 30,
             }
         };

    public List<DataReport> MockExpected()
     => new()
     {
         new()
         {
             UserId = 8,
             Amount = 11,
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
         new()
         {
             UserId = 8,
             Amount = 2,
             BetType = 1,
             GroupId = 1,
             EventDate = new DateTime(2020, 10, 2)
         },
         new()
         {
             UserId = 8,
             Amount = 30,
             BetType = 1,
             GroupId = 2,
             EventDate = new DateTime(2020, 10, 2)
         },
     };
}