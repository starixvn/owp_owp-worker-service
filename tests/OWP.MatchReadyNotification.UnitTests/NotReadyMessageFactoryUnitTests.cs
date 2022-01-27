using NSubstitute;
using OWP.MatchReadyNotification.Models;
using OWP.MatchReadyNotification.Services;
using Xunit;
using Xunit.Asserts.Compare;

namespace OWP.MatchReadyNotification.UnitTests;

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
        var allSessionLST = new List<Assignee>
            {
                new Assignee
                {
                    AssigneeId = 123,
                    AssigneeName = "Kevin Durrant"
                },

                 new Assignee
                {
                    AssigneeId = 234,
                    AssigneeName = "Kyrie Irving"
                }
            };

        queryRepository.GetLeaderSupervisorSectionHeadOfSession(Arg.Compat.Any<string>()).Returns(leadersByUserId);
        queryRepository.GetAllSessionLST().Returns(allSessionLST);
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

    private Dictionary<int, List<Assignee>> GetLeadersOfSession()
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

    private AssignmentSnapshot DataInput()
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
            Home = "LiverPool-Coners",
            Away = "Arsenal-Coners",
            ParentMatchId = 3456,
            LeagueName = "PremierLeague-Coners"
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

    private Dictionary<int, List<MatchAssignment>> GetAssignment()
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

    private List<MessageModel> MessagesExpected()
    {
        var messageModel = new MessageModel
        {
            IsImportant = true,
            IsWatcher = true,
            MessageType = 0,
            MessageContent = "Esther, Rylie are NOT READY for match #20000 - PremierLeague - LiverPool -vs- Arsenal - K.O Time 12:00 AM",
            ReceivedUserIds = "11,12,123,234,22,23",
            ReceivedUserNames = "Steve Kerr,James,Kevin Durrant,Kyrie Irving,Jordan,Klay Thompson"
        };

        var messageModel2 = new MessageModel
        {
            IsImportant = true,
            IsWatcher = true,
            MessageType = 0,
            MessageContent = "Curry are NOT READY for match #1234 - PremierLeague-Coners - LiverPool-Coners -vs- Arsenal-Coners - K.O Time 12:00 AM",
            ReceivedUserIds = "33,123,234",
            ReceivedUserNames = "Giannis,Kevin Durrant,Kyrie Irving"
        };

        var messages = new List<MessageModel> { messageModel, messageModel2 };
        return messages;
    }
}