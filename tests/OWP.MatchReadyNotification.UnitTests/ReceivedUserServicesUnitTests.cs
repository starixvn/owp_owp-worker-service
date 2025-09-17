namespace OWP.MatchReadyNotification.UnitTests;

using Models;
using NSubstitute;
using Services;
using Xunit;

public class ReceivedUserServicesUnitTests
{
    private readonly MatchReadyRepository _matchReadyRepository;
    private readonly ReceivedUserServices _receivedUserServices;

    public ReceivedUserServicesUnitTests()
    {
        _matchReadyRepository = Substitute.For<MatchReadyRepository>();
        _receivedUserServices = new ReceivedUserServicesImp(_matchReadyRepository);
    }

    [Fact]
    public void GetAllAssignees_WithValidSnapshot_ShouldReturnDistinctAssignees()
    {
        // Arrange
        var snapshot = CreateTestSnapshot();

        // Act
        var result = _receivedUserServices.GetAllAssignees(snapshot);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Contains(result, a => a.AssigneeId == 1 && a.AssigneeName == "John Doe");
        Assert.Contains(result, a => a.AssigneeId == 2 && a.AssigneeName == "Jane Smith");
        Assert.Contains(result, a => a.AssigneeId == 3 && a.AssigneeName == "Bob Johnson");
    }

    [Fact]
    public void GetAllAssignees_WithEmptySnapshot_ShouldReturnEmptyList()
    {
        // Arrange
        var snapshot = new AssignmentSnapshot
        {
            Matches = new Dictionary<int, MatchInfo>(),
            Assignments = new Dictionary<int, List<MatchAssignment>>()
        };

        // Act
        var result = _receivedUserServices.GetAllAssignees(snapshot);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetAllAssignees_WithDuplicateAssignees_ShouldReturnDistinctAssignees()
    {
        // Arrange
        var snapshot = CreateSnapshotWithDuplicateAssignees();

        // Act
        var result = _receivedUserServices.GetAllAssignees(snapshot);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, a => a.AssigneeId == 1 && a.AssigneeName == "John Doe");
        Assert.Contains(result, a => a.AssigneeId == 2 && a.AssigneeName == "Jane Smith");
    }

    [Fact]
    public void GetAssigneesOfMatch_WithValidMatchId_ShouldReturnAssigneesForMatch()
    {
        // Arrange
        var snapshot = CreateTestSnapshot();
        var matchId = 1234;

        // Act
        var result = _receivedUserServices.GetAssigneesOfMatch(snapshot, matchId);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, a => a.AssigneeId == 1 && a.AssigneeName == "John Doe");
        Assert.Contains(result, a => a.AssigneeId == 2 && a.AssigneeName == "Jane Smith");
    }

    [Fact]
    public void GetAssigneesOfMatch_WithNonExistentMatchId_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var snapshot = CreateTestSnapshot();
        var nonExistentMatchId = 9999;

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() =>
            _receivedUserServices.GetAssigneesOfMatch(snapshot, nonExistentMatchId));
    }

    [Fact]
    public void GetAssigneesOfMatch_WithZeroUserId_ShouldFilterOutZeroUserId()
    {
        // Arrange
        var snapshot = CreateSnapshotWithZeroUserId();

        // Act
        var result = _receivedUserServices.GetAssigneesOfMatch(snapshot, 1234);

        // Assert
        Assert.Single(result);
        Assert.Contains(result, a => a.AssigneeId == 1 && a.AssigneeName == "John Doe");
    }

    [Fact]
    public async Task GetAllReceivedUsers_WithValidAssignees_ShouldReturnManagers()
    {
        // Arrange
        var assignees = new List<Assignee>
        {
            new Assignee { AssigneeId = 1, AssigneeName = "John Doe" },
            new Assignee { AssigneeId = 2, AssigneeName = "Jane Smith" }
        };

        var expectedManagers = new Dictionary<int, List<Assignee>>
        {
            { 1, new List<Assignee> { new Assignee { AssigneeId = 11, AssigneeName = "Manager 1" } } },
            { 2, new List<Assignee> { new Assignee { AssigneeId = 22, AssigneeName = "Manager 2" } } }
        };

        _matchReadyRepository.GetLeaderSupervisorSectionHeadOfSession("1,2")
            .Returns(expectedManagers);

        // Act
        var result = await _receivedUserServices.GetAllReceivedUsers(assignees);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, kvp => kvp.Key == 1);
        Assert.Contains(result, kvp => kvp.Key == 2);
        await _matchReadyRepository.Received(1).GetLeaderSupervisorSectionHeadOfSession("1,2");
    }

    [Fact]
    public async Task GetAllReceivedUsers_WithEmptyAssignees_ShouldReturnEmptyDictionary()
    {
        // Arrange
        var assignees = new List<Assignee>();
        var expectedManagers = new Dictionary<int, List<Assignee>>();

        _matchReadyRepository.GetLeaderSupervisorSectionHeadOfSession("")
            .Returns(expectedManagers);

        // Act
        var result = await _receivedUserServices.GetAllReceivedUsers(assignees);

        // Assert
        Assert.Empty(result);
        await _matchReadyRepository.Received(1).GetLeaderSupervisorSectionHeadOfSession("");
    }

    [Fact]
    public async Task GetAllReceivedUsers_WithDuplicateManagers_ShouldReturnDistinctManagers()
    {
        // Arrange
        var assignees = new List<Assignee>
        {
            new Assignee { AssigneeId = 1, AssigneeName = "John Doe" }
        };

        var managersWithDuplicates = new Dictionary<int, List<Assignee>>
        {
            { 1, new List<Assignee>
                {
                    new Assignee { AssigneeId = 11, AssigneeName = "Manager 1" },
                    new Assignee { AssigneeId = 11, AssigneeName = "Manager 1" } // Duplicate
                }
            }
        };

        _matchReadyRepository.GetLeaderSupervisorSectionHeadOfSession("1")
            .Returns(managersWithDuplicates);

        // Act
        var result = await _receivedUserServices.GetAllReceivedUsers(assignees);

        // Assert
        Assert.Single(result[1]);
        Assert.Contains(result[1], m => m.AssigneeId == 11 && m.AssigneeName == "Manager 1");
    }

    [Fact]
    public void GetReceivedUsersOfMatch_WithValidData_ShouldReturnReceivedUsers()
    {
        // Arrange
        var assignees = new List<Assignee>
        {
            new Assignee { AssigneeId = 1, AssigneeName = "John Doe" },
            new Assignee { AssigneeId = 2, AssigneeName = "Jane Smith" }
        };

        var allReceivedUsers = new Dictionary<int, List<Assignee>>
        {
            { 1, new List<Assignee> { new Assignee { AssigneeId = 11, AssigneeName = "Manager 1" } } },
            { 2, new List<Assignee> { new Assignee { AssigneeId = 22, AssigneeName = "Manager 2" } } }
        };

        // Act
        var result = _receivedUserServices.GetReceivedUsersOfMatch(assignees, allReceivedUsers);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, u => u.AssigneeId == 11 && u.AssigneeName == "Manager 1");
        Assert.Contains(result, u => u.AssigneeId == 22 && u.AssigneeName == "Manager 2");
    }

    [Fact]
    public void GetReceivedUsersOfMatch_WithEmptyAssignees_ShouldReturnEmptyList()
    {
        // Arrange
        var assignees = new List<Assignee>();
        var allReceivedUsers = new Dictionary<int, List<Assignee>>();

        // Act
        var result = _receivedUserServices.GetReceivedUsersOfMatch(assignees, allReceivedUsers);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetReceivedUsersOfMatch_WithMissingReceivedUsers_ShouldReturnEmptyList()
    {
        // Arrange
        var assignees = new List<Assignee>
        {
            new Assignee { AssigneeId = 999, AssigneeName = "Non-existent" }
        };

        var allReceivedUsers = new Dictionary<int, List<Assignee>>();

        // Act
        var result = _receivedUserServices.GetReceivedUsersOfMatch(assignees, allReceivedUsers);

        // Assert
        Assert.Empty(result);
    }

    private static AssignmentSnapshot CreateTestSnapshot()
    {
        var match1 = new MatchInfo
        {
            MatchId = 1234,
            MatchNo = "20000",
            Home = "Liverpool",
            Away = "Arsenal",
            LeagueName = "Premier League"
        };

        var match2 = new MatchInfo
        {
            MatchId = 2345,
            MatchNo = "20001",
            Home = "Chelsea",
            Away = "Manchester City",
            LeagueName = "Premier League"
        };

        var assignment1 = new MatchAssignment
        {
            MatchId = 1234,
            UserId = 1,
            UserName = "John Doe"
        };

        var assignment2 = new MatchAssignment
        {
            MatchId = 1234,
            UserId = 2,
            UserName = "Jane Smith"
        };

        var assignment3 = new MatchAssignment
        {
            MatchId = 2345,
            UserId = 3,
            UserName = "Bob Johnson"
        };

        return new AssignmentSnapshot
        {
            Matches = new Dictionary<int, MatchInfo> { { 1234, match1 }, { 2345, match2 } },
            Assignments = new Dictionary<int, List<MatchAssignment>>
            {
                { 1234, new List<MatchAssignment> { assignment1, assignment2 } },
                { 2345, new List<MatchAssignment> { assignment3 } }
            }
        };
    }

    private static AssignmentSnapshot CreateSnapshotWithDuplicateAssignees()
    {
        var match1 = new MatchInfo
        {
            MatchId = 1234,
            MatchNo = "20000",
            Home = "Liverpool",
            Away = "Arsenal",
            LeagueName = "Premier League"
        };

        var match2 = new MatchInfo
        {
            MatchId = 2345,
            MatchNo = "20001",
            Home = "Chelsea",
            Away = "Manchester City",
            LeagueName = "Premier League"
        };

        var assignment1 = new MatchAssignment
        {
            MatchId = 1234,
            UserId = 1,
            UserName = "John Doe"
        };

        var assignment2 = new MatchAssignment
        {
            MatchId = 1234,
            UserId = 2,
            UserName = "Jane Smith"
        };

        var assignment3 = new MatchAssignment
        {
            MatchId = 2345,
            UserId = 1, // Duplicate UserId
            UserName = "John Doe" // Duplicate UserName
        };

        return new AssignmentSnapshot
        {
            Matches = new Dictionary<int, MatchInfo> { { 1234, match1 }, { 2345, match2 } },
            Assignments = new Dictionary<int, List<MatchAssignment>>
            {
                { 1234, new List<MatchAssignment> { assignment1, assignment2 } },
                { 2345, new List<MatchAssignment> { assignment3 } }
            }
        };
    }

    private static AssignmentSnapshot CreateSnapshotWithZeroUserId()
    {
        var match = new MatchInfo
        {
            MatchId = 1234,
            MatchNo = "20000",
            Home = "Liverpool",
            Away = "Arsenal",
            LeagueName = "Premier League"
        };

        var assignment1 = new MatchAssignment
        {
            MatchId = 1234,
            UserId = 1,
            UserName = "John Doe"
        };

        var assignment2 = new MatchAssignment
        {
            MatchId = 1234,
            UserId = 0, // Zero UserId should be filtered out
            UserName = "Invalid User"
        };

        return new AssignmentSnapshot
        {
            Matches = new Dictionary<int, MatchInfo> { { 1234, match } },
            Assignments = new Dictionary<int, List<MatchAssignment>>
            {
                { 1234, new List<MatchAssignment> { assignment1, assignment2 } }
            }
        };
    }
}
