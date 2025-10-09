using NSubstitute;
using OWP.Constants;
using OWP.Provider.Share.AgentConfirm;
using OWP.Provider.Share.AgentConfirm.Model;
using OWP.Provider.Share.AgentConfirm.Services;

namespace OWP.Provider.Share.UnitTests.AgentConfirm;

public class SyncAgentConfirmHandlerTests
{
    private readonly Repository _mockRepository;
    private readonly SyncAgentConfirmHandlerImp _handler;

    public SyncAgentConfirmHandlerTests()
    {
        _mockRepository = Substitute.For<Repository>();
        _handler = new SyncAgentConfirmHandlerImp(_mockRepository);
    }

    [Fact]
    public async Task Handle_ShouldProcessMatchesAndSaveConfirms()
    {
        // Arrange
        var eventDate = new DateTime(2024, 1, 15);
        var agentMatches = new List<AgentMatchInfo>
        {
            new() { OWPMatchId = 1, MatchId = 100, MatchStatus = "Live", Provider = "365GV", IsSaba = false, InhouseProvider = "Betradar" },
            new() { OWPMatchId = 2, MatchId = 200, MatchStatus = "Cancel", Provider = "BG", IsSaba = true, InhouseProvider = "FC" }
        };

        _mockRepository
            .GetOwpMainMatchesByDate(eventDate)
            .Returns(agentMatches);

        _mockRepository
            .SaveAgentConfirm(Arg.Any<List<MatchAgentConfirm>>())
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(eventDate);

        // Assert
        _mockRepository.Received(1).GetOwpMainMatchesByDate(eventDate);
        _mockRepository.Received(1).SaveAgentConfirm(Arg.Any<List<MatchAgentConfirm>>());
    }

    [Fact]
    public async Task Handle_ShouldHandleEmptyMatchesList()
    {
        // Arrange
        var eventDate = new DateTime(2024, 1, 15);
        var emptyMatches = new List<AgentMatchInfo>();

        _mockRepository
            .GetOwpMainMatchesByDate(eventDate)
            .Returns(emptyMatches);

        _mockRepository
            .SaveAgentConfirm(Arg.Any<List<MatchAgentConfirm>>())
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(eventDate);

        // Assert
        _mockRepository.Received(1).GetOwpMainMatchesByDate(eventDate);
        _mockRepository.Received(1).SaveAgentConfirm(Arg.Is<List<MatchAgentConfirm>>(list => list.Count == 0));
    }

    [Theory]
    [InlineData(true, "cancel provider", "Cancel")]
    [InlineData(true, "CANCEL PROVIDER", "Cancel")]
    [InlineData(true, "Cancel Provider", "Cancel")]
    public async Task Handle_ShouldHandleSabaWithCancelledProvider(bool isSaba, string provider, string expectedAgent)
    {
        // Arrange
        var eventDate = new DateTime(2024, 1, 15);
        var agentMatches = new List<AgentMatchInfo>
        {
            new() { OWPMatchId = 1, MatchId = 100, MatchStatus = "Live", Provider = provider, IsSaba = isSaba, InhouseProvider = "Betradar" }
        };

        _mockRepository
            .GetOwpMainMatchesByDate(eventDate)
            .Returns(agentMatches);

        _mockRepository
            .SaveAgentConfirm(Arg.Any<List<MatchAgentConfirm>>())
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(eventDate);

        // Assert
        _mockRepository.Received(1).SaveAgentConfirm(
            Arg.Is<List<MatchAgentConfirm>>(list =>
                list.Count == 1 &&
                list[0].Agent == expectedAgent));
    }

    [Fact]
    public async Task Handle_ShouldHandleSabaWithNonCancelledProvider()
    {
        // Arrange
        var eventDate = new DateTime(2024, 1, 15);
        var agentMatches = new List<AgentMatchInfo>
        {
            new() { OWPMatchId = 1, MatchId = 100, MatchStatus = "Live", Provider = "365GV", IsSaba = true, InhouseProvider = "Betradar" }
        };

        _mockRepository
            .GetOwpMainMatchesByDate(eventDate)
            .Returns(agentMatches);

        _mockRepository
            .SaveAgentConfirm(Arg.Any<List<MatchAgentConfirm>>())
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(eventDate);

        // Assert
        _mockRepository.Received(1).SaveAgentConfirm(
            Arg.Is<List<MatchAgentConfirm>>(list =>
                list.Count == 1 &&
                list[0].Agent == "Streaming"));
    }

    [Fact]
    public async Task Handle_ShouldHandleCancelledMatch()
    {
        // Arrange
        var eventDate = new DateTime(2024, 1, 15);
        var agentMatches = new List<AgentMatchInfo>
        {
            new() { OWPMatchId = 1, MatchId = 100, MatchStatus = MatchStatus.Cancel, Provider = "365GV", IsSaba = false, InhouseProvider = "Betradar" }
        };

        _mockRepository
            .GetOwpMainMatchesByDate(eventDate)
            .Returns(agentMatches);

        _mockRepository
            .SaveAgentConfirm(Arg.Any<List<MatchAgentConfirm>>())
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(eventDate);

        // Assert
        _mockRepository.Received(1).SaveAgentConfirm(
            Arg.Is<List<MatchAgentConfirm>>(list =>
                list.Count == 1 &&
                list[0].Agent == "Cancel"));
    }

    [Theory]
    [InlineData(MatchStatus.Postponed, "Betradar", "Betradar")]
    [InlineData(MatchStatus.Suspended, "365GV", "365GV")]
    [InlineData(MatchStatus.Postponed, "BG", "BG")]
    public async Task Handle_ShouldHandlePostponedOrSuspendedWithInhouseProvider(string matchStatus, string inhouseProvider, string expectedAgent)
    {
        // Arrange
        var eventDate = new DateTime(2024, 1, 15);
        var agentMatches = new List<AgentMatchInfo>
        {
            new() { OWPMatchId = 1, MatchId = 100, MatchStatus = matchStatus, Provider = "365GV", IsSaba = false, InhouseProvider = inhouseProvider }
        };

        _mockRepository
            .GetOwpMainMatchesByDate(eventDate)
            .Returns(agentMatches);

        _mockRepository
            .SaveAgentConfirm(Arg.Any<List<MatchAgentConfirm>>())
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(eventDate);

        // Assert
        _mockRepository.Received(1).SaveAgentConfirm(
            Arg.Is<List<MatchAgentConfirm>>(list =>
                list.Count == 1 &&
                list[0].Agent == expectedAgent));
    }

    [Theory]
    [InlineData(MatchStatus.Postponed, "")]
    [InlineData(MatchStatus.Suspended, "")]
    [InlineData(MatchStatus.Postponed, "UnknownProvider")]
    public async Task Handle_ShouldHandlePostponedOrSuspendedWithoutValidInhouseProvider(string matchStatus, string inhouseProvider)
    {
        // Arrange
        var eventDate = new DateTime(2024, 1, 15);
        var agentMatches = new List<AgentMatchInfo>
        {
            new() { OWPMatchId = 1, MatchId = 100, MatchStatus = matchStatus, Provider = "365GV", IsSaba = false, InhouseProvider = inhouseProvider }
        };

        _mockRepository
            .GetOwpMainMatchesByDate(eventDate)
            .Returns(agentMatches);

        _mockRepository
            .SaveAgentConfirm(Arg.Any<List<MatchAgentConfirm>>())
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(eventDate);

        // Assert
        _mockRepository.Received(1).SaveAgentConfirm(
            Arg.Is<List<MatchAgentConfirm>>(list =>
                list.Count == 1 &&
                list[0].Agent == "Cancel"));
    }

    [Theory]
    [InlineData("365GV", "365GV")]
    [InlineData("BG", "BG")]
    [InlineData("FC", "FC")]
    [InlineData("IMG", "IMG")]
    [InlineData("RB2", "RB2")]
    [InlineData("RB3", "RB3")]
    [InlineData("RTS", "RTS")]
    [InlineData("SLR", "SLR")]
    [InlineData("WOO", "WOO")]
    public async Task Handle_ShouldHandleValidProvider(string provider, string expectedAgent)
    {
        // Arrange
        var eventDate = new DateTime(2024, 1, 15);
        var agentMatches = new List<AgentMatchInfo>
        {
            new() { OWPMatchId = 1, MatchId = 100, MatchStatus = "Live", Provider = provider, IsSaba = false, InhouseProvider = "Betradar" }
        };

        _mockRepository
            .GetOwpMainMatchesByDate(eventDate)
            .Returns(agentMatches);

        _mockRepository
            .SaveAgentConfirm(Arg.Any<List<MatchAgentConfirm>>())
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(eventDate);

        // Assert
        _mockRepository.Received(1).SaveAgentConfirm(
            Arg.Is<List<MatchAgentConfirm>>(list =>
                list.Count == 1 &&
                list[0].Agent == expectedAgent));
    }

    [Theory]
    [InlineData("BET", "Betradar")]
    [InlineData("bet", "Betradar")]
    [InlineData("Bet", "Betradar")]
    [InlineData("BETRADAR", "Betradar")]
    [InlineData("betradar", "Betradar")]
    public async Task Handle_ShouldHandleBetradarProvider(string provider, string expectedAgent)
    {
        // Arrange
        var eventDate = new DateTime(2024, 1, 15);
        var agentMatches = new List<AgentMatchInfo>
        {
            new() { OWPMatchId = 1, MatchId = 100, MatchStatus = "Live", Provider = provider, IsSaba = false, InhouseProvider = "Betradar" }
        };

        _mockRepository
            .GetOwpMainMatchesByDate(eventDate)
            .Returns(agentMatches);

        _mockRepository
            .SaveAgentConfirm(Arg.Any<List<MatchAgentConfirm>>())
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(eventDate);

        // Assert
        _mockRepository.Received(1).SaveAgentConfirm(
            Arg.Is<List<MatchAgentConfirm>>(list =>
                list.Count == 1 &&
                list[0].Agent == expectedAgent));
    }

    [Theory]
    [InlineData("BET365", "Streaming")]
    [InlineData("bet365", "Streaming")]
    [InlineData("TV", "Streaming")]
    [InlineData("tv", "Streaming")]
    [InlineData("BET365TV", "Streaming")]
    public async Task Handle_ShouldHandleStreamingProvider(string provider, string expectedAgent)
    {
        // Arrange
        var eventDate = new DateTime(2024, 1, 15);
        var agentMatches = new List<AgentMatchInfo>
        {
            new() { OWPMatchId = 1, MatchId = 100, MatchStatus = "Live", Provider = provider, IsSaba = false, InhouseProvider = "Betradar" }
        };

        _mockRepository
            .GetOwpMainMatchesByDate(eventDate)
            .Returns(agentMatches);

        _mockRepository
            .SaveAgentConfirm(Arg.Any<List<MatchAgentConfirm>>())
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(eventDate);

        // Assert
        _mockRepository.Received(1).SaveAgentConfirm(
            Arg.Is<List<MatchAgentConfirm>>(list =>
                list.Count == 1 &&
                list[0].Agent == expectedAgent));
    }

    [Fact]
    public async Task Handle_ShouldHandleEmptyProvider()
    {
        // Arrange
        var eventDate = new DateTime(2024, 1, 15);
        var agentMatches = new List<AgentMatchInfo>
        {
            new() { OWPMatchId = 1, MatchId = 100, MatchStatus = "Live", Provider = "", IsSaba = false, InhouseProvider = "Betradar" }
        };

        _mockRepository
            .GetOwpMainMatchesByDate(eventDate)
            .Returns(agentMatches);

        _mockRepository
            .SaveAgentConfirm(Arg.Any<List<MatchAgentConfirm>>())
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(eventDate);

        // Assert
        _mockRepository.Received(1).SaveAgentConfirm(
            Arg.Is<List<MatchAgentConfirm>>(list =>
                list.Count == 1 &&
                list[0].Agent == null));
    }

    [Fact]
    public async Task Handle_ShouldHandleNullProvider()
    {
        // Arrange
        var eventDate = new DateTime(2024, 1, 15);
        var agentMatches = new List<AgentMatchInfo>
        {
            new() { OWPMatchId = 1, MatchId = 100, MatchStatus = "Live", Provider = null, IsSaba = false, InhouseProvider = "Betradar" }
        };

        _mockRepository
            .GetOwpMainMatchesByDate(eventDate)
            .Returns(agentMatches);

        _mockRepository
            .SaveAgentConfirm(Arg.Any<List<MatchAgentConfirm>>())
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(eventDate);

        // Assert
        _mockRepository.Received(1).SaveAgentConfirm(
            Arg.Is<List<MatchAgentConfirm>>(list =>
                list.Count == 1 &&
                list[0].Agent == null));
    }

    [Fact]
    public async Task Handle_ShouldHandleMultipleMatches()
    {
        // Arrange
        var eventDate = new DateTime(2024, 1, 15);
        var agentMatches = new List<AgentMatchInfo>
        {
            new() { OWPMatchId = 1, MatchId = 100, MatchStatus = "Live", Provider = "365GV", IsSaba = false, InhouseProvider = "Betradar" },
            new() { OWPMatchId = 2, MatchId = 200, MatchStatus = MatchStatus.Cancel, Provider = "BG", IsSaba = false, InhouseProvider = "FC" },
            new() { OWPMatchId = 3, MatchId = 300, MatchStatus = "Live", Provider = "BET", IsSaba = false, InhouseProvider = "Betradar" }
        };

        _mockRepository
            .GetOwpMainMatchesByDate(eventDate)
            .Returns(agentMatches);

        _mockRepository
            .SaveAgentConfirm(Arg.Any<List<MatchAgentConfirm>>())
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(eventDate);

        // Assert
        _mockRepository.Received(1).SaveAgentConfirm(
            Arg.Is<List<MatchAgentConfirm>>(list =>
                list.Count == 3 &&
                list.Any(c => c.Agent == "365GV") &&
                list.Any(c => c.Agent == "Cancel") &&
                list.Any(c => c.Agent == "Betradar")));
    }
}