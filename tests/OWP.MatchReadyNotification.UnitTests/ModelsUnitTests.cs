namespace OWP.MatchReadyNotification.UnitTests;

using Models;
using Xunit;

public class ModelsUnitTests
{
    [Fact]
    public void Assignee_Equals_WithSameValues_ShouldReturnTrue()
    {
        // Arrange
        var assignee1 = new Assignee { AssigneeId = 1, AssigneeName = "John Doe" };
        var assignee2 = new Assignee { AssigneeId = 1, AssigneeName = "John Doe" };

        // Act
        var result = assignee1.Equals(assignee1, assignee2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Assignee_Equals_WithDifferentIds_ShouldReturnFalse()
    {
        // Arrange
        var assignee1 = new Assignee { AssigneeId = 1, AssigneeName = "John Doe" };
        var assignee2 = new Assignee { AssigneeId = 2, AssigneeName = "John Doe" };

        // Act
        var result = assignee1.Equals(assignee1, assignee2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Assignee_Equals_WithDifferentNames_ShouldReturnFalse()
    {
        // Arrange
        var assignee1 = new Assignee { AssigneeId = 1, AssigneeName = "John Doe" };
        var assignee2 = new Assignee { AssigneeId = 1, AssigneeName = "Jane Smith" };

        // Act
        var result = assignee1.Equals(assignee1, assignee2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Assignee_Equals_WithNullValues_ShouldReturnFalse()
    {
        // Arrange
        var assignee1 = new Assignee { AssigneeId = 1, AssigneeName = "John Doe" };
        Assignee assignee2 = null;

        // Act
        var result = assignee1.Equals(assignee1, assignee2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Assignee_Equals_WithBothNullValues_ShouldReturnFalse()
    {
        // Arrange
        Assignee assignee1 = null;
        Assignee assignee2 = null;

        // Act
        var result = new Assignee().Equals(assignee1, assignee2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Assignee_GetHashCode_ShouldReturnAssigneeId()
    {
        // Arrange
        var assignee = new Assignee { AssigneeId = 123, AssigneeName = "John Doe" };

        // Act
        var result = assignee.GetHashCode(assignee);

        // Assert
        Assert.Equal(123, result);
    }

    [Fact]
    public void MessageModel_Properties_ShouldBeSettableAndGettable()
    {
        // Arrange
        var messageModel = new MessageModel();

        // Act
        messageModel.MessageContent = "Test message";
        messageModel.MessageType = 1;
        messageModel.IsImportant = true;
        messageModel.IsWatcher = false;
        messageModel.ReceivedUserIds = "1,2,3";
        messageModel.ReceivedUserNames = "John, Jane, Bob";

        // Assert
        Assert.Equal("Test message", messageModel.MessageContent);
        Assert.Equal(1, messageModel.MessageType);
        Assert.True(messageModel.IsImportant);
        Assert.False(messageModel.IsWatcher);
        Assert.Equal("1,2,3", messageModel.ReceivedUserIds);
        Assert.Equal("John, Jane, Bob", messageModel.ReceivedUserNames);
    }

    [Fact]
    public void NotReadyMatchInfo_Properties_ShouldBeSettableAndGettable()
    {
        // Arrange
        var notReadyMatchInfo = new NotReadyMatchInfo();

        // Act
        notReadyMatchInfo.MatchNo = "20000";
        notReadyMatchInfo.Event = "Liverpool vs Arsenal";
        notReadyMatchInfo.LeagueName = "Premier League";
        notReadyMatchInfo.KoTime = new TimeSpan(15, 30, 0);
        notReadyMatchInfo.Assignees = new List<Assignee>
        {
            new Assignee { AssigneeId = 1, AssigneeName = "John Doe" }
        };

        // Assert
        Assert.Equal("20000", notReadyMatchInfo.MatchNo);
        Assert.Equal("Liverpool vs Arsenal", notReadyMatchInfo.Event);
        Assert.Equal("Premier League", notReadyMatchInfo.LeagueName);
        Assert.Equal(new TimeSpan(15, 30, 0), notReadyMatchInfo.KoTime);
        Assert.Single(notReadyMatchInfo.Assignees);
        Assert.Equal(1, notReadyMatchInfo.Assignees[0].AssigneeId);
        Assert.Equal("John Doe", notReadyMatchInfo.Assignees[0].AssigneeName);
    }

    [Fact]
    public void MatchInfo_Properties_ShouldBeSettableAndGettable()
    {
        // Arrange
        var matchInfo = new MatchInfo();

        // Act
        matchInfo.MatchId = 1234;
        matchInfo.ParentMatchId = 5678;
        matchInfo.LeagueName = "Premier League";
        matchInfo.Home = "Liverpool";
        matchInfo.Away = "Arsenal";
        matchInfo.MatchNo = "20000";
        matchInfo.KoTime = new TimeSpan(15, 30, 0);

        // Assert
        Assert.Equal(1234, matchInfo.MatchId);
        Assert.Equal(5678, matchInfo.ParentMatchId);
        Assert.Equal("Premier League", matchInfo.LeagueName);
        Assert.Equal("Liverpool", matchInfo.Home);
        Assert.Equal("Arsenal", matchInfo.Away);
        Assert.Equal("20000", matchInfo.MatchNo);
        Assert.Equal(new TimeSpan(15, 30, 0), matchInfo.KoTime);
    }

    [Fact]
    public void MatchAssignment_Properties_ShouldBeSettableAndGettable()
    {
        // Arrange
        var matchAssignment = new MatchAssignment();

        // Act
        matchAssignment.MatchId = 1234;
        matchAssignment.UserId = 567;
        matchAssignment.UserName = "John Doe";

        // Assert
        Assert.Equal(1234, matchAssignment.MatchId);
        Assert.Equal(567, matchAssignment.UserId);
        Assert.Equal("John Doe", matchAssignment.UserName);
    }

    [Fact]
    public void AssignmentSnapshot_DefaultValues_ShouldBeInitialized()
    {
        // Arrange & Act
        var assignmentSnapshot = new AssignmentSnapshot();

        // Assert
        Assert.NotNull(assignmentSnapshot.Matches);
        Assert.NotNull(assignmentSnapshot.Assignments);
        Assert.Equal(string.Empty, assignmentSnapshot.LastDateTimeNotification);
    }

    [Fact]
    public void AssignmentSnapshot_Properties_ShouldBeSettableAndGettable()
    {
        // Arrange
        var assignmentSnapshot = new AssignmentSnapshot();
        var matches = new Dictionary<int, MatchInfo>
        {
            { 1234, new MatchInfo { MatchId = 1234, MatchNo = "20000" } }
        };
        var assignments = new Dictionary<int, List<MatchAssignment>>
        {
            { 1234, new List<MatchAssignment> { new MatchAssignment { MatchId = 1234, UserId = 1 } } }
        };

        // Act
        assignmentSnapshot.Matches = matches;
        assignmentSnapshot.Assignments = assignments;
        assignmentSnapshot.LastDateTimeNotification = "2024-01-01 10:00:00";

        // Assert
        Assert.Equal(matches, assignmentSnapshot.Matches);
        Assert.Equal(assignments, assignmentSnapshot.Assignments);
        Assert.Equal("2024-01-01 10:00:00", assignmentSnapshot.LastDateTimeNotification);
    }

    [Fact]
    public void SessionManager_Properties_ShouldBeSettableAndGettable()
    {
        // Arrange
        var sessionManager = new SessionManager();

        // Act
        sessionManager.UserId = 123;
        sessionManager.ReceivedUserId = 456;
        sessionManager.ReceivedUserName = "Manager Name";

        // Assert
        Assert.Equal(123, sessionManager.UserId);
        Assert.Equal(456, sessionManager.ReceivedUserId);
        Assert.Equal("Manager Name", sessionManager.ReceivedUserName);
    }

    [Fact]
    public void Assignee_EqualityComparer_ShouldWorkWithDistinct()
    {
        // Arrange
        var assignees = new List<Assignee>
        {
            new Assignee { AssigneeId = 1, AssigneeName = "John Doe" },
            new Assignee { AssigneeId = 1, AssigneeName = "John Doe" }, // Duplicate
            new Assignee { AssigneeId = 2, AssigneeName = "Jane Smith" }
        };

        // Act
        var distinctAssignees = assignees.Distinct(new Assignee()).ToList();

        // Assert
        Assert.Equal(2, distinctAssignees.Count);
        Assert.Contains(distinctAssignees, a => a.AssigneeId == 1 && a.AssigneeName == "John Doe");
        Assert.Contains(distinctAssignees, a => a.AssigneeId == 2 && a.AssigneeName == "Jane Smith");
    }

    [Fact]
    public void Assignee_EqualityComparer_WithNullValues_ShouldNotThrow()
    {
        // Arrange
        var assignees = new List<Assignee>
        {
            new Assignee { AssigneeId = 1, AssigneeName = "John Doe" },
            null,
            new Assignee { AssigneeId = 2, AssigneeName = "Jane Smith" }
        };

        // Act & Assert
        // Should not throw exception when comparing with null
        var result = new Assignee().Equals(assignees[0], assignees[1]);
        Assert.False(result);
    }
}
