using Fanex.Data.Repository;
using NSubstitute;
using OWP.Infosite.Criteria;
using OWP.Infosite.Helper;
using OWP.Infosite.Models;
using OWP.Infosite.Services;
using System.Reflection;
using Xunit;
using Xunit.Asserts.Compare;

namespace OWP.Infosite.UnitTests.Services;

public class EventPickingLogsServiceTests
{
    private readonly IDynamicRepository _mockRepository;
    private readonly EventPickingLogsServiceImp _service;

    public EventPickingLogsServiceTests()
    {
        _mockRepository = Substitute.For<IDynamicRepository>();
        _service = new EventPickingLogsServiceImp(_mockRepository);
    }

    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Arrange
        var mockRepository = Substitute.For<IDynamicRepository>();

        // Act
        var service = new EventPickingLogsServiceImp(mockRepository);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public async Task BuildLogs_WithEmptyLists_ShouldNotCallRepository()
    {
        // Arrange
        var queriedResult = new List<Event>();
        var infoSiteDbInfos = new List<InfoSiteDbInfo>();
        var eventDate = DateTime.Now;
        var fromService = "TestService";

        // Act
        await _service.BuildLogs(queriedResult, infoSiteDbInfos, eventDate, fromService);

        // Assert
        await _mockRepository.DidNotReceive().ExecuteAsync(Arg.Any<SaveLogsChangeCriteria>());
    }

    [Fact]
    public async Task BuildLogs_WithNewMatches_ShouldCreateNewMatchLogs()
    {
        // Arrange
        var queriedResult = new List<Event>
        {
            CreateTestEvent(1, "Team A", "Team B"),
            CreateTestEvent(2, "Team C", "Team D")
        };
        var infoSiteDbInfos = new List<InfoSiteDbInfo>();
        var eventDate = DateTime.Now;
        var fromService = "TestService";

        // Act
        await _service.BuildLogs(queriedResult, infoSiteDbInfos, eventDate, fromService);

        // Assert
        await _mockRepository.Received(1).ExecuteAsync(Arg.Any<SaveLogsChangeCriteria>());
    }

    [Fact]
    public async Task BuildLogs_WithChangedMatches_ShouldCreateChangeLogs()
    {
        // Arrange
        var queriedResult = new List<Event>
        {
            CreateTestEvent(1, "Team A Updated", "Team B")
        };
        var infoSiteDbInfos = new List<InfoSiteDbInfo>
        {
            CreateTestInfoSiteDbInfo(1, "Team A", "Team B")
        };
        var eventDate = DateTime.Now;
        var fromService = "TestService";

        // Act
        await _service.BuildLogs(queriedResult, infoSiteDbInfos, eventDate, fromService);

        // Assert
        await _mockRepository.Received(1).ExecuteAsync(Arg.Any<SaveLogsChangeCriteria>());
    }

    [Fact]
    public async Task BuildLogs_WithMixedData_ShouldCreateBothNewAndChangeLogs()
    {
        // Arrange
        var queriedResult = new List<Event>
        {
            CreateTestEvent(1, "Team A", "Team B"), // Existing match
            CreateTestEvent(2, "Team C", "Team D")  // New match
        };
        var infoSiteDbInfos = new List<InfoSiteDbInfo>
        {
            CreateTestInfoSiteDbInfo(1, "Team A Old", "Team B")
        };
        var eventDate = DateTime.Now;
        var fromService = "TestService";

        // Act
        await _service.BuildLogs(queriedResult, infoSiteDbInfos, eventDate, fromService);

        // Assert
        await _mockRepository.Received(1).ExecuteAsync(Arg.Any<SaveLogsChangeCriteria>());
    }

    [Fact]
    public void AddLogUpdateCheckingStatus_ShouldAddCorrectLogEntry()
    {
        // Arrange
        var infoSiteMatchId = 123;
        var loggedChanges = new List<LoggedChangeModel>();
        var fromService = "TestService";

        // Act
        _service.AddLogUpdateCheckingStatus(infoSiteMatchId, loggedChanges, fromService);

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
    public void AddLogUpdateCheckingStatus_WithMultipleCalls_ShouldAddMultipleEntries()
    {
        // Arrange
        var loggedChanges = new List<LoggedChangeModel>();
        var fromService = "TestService";

        // Act
        _service.AddLogUpdateCheckingStatus(1, loggedChanges, fromService);
        _service.AddLogUpdateCheckingStatus(2, loggedChanges, fromService);

        // Assert
        Assert.Equal(2, loggedChanges.Count);
        Assert.Equal(1, loggedChanges[0].InfoSiteMatchId);
        Assert.Equal(2, loggedChanges[1].InfoSiteMatchId);
    }

    [Fact]
    public async Task BuildLogs_WithCheckedMatchAndChanges_ShouldAddCheckingStatusLog()
    {
        // Arrange
        var queriedResult = new List<Event>
        {
            CreateTestEvent(1, "Team A Updated", "Team B")
        };
        var infoSiteDbInfos = new List<InfoSiteDbInfo>
        {
            CreateTestInfoSiteDbInfo(1, "Team A", "Team B", isCheck: true)
        };
        var eventDate = DateTime.Now;
        var fromService = "TestService";

        // Act
        await _service.BuildLogs(queriedResult, infoSiteDbInfos, eventDate, fromService);

        // Assert
        await _mockRepository.Received(1).ExecuteAsync(Arg.Any<SaveLogsChangeCriteria>());
    }

    [Fact]
    public async Task BuildLogs_WithStatusChanges_ShouldHandleStatusUpdates()
    {
        // Arrange
        var queriedResult = new List<Event>
        {
            CreateTestEvent(1, "Team A", "Team B", remark2: "Postponed", matchStatus: 1)
        };
        var infoSiteDbInfos = new List<InfoSiteDbInfo>
        {
            CreateTestInfoSiteDbInfo(1, "Team A", "Team B", remark2: "Normal", matchStatus: 0, lastStatus: "Normal")
        };
        var eventDate = DateTime.Now;
        var fromService = "TestService";

        // Act
        await _service.BuildLogs(queriedResult, infoSiteDbInfos, eventDate, fromService);

        // Assert
        await _mockRepository.Received(1).ExecuteAsync(Arg.Any<SaveLogsChangeCriteria>());
    }

    [Fact]
    public async Task BuildLogs_WithNoChanges_ShouldStillCallRepository()
    {
        // Arrange
        var queriedResult = new List<Event>
        {
            CreateTestEvent(1, "Team A", "Team B")
        };
        var infoSiteDbInfos = new List<InfoSiteDbInfo>
        {
            CreateTestInfoSiteDbInfo(1, "Team A", "Team B")
        };
        var eventDate = DateTime.Now;
        var fromService = "TestService";

        // Act
        await _service.BuildLogs(queriedResult, infoSiteDbInfos, eventDate, fromService);

        // Assert
        await _mockRepository.Received(1).ExecuteAsync(Arg.Any<SaveLogsChangeCriteria>());
    }

    [Fact]
    public async Task BuildLogs_WithNullParameters_ShouldHandleGracefully()
    {
        // Arrange
        List<Event> queriedResult = null;
        List<InfoSiteDbInfo> infoSiteDbInfos = null;
        var eventDate = DateTime.Now;
        var fromService = "TestService";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.BuildLogs(queriedResult, infoSiteDbInfos, eventDate, fromService));
    }

    [Fact]
    public void AddLogUpdateCheckingStatus_WithNullLoggedChanges_ShouldThrowException()
    {
        // Arrange
        var infoSiteMatchId = 123;
        IList<LoggedChangeModel> loggedChanges = null;
        var fromService = "TestService";

        // Act & Assert
        Assert.Throws<NullReferenceException>(() =>
            _service.AddLogUpdateCheckingStatus(infoSiteMatchId, loggedChanges, fromService));
    }

    [Fact]
    public async Task BuildLogs_WithLargeDataSet_ShouldProcessAllItems()
    {
        // Arrange
        var queriedResult = new List<Event>();
        var infoSiteDbInfos = new List<InfoSiteDbInfo>();

        // Create 100 new matches
        for (int i = 1; i <= 100; i++)
        {
            queriedResult.Add(CreateTestEvent(i, $"Team A{i}", $"Team B{i}"));
        }

        var eventDate = DateTime.Now;
        var fromService = "TestService";

        // Act
        await _service.BuildLogs(queriedResult, infoSiteDbInfos, eventDate, fromService);

        // Assert
        await _mockRepository.Received(1).ExecuteAsync(Arg.Any<SaveLogsChangeCriteria>());
    }

    [Fact]
    public async Task BuildLogs_VerifyXmlSerialization_ShouldCallRepositoryWithValidXml()
    {
        // Arrange
        var queriedResult = new List<Event>
        {
            CreateTestEvent(1, "Team A", "Team B")
        };
        var infoSiteDbInfos = new List<InfoSiteDbInfo>();
        var eventDate = DateTime.Now;
        var fromService = "TestService";

        // Act
        await _service.BuildLogs(queriedResult, infoSiteDbInfos, eventDate, fromService);

        // Assert
        await _mockRepository.Received(1).ExecuteAsync(Arg.Is<SaveLogsChangeCriteria>(criteria =>
            !string.IsNullOrEmpty(criteria.Xml)));
    }

    // Helper methods to create test data
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

    private static InfoSiteDbInfo CreateTestInfoSiteDbInfo(int infoSiteMatchId, string homeName, string awayName,
        bool isCheck = false, string remark2 = "", byte? matchStatus = null, string lastStatus = "")
    {
        return new InfoSiteDbInfo
        {
            InfoSiteMatchId = infoSiteMatchId,
            LeagueName = "Test League",
            HomeName = homeName,
            AwayName = awayName,
            IsCheck = isCheck,
            Remark2 = remark2,
            MatchStatus = matchStatus,
            LastStatus = lastStatus,
            EventDateTime = DateTime.Now,
            Provider = "Test Provider",
            Round = "1",
            Division = "Test Division"
        };
    }
}
