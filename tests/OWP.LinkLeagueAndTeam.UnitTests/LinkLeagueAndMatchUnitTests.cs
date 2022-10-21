using NSubstitute;
using OWP.LinkLeagueAndTeam.Models;
using OWP.LinkLeagueAndTeam.Services;
using Xunit;
using Xunit.Asserts.Compare;

namespace OWP.LinkLeagueAndTeam.UnitTests;

public class LinkLeagueAndMatchUnitTests
{
    private readonly CacheManipulator cache;
    private readonly LinkLeagueAndTeamRepository repository;

    public LinkLeagueAndMatchUnitTests()
    {
        cache = Substitute.For<CacheManipulator>();
        repository = Substitute.For<LinkLeagueAndTeamRepository>();
        MockFunction();
    }

    [Fact]
    public void BuildEndTime_StartTime_ReturnEndTime_Successfully()
    {
        // Arrange
        var now = DateTime.Now;

        // Act
        var actual = LinkLeagueAndTeamServiceImp.BuildEndTime(now);

        // Assert
        Assert.Equal(now.AddDays(7), actual);
    }

    [Fact]
    public async Task UpdateTeamInfo_TeamInfo_Successfully()
    {
        // Arrange
        var linkLeagueAndMatch = new LinkLeagueAndTeamServiceImp(cache, repository);

        var teamInfo = new List<TeamInfo>
            {
                new TeamInfo
                {
                    TeamId = 123,
                    LeagueCategoryId = 5
                }
            };

        // Act
        var actual = await Record.ExceptionAsync(() => linkLeagueAndMatch.UpdateTeamInfo(teamInfo));

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async Task Execute_ReturnTimeRange_Correct()
    {
        // Arrange
        var linkLeagueAndMatch = new LinkLeagueAndTeamServiceImp(cache, repository);
        var startTime = new DateTime(2021, 10, 21, 0, 0, 0);

        // Act
        await linkLeagueAndMatch.Execute();

        // Assert
        await cache.Received().SaveStartTimeQueryTeamInfo(Arg.Any<DateTime>());
    }

    [Fact]
    public async Task PreExecute_ReturnTimeRange_Correct()
    {
        // Arrange
        var linkLeagueAndMatch = new LinkLeagueAndTeamServiceImp(cache, repository);
        var startTime = new DateTime(2021, 1, 1, 0, 0, 0);
        var currentTime = DateTime.Now.AddHours(12);

        // Act
        var actual = await linkLeagueAndMatch.PreExecute();

        // Assert
        var expected = new TimeRange
        {
            StartTime = startTime,
            EndTime = startTime.AddDays(7),
            FinishRunTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 0, 0, 0)
        };
        DeepAssert.Equal(actual, expected);
    }

    [Fact]
    public async Task Execute_Should_Return_EndTime_Correct()
    {
        // Arrange
        var linkLeagueAndMatch = new LinkLeagueAndTeamServiceImp(cache, repository);
        var startTime = new DateTime(2021, 1, 1, 0, 0, 0);
        var currentTime = DateTime.Now;
        var teamInfo = new List<TeamInfo>
        {
            new() { TeamId = 11, EventDateTime = new DateTime(2022, 10, 20)},
            new() { TeamId = 11, EventDateTime = new DateTime(2022, 10, 21)},
            new() { TeamId = 22, EventDateTime = new DateTime(2022, 10, 20)},
        };
        repository.GetTeamInfoInTimeRange(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(teamInfo);
        var timeRange = new TimeRange
        {
            StartTime = startTime,
            EndTime = startTime.AddDays(7),
            FinishRunTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 0, 0, 0)
        };

        // Act
        var actual = await linkLeagueAndMatch.Execute(timeRange);

        //Assert
        var expected = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 0, 0, 0);
        DeepAssert.Equal(actual, expected);
    }

    private void MockFunction()
    {
        var startTime = DateTime.Now;
        var endTime = startTime.AddDays(7);

        cache
            .GetStartTimeFromCache()
            .Returns(new DateTime(2021, 1, 1, 0, 0, 0));

        repository
            .GetTeamInfoInTimeRange(startTime, endTime)
            .Returns(new List<TeamInfo>
            {
                    new TeamInfo
                    {
                        TeamId = 123,
                        LeagueCategoryId = 1
                    },
                    new TeamInfo
                    {
                        TeamId = 123,
                        LeagueCategoryId = 1
                    }
            });

        repository
            .InsertOrUpdateTeamInfo(new TeamInfo
            {
                TeamId = 123,
                LeagueId = 5,
                LeagueCategoryId = 12455
            })
            .Returns(Task.CompletedTask);
    }
}