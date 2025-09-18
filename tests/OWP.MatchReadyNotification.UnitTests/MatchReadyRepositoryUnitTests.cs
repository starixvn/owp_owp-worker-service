namespace OWP.MatchReadyNotification.UnitTests;

using Criteria;
using Fanex.Data.Repository;
using Models;
using NSubstitute;
using Olympus.Xml;
using Xunit;

public class MatchReadyRepositoryUnitTests
{
    private readonly IDynamicRepository _repository;
    private readonly MatchReadyRepository _matchReadyRepository;

    public MatchReadyRepositoryUnitTests()
    {
        _repository = Substitute.For<IDynamicRepository>();
        _matchReadyRepository = new MatchReadyRepositoryImp(_repository);
    }

    [Fact]
    public async Task GetNotReadyMatches_WithValidData_ShouldReturnAssignmentSnapshot()
    {
        // Arrange
        var lastTimeNotReadyNotification = "2024-01-01 10:00:00";
        var lastTimeNotification = new DateTime(2024, 1, 1, 11, 0, 0);
        var assignments = CreateTestAssignments();
        var matches = CreateTestMatches();

        _repository.FetchMultipleAsync<DateTime, MatchAssignment>(Arg.Any<GetAllAssignmentsByXMinsCriteria>())
            .Returns((new List<DateTime> { lastTimeNotification }, assignments));
        _repository.FetchAsync<MatchInfo>(Arg.Any<GetAllMatchesByXMinsCriteria>())
            .Returns(matches);

        // Act
        var result = await _matchReadyRepository.GetNotReadyMatches(lastTimeNotReadyNotification);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("01/01/2024 11:00:00", result.LastDateTimeNotification);
        Assert.Equal(2, result.Matches.Count);
        Assert.Equal(2, result.Assignments.Count);
        Assert.Contains(result.Matches, kvp => kvp.Key == 1234);
        Assert.Contains(result.Matches, kvp => kvp.Key == 2345);
        Assert.Contains(result.Assignments, kvp => kvp.Key == 1234);
        Assert.Contains(result.Assignments, kvp => kvp.Key == 2345);
    }

    [Fact]
    public async Task GetNotReadyMatches_WithEmptyAssignments_ShouldReturnEmptySnapshot()
    {
        // Arrange
        var lastTimeNotReadyNotification = "2024-01-01 10:00:00";
        var lastTimeNotification = new DateTime(2024, 1, 1, 11, 0, 0);
        var emptyAssignments = new List<MatchAssignment>();
        var emptyMatches = new List<MatchInfo>();

        _repository.FetchMultipleAsync<DateTime, MatchAssignment>(Arg.Any<GetAllAssignmentsByXMinsCriteria>())
            .Returns((new List<DateTime> { lastTimeNotification }, emptyAssignments));
        _repository.FetchAsync<MatchInfo>(Arg.Any<GetAllMatchesByXMinsCriteria>())
            .Returns(emptyMatches);

        // Act
        var result = await _matchReadyRepository.GetNotReadyMatches(lastTimeNotReadyNotification);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("01/01/2024 11:00:00", result.LastDateTimeNotification);
        Assert.Empty(result.Matches);
        Assert.Empty(result.Assignments);
    }

    [Fact]
    public async Task GetNotReadyMatches_WithNoLastTimeNotification_ShouldHandleNullDateTime()
    {
        // Arrange
        var lastTimeNotReadyNotification = "2024-01-01 10:00:00";
        var assignments = CreateTestAssignments();
        var matches = CreateTestMatches();

        _repository.FetchMultipleAsync<DateTime, MatchAssignment>(Arg.Any<GetAllAssignmentsByXMinsCriteria>())
            .Returns((new List<DateTime>(), assignments));
        _repository.FetchAsync<MatchInfo>(Arg.Any<GetAllMatchesByXMinsCriteria>())
            .Returns(matches);

        // Act
        var result = await _matchReadyRepository.GetNotReadyMatches(lastTimeNotReadyNotification);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("01/01/0001 00:00:00", result.LastDateTimeNotification);
        Assert.Equal(2, result.Matches.Count);
        Assert.Equal(2, result.Assignments.Count);
    }

    [Fact]
    public async Task GetNotReadyMatches_WithGroupedAssignments_ShouldGroupByMatchId()
    {
        // Arrange
        var lastTimeNotReadyNotification = "2024-01-01 10:00:00";
        var lastTimeNotification = new DateTime(2024, 1, 1, 11, 0, 0);
        var assignments = CreateAssignmentsWithSameMatchId();
        var matches = CreateTestMatches();

        _repository.FetchMultipleAsync<DateTime, MatchAssignment>(Arg.Any<GetAllAssignmentsByXMinsCriteria>())
            .Returns((new List<DateTime> { lastTimeNotification }, assignments));
        _repository.FetchAsync<MatchInfo>(Arg.Any<GetAllMatchesByXMinsCriteria>())
            .Returns(matches);

        // Act
        var result = await _matchReadyRepository.GetNotReadyMatches(lastTimeNotReadyNotification);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Assignments);
        Assert.Equal(2, result.Assignments[1234].Count);
    }

    [Fact]
    public async Task GetLeaderSupervisorSectionHeadOfSession_WithValidUserIds_ShouldReturnManagers()
    {
        // Arrange
        var userIds = "1,2,3";
        var sessionManagers = CreateTestSessionManagers();

        _repository.FetchAsync<SessionManager>(Arg.Any<GetLeaderSupervisorSectionHeadOfSessionCriteria>())
            .Returns(sessionManagers);

        // Act
        var result = await _matchReadyRepository.GetLeaderSupervisorSectionHeadOfSession(userIds);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, kvp => kvp.Key == 1);
        Assert.Contains(result, kvp => kvp.Key == 2);
        Assert.Equal(2, result[1].Count);
        Assert.Single(result[2]);
    }

    [Fact]
    public async Task GetLeaderSupervisorSectionHeadOfSession_WithEmptyUserIds_ShouldReturnEmptyDictionary()
    {
        // Arrange
        var userIds = "";
        var emptySessionManagers = new List<SessionManager>();

        _repository.FetchAsync<SessionManager>(Arg.Any<GetLeaderSupervisorSectionHeadOfSessionCriteria>())
            .Returns(emptySessionManagers);

        // Act
        var result = await _matchReadyRepository.GetLeaderSupervisorSectionHeadOfSession(userIds);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetLeaderSupervisorSectionHeadOfSession_WithDuplicateManagers_ShouldReturnDistinctManagers()
    {
        // Arrange
        var userIds = "1";
        var sessionManagersWithDuplicates = CreateSessionManagersWithDuplicates();

        _repository.FetchAsync<SessionManager>(Arg.Any<GetLeaderSupervisorSectionHeadOfSessionCriteria>())
            .Returns(sessionManagersWithDuplicates);

        // Act
        var result = await _matchReadyRepository.GetLeaderSupervisorSectionHeadOfSession(userIds);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Single(result[1]); // Should be distinct
        Assert.Contains(result[1], a => a.AssigneeId == 11 && a.AssigneeName == "Manager 1");
    }

    [Fact]
    public async Task InsertMessage_WithValidMessages_ShouldCallRepositoryExecute()
    {
        // Arrange
        var messages = CreateTestMessageModels();
        var sessionId = "test-session-id";

        _repository.ExecuteAsync(Arg.Any<InsertMessageCriteria>())
            .Returns(Task.FromResult(1));

        // Act
        await _matchReadyRepository.InsertMessage(messages, sessionId);

        // Assert
        await _repository.Received(1).ExecuteAsync(Arg.Is<InsertMessageCriteria>(criteria =>
            criteria.SessionId == sessionId &&
            criteria.CreatedByUserId == 0 &&
            !string.IsNullOrEmpty(criteria.Messages)));
    }

    [Fact]
    public async Task InsertMessage_WithEmptyMessages_ShouldStillCallRepositoryExecute()
    {
        // Arrange
        var messages = new List<MessageModel>();
        var sessionId = "test-session-id";

        _repository.ExecuteAsync(Arg.Any<InsertMessageCriteria>())
            .Returns(Task.FromResult(1));

        // Act
        await _matchReadyRepository.InsertMessage(messages, sessionId);

        // Assert
        await _repository.Received(1).ExecuteAsync(Arg.Is<InsertMessageCriteria>(criteria =>
            criteria.SessionId == sessionId &&
            criteria.CreatedByUserId == 0));
    }

    [Fact]
    public async Task InsertMessage_WithNullSessionId_ShouldCallRepositoryExecute()
    {
        // Arrange
        var messages = CreateTestMessageModels();
        string sessionId = null;

        _repository.ExecuteAsync(Arg.Any<InsertMessageCriteria>())
            .Returns(Task.FromResult(1));

        // Act
        await _matchReadyRepository.InsertMessage(messages, sessionId);

        // Assert
        await _repository.Received(1).ExecuteAsync(Arg.Is<InsertMessageCriteria>(criteria =>
            criteria.SessionId == null &&
            criteria.CreatedByUserId == 0));
    }

    [Fact]
    public async Task InsertMessage_WhenRepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var messages = CreateTestMessageModels();
        var sessionId = "test-session-id";

        _repository.ExecuteAsync(Arg.Any<InsertMessageCriteria>())
            .Returns(Task.FromException<int>(new InvalidOperationException("Database error")));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _matchReadyRepository.InsertMessage(messages, sessionId));
    }

    private static List<MatchAssignment> CreateTestAssignments()
    {
        return new List<MatchAssignment>
        {
            new MatchAssignment
            {
                MatchId = 1234,
                UserId = 1,
                UserName = "John Doe"
            },
            new MatchAssignment
            {
                MatchId = 1234,
                UserId = 2,
                UserName = "Jane Smith"
            },
            new MatchAssignment
            {
                MatchId = 2345,
                UserId = 3,
                UserName = "Bob Johnson"
            }
        };
    }

    private static List<MatchAssignment> CreateAssignmentsWithSameMatchId()
    {
        return new List<MatchAssignment>
        {
            new MatchAssignment
            {
                MatchId = 1234,
                UserId = 1,
                UserName = "John Doe"
            },
            new MatchAssignment
            {
                MatchId = 1234,
                UserId = 2,
                UserName = "Jane Smith"
            }
        };
    }

    private static List<MatchInfo> CreateTestMatches()
    {
        return new List<MatchInfo>
        {
            new MatchInfo
            {
                MatchId = 1234,
                MatchNo = "20000",
                Home = "Liverpool",
                Away = "Arsenal",
                LeagueName = "Premier League",
                KoTime = new TimeSpan(15, 30, 0)
            },
            new MatchInfo
            {
                MatchId = 2345,
                MatchNo = "20001",
                Home = "Chelsea",
                Away = "Manchester City",
                LeagueName = "Premier League",
                KoTime = new TimeSpan(17, 45, 0)
            }
        };
    }

    private static List<SessionManager> CreateTestSessionManagers()
    {
        return new List<SessionManager>
        {
            new SessionManager
            {
                UserId = 1,
                ReceivedUserId = 11,
                ReceivedUserName = "Manager 1"
            },
            new SessionManager
            {
                UserId = 1,
                ReceivedUserId = 12,
                ReceivedUserName = "Manager 2"
            },
            new SessionManager
            {
                UserId = 2,
                ReceivedUserId = 21,
                ReceivedUserName = "Manager 3"
            }
        };
    }

    private static List<SessionManager> CreateSessionManagersWithDuplicates()
    {
        return new List<SessionManager>
        {
            new SessionManager
            {
                UserId = 1,
                ReceivedUserId = 11,
                ReceivedUserName = "Manager 1"
            },
            new SessionManager
            {
                UserId = 1,
                ReceivedUserId = 11,
                ReceivedUserName = "Manager 1"
            }
        };
    }

    private static List<MessageModel> CreateTestMessageModels()
    {
        return new List<MessageModel>
        {
            new MessageModel
            {
                MessageContent = "Test message content",
                IsImportant = true,
                IsWatcher = true,
                MessageType = 0,
                ReceivedUserIds = "1,2",
                ReceivedUserNames = "John Doe, Jane Smith"
            },
            new MessageModel
            {
                MessageContent = "Another test message",
                IsImportant = false,
                IsWatcher = false,
                MessageType = 1,
                ReceivedUserIds = "3",
                ReceivedUserNames = "Bob Johnson"
            }
        };
    }
}
