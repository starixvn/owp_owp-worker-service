using NSubstitute;
using Olympus.OWP.Log.Data.Repository.Criteria;
using Olympus.OWP.Log.Data.Repository.Services;
using OWP.AutoGenerateMaxbetSetting.Models;
using OWP.AutoGenerateMaxbetSetting.Services;
using Match = OWP.AutoGenerateMaxbetSetting.Models.Match;

namespace OWP.AutoGenerateMaxbetSetting.UnitTests.Services;

public class GeneratorServiceTests
{
    private readonly LeagueSettingsRepository _mockRepository;
    private readonly LogWriterService _mockLogger;
    private readonly GeneratorServiceImp _service;
    private readonly LeagueSettingChangeServiceImp _mockLeagueSettingChangeService;

    public GeneratorServiceTests()
    {
        _mockRepository = Substitute.For<LeagueSettingsRepository>();
        _mockLogger = Substitute.For<LogWriterService>();
        _mockLeagueSettingChangeService = Substitute.For<LeagueSettingChangeServiceImp>();
        _service = new GeneratorServiceImp(_mockRepository, _mockLogger, _mockLeagueSettingChangeService);
    }

    [Fact]
    public async Task GenerateNewLeagueSetting_ShouldProcessMatchesAndCreateSettings()
    {
        // Arrange
        var date = new DateTime(2024, 1, 15);
        var matches = new List<Match>
        {
            new() { OwpLeagueId = 1, OwpMatchId = 100, EventDateTime = date, Tv = "TV1", NoOfHDP = "2" },
            new() { OwpLeagueId = 2, OwpMatchId = 200, EventDateTime = date, Tv = "TV2", NoOfHDP = "3" }
        };
        var matchSettings = new List<MatchSetting>
        {
            new() { OwpMatchId = 100, Bettype = 1, Spread = 1.5m, Maxbet = 1000m },
            new() { OwpMatchId = 200, Bettype = 3, Spread = 2.0m, Maxbet = 2000m }
        };

        _mockRepository
            .GetMatchesAndSettings(date)
            .Returns(new Tuple<IEnumerable<Match>, IEnumerable<MatchSetting>>(matches.AsEnumerable(), matchSettings.AsEnumerable()));

        _mockRepository
            .InsertNewLeaguesSettings(Arg.Any<List<LeagueSetting>>())
            .Returns(Task.CompletedTask);

        _mockLogger
            .Execute(Arg.Any<OwpLogCriteria>())
            .Returns(Task.CompletedTask);

        // Act
        await _service.GenerateNewLeagueSetting(date);

        // Assert
        _mockRepository.Received(1).GetMatchesAndSettings(date);
        _mockRepository.Received(1).InsertNewLeaguesSettings(Arg.Any<List<LeagueSetting>>());
        _mockLogger.Received(1).Execute(Arg.Any<OwpLogCriteria>());
    }

    [Fact]
    public async Task GenerateNewLeagueSetting_ShouldHandleEmptyMatches()
    {
        // Arrange
        var date = new DateTime(2024, 1, 15);
        var emptyMatches = new List<Match>();
        var emptySettings = new List<MatchSetting>();

        _mockRepository
            .GetMatchesAndSettings(date)
            .Returns(new Tuple<IEnumerable<Match>, IEnumerable<MatchSetting>>(emptyMatches.AsEnumerable(), emptySettings.AsEnumerable()));

        _mockRepository
            .InsertNewLeaguesSettings(Arg.Any<List<LeagueSetting>>())
            .Returns(Task.CompletedTask);

        _mockLogger
            .Execute(Arg.Any<OwpLogCriteria>())
            .Returns(Task.CompletedTask);

        // Act
        await _service.GenerateNewLeagueSetting(date);

        // Assert
        _mockRepository.Received(1).GetMatchesAndSettings(date);
        _mockRepository.Received(1).InsertNewLeaguesSettings(Arg.Any<List<LeagueSetting>>());
        _mockLogger.Received(1).Execute(Arg.Any<OwpLogCriteria>());
    }

    [Fact]
    public async Task GenerateNewLeagueSetting_ShouldAssociateMatchSettingsWithMatches()
    {
        // Arrange
        var date = new DateTime(2024, 1, 15);
        var matches = new List<Match>
        {
            new() { OwpLeagueId = 1, OwpMatchId = 100, EventDateTime = date, Tv = "TV1", NoOfHDP = "2" },
            new() { OwpLeagueId = 2, OwpMatchId = 200, EventDateTime = date, Tv = "TV2", NoOfHDP = "3" }
        };
        var matchSettings = new List<MatchSetting>
        {
            new() { OwpMatchId = 100, Bettype = 1, Spread = 1.5m, Maxbet = 1000m },
            new() { OwpMatchId = 200, Bettype = 3, Spread = 2.0m, Maxbet = 2000m }
        };

        _mockRepository
            .GetMatchesAndSettings(date)
            .Returns(new Tuple<IEnumerable<Match>, IEnumerable<MatchSetting>>(matches.AsEnumerable(), matchSettings.AsEnumerable()));

        _mockRepository
            .InsertNewLeaguesSettings(Arg.Any<List<LeagueSetting>>())
            .Returns(Task.CompletedTask);

        _mockLogger
            .Execute(Arg.Any<OwpLogCriteria>())
            .Returns(Task.CompletedTask);

        // Act
        await _service.GenerateNewLeagueSetting(date);

        // Assert
        // Verify that matches have their settings associated
        matches[0].MatchSettings.Should().HaveCount(1);
        matches[0].MatchSettings.First().OwpMatchId.Should().Be(100);
        matches[1].MatchSettings.Should().HaveCount(1);
        matches[1].MatchSettings.First().OwpMatchId.Should().Be(200);
    }

    [Fact]
    public async Task GenerateNewLeagueSetting_ShouldLogCorrectInformation()
    {
        // Arrange
        var date = new DateTime(2024, 1, 15);
        var matches = new List<Match>
        {
            new() { OwpLeagueId = 1, OwpMatchId = 100, EventDateTime = date, Tv = "TV1", NoOfHDP = "2" }
        };
        var matchSettings = new List<MatchSetting>
        {
            new() { OwpMatchId = 100, Bettype = 1, Spread = 1.5m, Maxbet = 1000m }
        };

        _mockRepository
            .GetMatchesAndSettings(date)
            .Returns(new Tuple<IEnumerable<Match>, IEnumerable<MatchSetting>>(matches.AsEnumerable(), matchSettings.AsEnumerable()));

        _mockRepository
            .InsertNewLeaguesSettings(Arg.Any<List<LeagueSetting>>())
            .Returns(Task.CompletedTask);

        _mockLogger
            .Execute(Arg.Any<OwpLogCriteria>())
            .Returns(Task.CompletedTask);

        // Act
        await _service.GenerateNewLeagueSetting(date);

        // Assert
        _mockLogger.Received(1).Execute(
            Arg.Is<OwpLogCriteria>(c =>
                c.Action == "Generator" &&
                !string.IsNullOrEmpty(c.Payload) &&
                c.CreatedDate.Date == DateTime.Now.AddHours(12).Date));
    }

    [Fact]
    public async Task GenerateNewLeagueSetting_ShouldHandleRepositoryExceptions()
    {
        // Arrange
        var date = new DateTime(2024, 1, 15);

        _mockRepository
            .GetMatchesAndSettings(date)
            .Returns(Task.FromException<Tuple<IEnumerable<Match>, IEnumerable<MatchSetting>>>(new Exception("Database error")));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GenerateNewLeagueSetting(date));
    }

    [Fact]
    public async Task GenerateNewLeagueSetting_ShouldHandleLoggerExceptions()
    {
        // Arrange
        var date = new DateTime(2024, 1, 15);
        var matches = new List<Match>();
        var matchSettings = new List<MatchSetting>();

        _mockRepository
            .GetMatchesAndSettings(date)
            .Returns(new Tuple<IEnumerable<Match>, IEnumerable<MatchSetting>>(matches.AsEnumerable(), matchSettings.AsEnumerable()));

        _mockRepository
            .InsertNewLeaguesSettings(Arg.Any<List<LeagueSetting>>())
            .Returns(Task.CompletedTask);

        _mockLogger
            .Execute(Arg.Any<OwpLogCriteria>())
            .Returns(Task.FromException(new Exception("Logging error")));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GenerateNewLeagueSetting(date));
    }

    [Fact]
    public async Task GenerateNewLeagueSetting_ShouldHandleInsertExceptions()
    {
        // Arrange
        var date = new DateTime(2024, 1, 15);
        var matches = new List<Match>();
        var matchSettings = new List<MatchSetting>();

        _mockRepository
            .GetMatchesAndSettings(date)
            .Returns(new Tuple<IEnumerable<Match>, IEnumerable<MatchSetting>>(matches.AsEnumerable(), matchSettings.AsEnumerable()));

        _mockRepository
            .InsertNewLeaguesSettings(Arg.Any<List<LeagueSetting>>())
            .Returns(Task.FromException(new Exception("Insert error")));

        _mockLogger
            .Execute(Arg.Any<OwpLogCriteria>())
            .Returns(Task.CompletedTask);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GenerateNewLeagueSetting(date));
    }
}