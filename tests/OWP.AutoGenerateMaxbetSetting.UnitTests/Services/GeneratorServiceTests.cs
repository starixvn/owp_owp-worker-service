using Moq;
using OWP.AutoGenerateMaxbetSetting;
using OWP.AutoGenerateMaxbetSetting.Models;
using OWP.AutoGenerateMaxbetSetting.Services;
using Olympus.OWP.Log.Data.Repository.Criteria;
using Olympus.OWP.Log.Data.Repository.Services;
using Match = OWP.AutoGenerateMaxbetSetting.Models.Match;

namespace OWP.AutoGenerateMaxbetSetting.UnitTests.Services;

public class GeneratorServiceTests
{
    private readonly Mock<LeagueSettingsRepository> _mockRepository;
    private readonly Mock<LogWriterService> _mockLogger;
    private readonly GeneratorServiceImp _service;

    public GeneratorServiceTests()
    {
        _mockRepository = new Mock<LeagueSettingsRepository>();
        _mockLogger = new Mock<LogWriterService>();
        _service = new GeneratorServiceImp(_mockRepository.Object, _mockLogger.Object);
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
            .Setup(x => x.GetMatchesAndSettings(date))
            .ReturnsAsync(new Tuple<IEnumerable<Match>, IEnumerable<MatchSetting>>(matches.AsEnumerable(), matchSettings.AsEnumerable()));

        _mockRepository
            .Setup(x => x.InsertNewLeaguesSettings(It.IsAny<List<LeagueSetting>>()))
            .Returns(Task.CompletedTask);

        _mockLogger
            .Setup(x => x.Execute(It.IsAny<OwpLogCriteria>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.GenerateNewLeagueSetting(date);

        // Assert
        _mockRepository.Verify(x => x.GetMatchesAndSettings(date), Times.Once);
        _mockRepository.Verify(x => x.InsertNewLeaguesSettings(It.IsAny<List<LeagueSetting>>()), Times.Once);
        _mockLogger.Verify(x => x.Execute(It.IsAny<OwpLogCriteria>()), Times.Once);
    }

    [Fact]
    public async Task GenerateNewLeagueSetting_ShouldHandleEmptyMatches()
    {
        // Arrange
        var date = new DateTime(2024, 1, 15);
        var emptyMatches = new List<Match>();
        var emptySettings = new List<MatchSetting>();

        _mockRepository
            .Setup(x => x.GetMatchesAndSettings(date))
            .ReturnsAsync(new Tuple<IEnumerable<Match>, IEnumerable<MatchSetting>>(emptyMatches.AsEnumerable(), emptySettings.AsEnumerable()));

        _mockRepository
            .Setup(x => x.InsertNewLeaguesSettings(It.IsAny<List<LeagueSetting>>()))
            .Returns(Task.CompletedTask);

        _mockLogger
            .Setup(x => x.Execute(It.IsAny<OwpLogCriteria>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.GenerateNewLeagueSetting(date);

        // Assert
        _mockRepository.Verify(x => x.GetMatchesAndSettings(date), Times.Once);
        _mockRepository.Verify(x => x.InsertNewLeaguesSettings(It.IsAny<List<LeagueSetting>>()), Times.Once);
        _mockLogger.Verify(x => x.Execute(It.IsAny<OwpLogCriteria>()), Times.Once);
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
            .Setup(x => x.GetMatchesAndSettings(date))
            .ReturnsAsync(new Tuple<IEnumerable<Match>, IEnumerable<MatchSetting>>(matches.AsEnumerable(), matchSettings.AsEnumerable()));

        _mockRepository
            .Setup(x => x.InsertNewLeaguesSettings(It.IsAny<List<LeagueSetting>>()))
            .Returns(Task.CompletedTask);

        _mockLogger
            .Setup(x => x.Execute(It.IsAny<OwpLogCriteria>()))
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
            .Setup(x => x.GetMatchesAndSettings(date))
            .ReturnsAsync(new Tuple<IEnumerable<Match>, IEnumerable<MatchSetting>>(matches.AsEnumerable(), matchSettings.AsEnumerable()));

        _mockRepository
            .Setup(x => x.InsertNewLeaguesSettings(It.IsAny<List<LeagueSetting>>()))
            .Returns(Task.CompletedTask);

        _mockLogger
            .Setup(x => x.Execute(It.IsAny<OwpLogCriteria>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.GenerateNewLeagueSetting(date);

        // Assert
        _mockLogger.Verify(x => x.Execute(
            It.Is<OwpLogCriteria>(c =>
                c.Action == "Generator" &&
                !string.IsNullOrEmpty(c.Payload) &&
                c.CreatedDate.Date == DateTime.Now.AddHours(12).Date)), Times.Once);
    }

    [Fact]
    public async Task GenerateNewLeagueSetting_ShouldHandleRepositoryExceptions()
    {
        // Arrange
        var date = new DateTime(2024, 1, 15);

        _mockRepository
            .Setup(x => x.GetMatchesAndSettings(date))
            .ThrowsAsync(new Exception("Database error"));

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
            .Setup(x => x.GetMatchesAndSettings(date))
            .ReturnsAsync(new Tuple<IEnumerable<Match>, IEnumerable<MatchSetting>>(matches.AsEnumerable(), matchSettings.AsEnumerable()));

        _mockRepository
            .Setup(x => x.InsertNewLeaguesSettings(It.IsAny<List<LeagueSetting>>()))
            .Returns(Task.CompletedTask);

        _mockLogger
            .Setup(x => x.Execute(It.IsAny<OwpLogCriteria>()))
            .ThrowsAsync(new Exception("Logging error"));

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
            .Setup(x => x.GetMatchesAndSettings(date))
            .ReturnsAsync(new Tuple<IEnumerable<Match>, IEnumerable<MatchSetting>>(matches.AsEnumerable(), matchSettings.AsEnumerable()));

        _mockRepository
            .Setup(x => x.InsertNewLeaguesSettings(It.IsAny<List<LeagueSetting>>()))
            .ThrowsAsync(new Exception("Insert error"));

        _mockLogger
            .Setup(x => x.Execute(It.IsAny<OwpLogCriteria>()))
            .Returns(Task.CompletedTask);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GenerateNewLeagueSetting(date));
    }
}
