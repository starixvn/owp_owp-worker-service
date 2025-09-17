namespace OWP.MatchReadyNotification.UnitTests;

using Models;
using NSubstitute;
using Services;
using Xunit;
using Xunit.Asserts.Compare;

public class NotReadyMessageFactoryUnitTests
{
    private readonly MatchReadyRepository queryRepository;

    private readonly NotReadyMessageFactory messageFactory;

    public NotReadyMessageFactoryUnitTests()
    {
        queryRepository = Substitute.For<MatchReadyRepository>();

        var receivedUserHelper = new ReceivedUserServicesImp(queryRepository);
        messageFactory = new NotReadyMessageFactoryImp(receivedUserHelper);
    }

    [Fact]
    public async Task Create_Always_ReturnMessages()
    {
        var snapshot = DataInput();
        var leadersByUserId = GetLeadersOfSession();

        queryRepository.GetLeaderSupervisorSectionHeadOfSession(Arg.Compat.Any<string>()).Returns(leadersByUserId);
        var expected = MessagesExpected();

        var actual = await messageFactory.Create(snapshot);
        DeepAssert.Equal(expected[0], actual[0]);
        DeepAssert.Equal(expected[1], actual[1]);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData(null, "")]
    [InlineData("123", "#123")]
    public void GetFormattedMatchNo(string matchNo, string expected)
    {
        var actual = NotReadyMessageFactoryImp.GetFormattedMatchNo(matchNo);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task Create_WithEmptySnapshot_ShouldReturnEmptyMessageList()
    {
        // Arrange
        var emptySnapshot = new AssignmentSnapshot
        {
            Matches = new Dictionary<int, MatchInfo>(),
            Assignments = new Dictionary<int, List<MatchAssignment>>()
        };

        queryRepository.GetLeaderSupervisorSectionHeadOfSession(Arg.Compat.Any<string>()).Returns(new Dictionary<int, List<Assignee>>());

        // Act
        var result = await messageFactory.Create(emptySnapshot);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Create_WithNullAssignees_ShouldHandleGracefully()
    {
        // Arrange
        var snapshot = new AssignmentSnapshot
        {
            Matches = new Dictionary<int, MatchInfo>
            {
                { 1234, new MatchInfo { MatchId = 1234, MatchNo = "20000", Home = "Liverpool", Away = "Arsenal", LeagueName = "Premier League" } }
            },
            Assignments = new Dictionary<int, List<MatchAssignment>>
            {
                { 1234, new List<MatchAssignment>() }
            }
        };

        queryRepository.GetLeaderSupervisorSectionHeadOfSession(Arg.Compat.Any<string>()).Returns(new Dictionary<int, List<Assignee>>());

        // Act
        var result = await messageFactory.Create(snapshot);

        // Assert
        Assert.Single(result);
        Assert.Contains("are NOT READY", result[0].MessageContent);
    }

    [Fact]
    public async Task Create_WithSingleAssignee_ShouldCreateCorrectMessage()
    {
        // Arrange
        var snapshot = new AssignmentSnapshot
        {
            Matches = new Dictionary<int, MatchInfo>
            {
                { 1234, new MatchInfo
                {
                    MatchId = 1234,
                    MatchNo = "20000",
                    Home = "Liverpool",
                    Away = "Arsenal",
                    LeagueName = "Premier League",
                    KoTime = new TimeSpan(15, 30, 0)
                } }
            },
            Assignments = new Dictionary<int, List<MatchAssignment>>
            {
                { 1234, new List<MatchAssignment>
                {
                    new MatchAssignment { MatchId = 1234, UserId = 1, UserName = "John Doe" }
                } }
            }
        };

        var leaders = new Dictionary<int, List<Assignee>>
        {
            { 1, new List<Assignee> { new Assignee { AssigneeId = 11, AssigneeName = "Manager 1" } } }
        };

        queryRepository.GetLeaderSupervisorSectionHeadOfSession(Arg.Compat.Any<string>()).Returns(leaders);

        // Act
        var result = await messageFactory.Create(snapshot);

        // Assert
        Assert.Single(result);
        var message = result[0];
        Assert.True(message.IsImportant);
        Assert.True(message.IsWatcher);
        Assert.Equal(0, message.MessageType);
        Assert.Contains("John Doe are NOT READY", message.MessageContent);
        Assert.Contains("#20000", message.MessageContent);
        Assert.Contains("Premier League", message.MessageContent);
        Assert.Contains("Liverpool -vs- Arsenal", message.MessageContent);
        Assert.Contains("03:30 PM", message.MessageContent);
        Assert.Equal("11", message.ReceivedUserIds);
        Assert.Equal("Manager 1", message.ReceivedUserNames);
    }

    [Fact]
    public async Task Create_WithMultipleAssignees_ShouldCreateCorrectMessage()
    {
        // Arrange
        var snapshot = new AssignmentSnapshot
        {
            Matches = new Dictionary<int, MatchInfo>
            {
                { 1234, new MatchInfo
                {
                    MatchId = 1234,
                    MatchNo = "20000",
                    Home = "Liverpool",
                    Away = "Arsenal",
                    LeagueName = "Premier League",
                    KoTime = new TimeSpan(20, 45, 0)
                } }
            },
            Assignments = new Dictionary<int, List<MatchAssignment>>
            {
                { 1234, new List<MatchAssignment>
                {
                    new MatchAssignment { MatchId = 1234, UserId = 1, UserName = "John Doe" },
                    new MatchAssignment { MatchId = 1234, UserId = 2, UserName = "Jane Smith" }
                } }
            }
        };

        var leaders = new Dictionary<int, List<Assignee>>
        {
            { 1, new List<Assignee> { new Assignee { AssigneeId = 11, AssigneeName = "Manager 1" } } },
            { 2, new List<Assignee> { new Assignee { AssigneeId = 22, AssigneeName = "Manager 2" } } }
        };

        queryRepository.GetLeaderSupervisorSectionHeadOfSession(Arg.Compat.Any<string>()).Returns(leaders);

        // Act
        var result = await messageFactory.Create(snapshot);

        // Assert
        Assert.Single(result);
        var message = result[0];
        Assert.Contains("John Doe, Jane Smith are NOT READY", message.MessageContent);
        Assert.Contains("08:45 PM", message.MessageContent);
        Assert.Equal("11,22", message.ReceivedUserIds);
        Assert.Equal("Manager 1,Manager 2", message.ReceivedUserNames);
    }

    [Fact]
    public async Task Create_WithEmptyMatchNo_ShouldHandleCorrectly()
    {
        // Arrange
        var snapshot = new AssignmentSnapshot
        {
            Matches = new Dictionary<int, MatchInfo>
            {
                { 1234, new MatchInfo
                {
                    MatchId = 1234,
                    MatchNo = "",
                    Home = "Liverpool",
                    Away = "Arsenal",
                    LeagueName = "Premier League",
                    KoTime = new TimeSpan(15, 30, 0)
                } }
            },
            Assignments = new Dictionary<int, List<MatchAssignment>>
            {
                { 1234, new List<MatchAssignment>
                {
                    new MatchAssignment { MatchId = 1234, UserId = 1, UserName = "John Doe" }
                } }
            }
        };

        queryRepository.GetLeaderSupervisorSectionHeadOfSession(Arg.Compat.Any<string>()).Returns(new Dictionary<int, List<Assignee>>());

        // Act
        var result = await messageFactory.Create(snapshot);

        // Assert
        Assert.Single(result);
        var message = result[0];
        Assert.Contains("John Doe are NOT READY for match  - Premier League", message.MessageContent);
        Assert.DoesNotContain("#", message.MessageContent);
    }

    [Fact]
    public async Task Create_WithNullMatchNo_ShouldHandleCorrectly()
    {
        // Arrange
        var snapshot = new AssignmentSnapshot
        {
            Matches = new Dictionary<int, MatchInfo>
            {
                { 1234, new MatchInfo
                {
                    MatchId = 1234,
                    MatchNo = null,
                    Home = "Liverpool",
                    Away = "Arsenal",
                    LeagueName = "Premier League",
                    KoTime = new TimeSpan(15, 30, 0)
                } }
            },
            Assignments = new Dictionary<int, List<MatchAssignment>>
            {
                { 1234, new List<MatchAssignment>
                {
                    new MatchAssignment { MatchId = 1234, UserId = 1, UserName = "John Doe" }
                } }
            }
        };

        queryRepository.GetLeaderSupervisorSectionHeadOfSession(Arg.Compat.Any<string>()).Returns(new Dictionary<int, List<Assignee>>());

        // Act
        var result = await messageFactory.Create(snapshot);

        // Assert
        Assert.Single(result);
        var message = result[0];
        Assert.Contains("John Doe are NOT READY for match  - Premier League", message.MessageContent);
        Assert.DoesNotContain("#", message.MessageContent);
    }

    [Fact]
    public void Transform_WithValidData_ShouldReturnCorrectMessage()
    {
        // Arrange
        var model = new NotReadyMatchInfo
        {
            MatchNo = "20000",
            LeagueName = "Premier League",
            Event = "Liverpool -vs- Arsenal",
            KoTime = new TimeSpan(15, 30, 0),
            Assignees = new List<Assignee>
            {
                new Assignee { AssigneeId = 1, AssigneeName = "John Doe" },
                new Assignee { AssigneeId = 2, AssigneeName = "Jane Smith" }
            }
        };

        // Act
        var result = NotReadyMessageFactoryImp.Transform(model);

        // Assert
        Assert.Equal("John Doe, Jane Smith are NOT READY for match #20000 - Premier League - Liverpool -vs- Arsenal - K.O Time 03:30 PM", result);
    }

    [Fact]
    public void Transform_WithEmptyAssignees_ShouldReturnCorrectMessage()
    {
        // Arrange
        var model = new NotReadyMatchInfo
        {
            MatchNo = "20000",
            LeagueName = "Premier League",
            Event = "Liverpool -vs- Arsenal",
            KoTime = new TimeSpan(15, 30, 0),
            Assignees = new List<Assignee>()
        };

        // Act
        var result = NotReadyMessageFactoryImp.Transform(model);

        // Assert
        Assert.Equal(" are NOT READY for match #20000 - Premier League - Liverpool -vs- Arsenal - K.O Time 03:30 PM", result);
    }

    [Theory]
    [InlineData(0, 0, "12:00 AM")]
    [InlineData(12, 0, "12:00 PM")]
    [InlineData(15, 30, "03:30 PM")]
    [InlineData(23, 59, "11:59 PM")]
    public void Transform_WithDifferentKoTimes_ShouldFormatCorrectly(int hours, int minutes, string expected)
    {
        // Arrange
        var model = new NotReadyMatchInfo
        {
            MatchNo = "20000",
            LeagueName = "Premier League",
            Event = "Liverpool -vs- Arsenal",
            KoTime = new TimeSpan(hours, minutes, 0),
            Assignees = new List<Assignee>
            {
                new Assignee { AssigneeId = 1, AssigneeName = "John Doe" }
            }
        };

        // Act
        var result = NotReadyMessageFactoryImp.Transform(model);

        // Assert
        Assert.Contains(expected, result);
    }

    private static Dictionary<int, List<Assignee>> GetLeadersOfSession()
    {
        var leader = new Assignee
        {
            AssigneeId = 11,
            AssigneeName = "Steve Kerr"
        };

        var leader2 = new Assignee
        {
            AssigneeId = 12,
            AssigneeName = "James"
        };

        var leader3 = new Assignee
        {
            AssigneeId = 22,
            AssigneeName = "Jordan"
        };

        var leader4 = new Assignee
        {
            AssigneeId = 23,
            AssigneeName = "Klay Thompson"
        };

        var leader5 = new Assignee
        {
            AssigneeId = 33,
            AssigneeName = "Giannis"
        };

        var leaders = new List<Assignee> { leader, leader2 };
        var leaders2 = new List<Assignee> { leader3, leader4 };
        var leaders3 = new List<Assignee> { leader5 };

        var leadersByUserId = new Dictionary<int, List<Assignee>> { { 1, leaders }, { 2, leaders2 }, { 3, leaders3 } };
        return leadersByUserId;
    }

    private static AssignmentSnapshot DataInput()
    {
        var match = new MatchInfo
        {
            MatchId = 1234,
            MatchNo = "20000",
            Home = "LiverPool",
            Away = "Arsenal",
            LeagueName = "PremierLeague"
        };

        var match2 = new MatchInfo
        {
            MatchId = 2345,
            MatchNo = "1234",
            Home = "LiverPool-Corners",
            Away = "Arsenal-Corners",
            ParentMatchId = 3456,
            LeagueName = "PremierLeague-Corners"
        };

        var matches = new Dictionary<int, MatchInfo> { { 1234, match }, { 2345, match2 } };

        var assignments = GetAssignment();
        var snapshot = new AssignmentSnapshot
        {
            Matches = matches,
            Assignments = assignments
        };

        return snapshot;
    }

    private static Dictionary<int, List<MatchAssignment>> GetAssignment()
    {
        var assignment = new MatchAssignment
        {
            MatchId = 1234,
            UserId = 1,
            UserName = "Esther"
        };

        var assignment2 = new MatchAssignment
        {
            MatchId = 1234,
            UserId = 2,
            UserName = "Rylie"
        };

        var assignment3 = new MatchAssignment
        {
            MatchId = 2345,
            UserId = 3,
            UserName = "Curry"
        };

        var assignments = new List<MatchAssignment> { assignment, assignment2 };
        var assignments2 = new List<MatchAssignment> { assignment3 };

        var assignmentByMatchId = new Dictionary<int, List<MatchAssignment>> { { 1234, assignments }, { 2345, assignments2 } };
        return assignmentByMatchId;
    }

    private static List<MessageModel> MessagesExpected()
    {
        var messageModel = new MessageModel
        {
            IsImportant = true,
            IsWatcher = true,
            MessageType = 0,
            MessageContent = "Esther, Rylie are NOT READY for match #20000 - PremierLeague - LiverPool -vs- Arsenal - K.O Time 12:00 AM",
            ReceivedUserIds = "11,12,22,23",
            ReceivedUserNames = "Steve Kerr,James,Jordan,Klay Thompson"
        };

        var messageModel2 = new MessageModel
        {
            IsImportant = true,
            IsWatcher = true,
            MessageType = 0,
            MessageContent = "Curry are NOT READY for match #1234 - PremierLeague-Corners - LiverPool-Corners -vs- Arsenal-Corners - K.O Time 12:00 AM",
            ReceivedUserIds = "33",
            ReceivedUserNames = "Giannis"
        };

        var messages = new List<MessageModel> { messageModel, messageModel2 };
        return messages;
    }
}