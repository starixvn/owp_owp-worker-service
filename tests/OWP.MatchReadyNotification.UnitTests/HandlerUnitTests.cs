namespace OWP.MatchReadyNotification.UnitTests;

using Models;
using NSubstitute;
using Services;
using Xunit;

public class HandlerUnitTests
{
    private readonly MatchReadyRepository _matchReadyRepository;
    private readonly CacheManipulator _cacheManipulator;
    private readonly NotReadyMessageFactory _messageFactory;
    private readonly Handler _handler;

    public HandlerUnitTests()
    {
        _matchReadyRepository = Substitute.For<MatchReadyRepository>();
        _cacheManipulator = Substitute.For<CacheManipulator>();
        _messageFactory = Substitute.For<NotReadyMessageFactory>();
        _handler = new HandlerImp(_matchReadyRepository, _cacheManipulator, _messageFactory);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldExecuteAllStepsInCorrectOrder()
    {
        // Arrange
        var lastTimeNotReadyNotification = "2024-01-01 10:00:00";
        var snapshot = CreateTestSnapshot();
        var messageModels = CreateTestMessageModels();

        _cacheManipulator.GetLastTimeNotReadyNotification().Returns(lastTimeNotReadyNotification);
        _matchReadyRepository.GetNotReadyMatches(lastTimeNotReadyNotification).Returns(snapshot);
        _messageFactory.Create(snapshot).Returns(messageModels);

        // Act
        await _handler.Handle();

        // Assert
        await _cacheManipulator.Received(1).GetLastTimeNotReadyNotification();
        await _matchReadyRepository.Received(1).GetNotReadyMatches(lastTimeNotReadyNotification);
        await _cacheManipulator.Received(1).SaveLastTimeNotification(snapshot.LastDateTimeNotification);
        await _messageFactory.Received(1).Create(snapshot);
    }

    [Fact]
    public async Task Handle_WhenCacheReturnsEmptyString_ShouldStillProcess()
    {
        // Arrange
        var emptyLastTime = string.Empty;
        var snapshot = CreateTestSnapshot();
        var messageModels = CreateTestMessageModels();

        _cacheManipulator.GetLastTimeNotReadyNotification().Returns(emptyLastTime);
        _matchReadyRepository.GetNotReadyMatches(emptyLastTime).Returns(snapshot);
        _messageFactory.Create(snapshot).Returns(messageModels);

        // Act
        await _handler.Handle();

        // Assert
        await _matchReadyRepository.Received(1).GetNotReadyMatches(emptyLastTime);
        await _cacheManipulator.Received(1).SaveLastTimeNotification(snapshot.LastDateTimeNotification);
    }

    [Fact]
    public async Task Handle_WhenSnapshotIsEmpty_ShouldStillSaveLastTimeNotification()
    {
        // Arrange
        var lastTimeNotReadyNotification = "2024-01-01 10:00:00";
        var emptySnapshot = new AssignmentSnapshot
        {
            Matches = new Dictionary<int, MatchInfo>(),
            Assignments = new Dictionary<int, List<MatchAssignment>>(),
            LastDateTimeNotification = "2024-01-01 11:00:00"
        };
        var emptyMessageModels = new List<MessageModel>();

        _cacheManipulator.GetLastTimeNotReadyNotification().Returns(lastTimeNotReadyNotification);
        _matchReadyRepository.GetNotReadyMatches(lastTimeNotReadyNotification).Returns(emptySnapshot);
        _messageFactory.Create(emptySnapshot).Returns(emptyMessageModels);

        // Act
        await _handler.Handle();

        // Assert
        await _cacheManipulator.Received(1).SaveLastTimeNotification(emptySnapshot.LastDateTimeNotification);
        await _messageFactory.Received(1).Create(emptySnapshot);
    }

    [Fact]
    public async Task Handle_WhenExceptionOccurs_ShouldNotCatchException()
    {
        // Arrange
        _cacheManipulator.GetLastTimeNotReadyNotification().Returns(Task.FromException<string>(new InvalidOperationException("Cache error")));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await _handler.Handle());
    }

    private static AssignmentSnapshot CreateTestSnapshot()
    {
        var match = new MatchInfo
        {
            MatchId = 1234,
            MatchNo = "20000",
            Home = "Liverpool",
            Away = "Arsenal",
            LeagueName = "Premier League",
            KoTime = new TimeSpan(15, 30, 0)
        };

        var assignment = new MatchAssignment
        {
            MatchId = 1234,
            UserId = 1,
            UserName = "John Doe"
        };

        return new AssignmentSnapshot
        {
            Matches = new Dictionary<int, MatchInfo> { { 1234, match } },
            Assignments = new Dictionary<int, List<MatchAssignment>> { { 1234, new List<MatchAssignment> { assignment } } },
            LastDateTimeNotification = "2024-01-01 11:00:00"
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
                ReceivedUserIds = "1",
                ReceivedUserNames = "John Doe"
            }
        };
    }
}

