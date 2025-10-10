using Fanex.Data.Repository;
using NSubstitute;
using OWP.Infosite.Criteria;
using OWP.Infosite.Helper;
using OWP.Infosite.Models;
using OWP.Infosite.Services;
using System.Reflection;
using Xunit;
using Xunit.Asserts.Compare;

namespace OWP.Infosite.UnitTests;

public class EventPickingLogsServiceTestRunner
{
    [Fact]
    public void EventPickingLogsService_Constructor_ShouldCreateInstance()
    {
        // Arrange
        var mockRepository = Substitute.For<IDynamicRepository>();

        // Act
        var service = new EventPickingLogsServiceImp(mockRepository);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public async Task EventPickingLogsService_BuildLogs_WithEmptyLists_ShouldNotCallRepository()
    {
        // Arrange
        var mockRepository = Substitute.For<IDynamicRepository>();
        var service = new EventPickingLogsServiceImp(mockRepository);
        var queriedResult = new List<Event>();
        var infoSiteDbInfos = new List<InfoSiteDbInfo>();
        var eventDate = DateTime.Now;
        var fromService = "TestService";

        // Act
        await service.BuildLogs(queriedResult, infoSiteDbInfos, eventDate, fromService);

        // Assert
        await mockRepository.DidNotReceive().ExecuteAsync(Arg.Any<SaveLogsChangeCriteria>());
    }

    [Fact]
    public void EventPickingLogsService_AddLogUpdateCheckingStatus_ShouldAddCorrectLogEntry()
    {
        // Arrange
        var mockRepository = Substitute.For<IDynamicRepository>();
        var service = new EventPickingLogsServiceImp(mockRepository);
        var infoSiteMatchId = 123;
        var loggedChanges = new List<LoggedChangeModel>();
        var fromService = "TestService";

        // Act
        service.AddLogUpdateCheckingStatus(infoSiteMatchId, loggedChanges, fromService);

        // Assert
        Assert.Single(loggedChanges);
        var logEntry = loggedChanges.First();
        Assert.Equal(infoSiteMatchId, logEntry.InfoSiteMatchId);
        Assert.Null(logEntry.UpdateTypeId);
        Assert.Equal("Update the <b>Checking status</b>", logEntry.Action);
        Assert.Equal("Unchecked", logEntry.NewValue);
        Assert.Equal("Checked", logEntry.OldValue);
        Assert.Equal(fromService, logEntry.FromService);
        Assert.True(logEntry.UpDateTime > DateTime.MinValue);
    }

    [Fact]
    public async Task EventPickingLogsService_BuildLogs_WithNewMatches_ShouldCreateNewMatchLogs()
    {
        // Arrange
        var mockRepository = Substitute.For<IDynamicRepository>();
        var service = new EventPickingLogsServiceImp(mockRepository);
        var queriedResult = new List<Event>
        {
            CreateTestEvent(1, "Team A", "Team B"),
            CreateTestEvent(2, "Team C", "Team D")
        };
        var infoSiteDbInfos = new List<InfoSiteDbInfo>();
        var eventDate = DateTime.Now;
        var fromService = "TestService";

        // Act
        await service.BuildLogs(queriedResult, infoSiteDbInfos, eventDate, fromService);

        // Assert
        await mockRepository.Received(1).ExecuteAsync(Arg.Any<SaveLogsChangeCriteria>());
    }

    // Helper method to create test data
    private static Event CreateTestEvent(int infoSiteMatchId, string homeName, string awayName,
        string remark2 = "", byte? matchStatus = null)
    {
        var commonMatch = new CommonMatchModel
        {
            match_id = infoSiteMatchId,
            league_name = "Test League",
            home_name = homeName,
            away_name = awayName,
            ext_info = new SoccerEventSchedule
            {
                remark2 = remark2,
                sport_id = 1,
                rd_checked = true,
                update_time = DateTime.Now,
                create_match_id = infoSiteMatchId + 1000,
                game_date = DateTime.Now.AddDays(1),
                round = "1",
                code = "TEST",
                agent = "Test Agent",
                venue = "Test Venue",
                local_tv = "Test TV",
                tv_live = "Live TV",
                website1 = "www.test1.com",
                website2 = "www.test2.com",
                website3 = "www.test3.com",
                website4 = "www.test4.com",
                provider = "Test Provider",
                provider_use = "Test Provider Use",
                et_pk = "ETPK123",
                remark = "Test Remark",
                round_total = "38",
                create_match_code = "TEST123",
                home_remark = "Home Remark",
                away_remark = "Away Remark"
            }
        };

        var evt = new Event(commonMatch);

        // Set additional properties using reflection if needed
        if (matchStatus.HasValue)
        {
            var matchStatusProperty = typeof(Event).GetProperty("MatchStatus");
            matchStatusProperty?.SetValue(evt, matchStatus.Value);
        }

        return evt;
    }
}
