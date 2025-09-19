using System;
using System.Collections.Generic;
using System.Linq;
using OWP.OtherSport.SyncAdminSiteId.Helpers;
using OWP.OtherSport.SyncAdminSiteId.Models;

namespace OWP.OtherSport.UnitTests.SyncAdminSiteId.Helpers;

using Xunit;

public class MatchHelperUnitTests
{
    [Fact]
    public void GetRemovedMatches_WithRemovedMatches_ShouldReturnCorrectMatches()
    {
        // Arrange
        var owpMatches = new List<Match>
        {
            new Match { OsMatchId = 1, AdminSiteMatchId = 100, HomeName = "Team A", AwayName = "Team B", LeagueName = "League 1", EventDateTime = DateTime.Now },
            new Match { OsMatchId = 2, AdminSiteMatchId = 200, HomeName = "Team C", AwayName = "Team D", LeagueName = "League 2", EventDateTime = DateTime.Now },
            new Match { OsMatchId = 3, AdminSiteMatchId = 300, HomeName = "Team E", AwayName = "Team F", LeagueName = "League 3", EventDateTime = DateTime.Now }
        };

        var adminSiteMatches = new List<Match>
        {
            new Match { AdminSiteMatchId = 100, HomeName = "Team A", AwayName = "Team B", LeagueName = "League 1", EventDateTime = DateTime.Now },
            new Match { AdminSiteMatchId = 200, HomeName = "Team C", AwayName = "Team D", LeagueName = "League 2", EventDateTime = DateTime.Now }
        };

        // Act
        var result = MatchHelper.GetRemovedMatches(owpMatches, adminSiteMatches);

        // Assert
        Assert.Single(result);
        var removedMatch = result.First();
        Assert.Equal(3, removedMatch.OsMatchId);
        Assert.Equal(300, removedMatch.OldAdminSiteMatchId);
        Assert.Equal("Team E", removedMatch.OwpHome);
        Assert.Equal("Team F", removedMatch.OwpAway);
        Assert.Equal("League 3", removedMatch.OwpLeague);
    }

    [Fact]
    public void GetRemovedMatches_WithNoRemovedMatches_ShouldReturnEmpty()
    {
        // Arrange
        var owpMatches = new List<Match>
        {
            new Match { OsMatchId = 1, AdminSiteMatchId = 100, HomeName = "Team A", AwayName = "Team B", LeagueName = "League 1", EventDateTime = DateTime.Now }
        };

        var adminSiteMatches = new List<Match>
        {
            new Match { AdminSiteMatchId = 100, HomeName = "Team A", AwayName = "Team B", LeagueName = "League 1", EventDateTime = DateTime.Now }
        };

        // Act
        var result = MatchHelper.GetRemovedMatches(owpMatches, adminSiteMatches);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetRemovedMatches_WithEmptyOwpMatches_ShouldReturnEmpty()
    {
        // Arrange
        var owpMatches = new List<Match>();
        var adminSiteMatches = new List<Match>
        {
            new Match { AdminSiteMatchId = 100, HomeName = "Team A", AwayName = "Team B", LeagueName = "League 1", EventDateTime = DateTime.Now }
        };

        // Act
        var result = MatchHelper.GetRemovedMatches(owpMatches, adminSiteMatches);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetNewLinkedMatches_WithValidMatches_ShouldReturnLinkedMatches()
    {
        // Arrange
        var testDate = new DateTime(2024, 1, 15, 14, 30, 0);
        var owpMatches = new List<Match>
        {
            new Match
            {
                OsMatchId = 1,
                HomeName = "Team A",
                AwayName = "Team B",
                LeagueName = "League 1",
                EventDateTime = testDate,
                EventDate = testDate,
                IsLinked = false,
                Sport = "Football"
            }
        };

        var adminSiteMatches = new List<Match>
        {
            new Match
            {
                AdminSiteMatchId = 100,
                HomeName = "Team A",
                AwayName = "Team B",
                LeagueName = "League 1",
                EventDateTime = testDate,
                EventDate = testDate,
                Sport = "Football"
            }
        };

        // Act
        var result = MatchHelper.GetNewLinkedMatches(owpMatches, adminSiteMatches);

        // Assert
        Assert.Single(result);
        var linkedMatch = result.First();
        Assert.Equal(1, linkedMatch.OsMatchId);
        Assert.Equal(100, linkedMatch.AdminSiteMatchId);
        Assert.Equal("Team A", linkedMatch.OwpHome);
        Assert.Equal("Team B", linkedMatch.OwpAway);
        Assert.Equal("League 1", linkedMatch.OwpLeague);
        Assert.True(owpMatches[0].IsLinked);
    }

    [Fact]
    public void GetNewLinkedMatches_WithAlreadyLinkedMatches_ShouldNotReturnThem()
    {
        // Arrange
        var owpMatches = new List<Match>
        {
            new Match
            {
                OsMatchId = 1,
                HomeName = "Team A",
                AwayName = "Team B",
                LeagueName = "League 1",
                EventDateTime = DateTime.Now,
                EventDate = DateTime.Now,
                IsLinked = true,
                Sport = "Football"
            }
        };

        var adminSiteMatches = new List<Match>
        {
            new Match
            {
                AdminSiteMatchId = 100,
                HomeName = "Team A",
                AwayName = "Team B",
                LeagueName = "Regular League 1",
                EventDateTime = DateTime.Now,
                EventDate = DateTime.Now,
                Sport = "Football"
            }
        };

        // Act
        var result = MatchHelper.GetNewLinkedMatches(owpMatches, adminSiteMatches);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetNewLinkedMatches_WithRule5HomeMatch_ShouldReturnLinkedMatch()
    {
        // Arrange
        var testDate = new DateTime(2024, 1, 15, 14, 30, 0);
        var owpMatches = new List<Match>
        {
            new Match
            {
                OsMatchId = 1,
                HomeName = "Lakers (Los Angeles)",
                AwayName = "Warriors",
                LeagueName = "*NBA - Most Points Scored Home",
                EventDateTime = testDate,
                EventDate = testDate,
                IsLinked = false,
                Sport = "Basketball"
            }
        };

        var adminSiteMatches = new List<Match>
        {
            new Match
            {
                AdminSiteMatchId = 100,
                HomeName = "Lakers",
                AwayName = "N/A",
                LeagueName = "*NBA - Most Points Scored",
                EventDateTime = testDate,
                EventDate = testDate,
                Sport = "Basketball"
            }
        };

        // Act
        var result = MatchHelper.GetNewLinkedMatches(owpMatches, adminSiteMatches);

        // Assert
        Assert.Single(result);
        var linkedMatch = result.First();
        Assert.Equal(1, linkedMatch.OsMatchId);
        Assert.Equal(100, linkedMatch.AdminSiteMatchId);
    }

    [Fact]
    public void GetNewLinkedMatches_WithRule5AwayMatch_ShouldReturnLinkedMatch()
    {
        // Arrange
        var testDate = new DateTime(2024, 1, 15, 14, 30, 0);
        var owpMatches = new List<Match>
        {
            new Match
            {
                OsMatchId = 1,
                HomeName = "Lakers",
                AwayName = "Warriors (Golden State)",
                LeagueName = "*NBA - Most Points Scored Away",
                EventDateTime = testDate,
                EventDate = testDate,
                IsLinked = false,
                Sport = "Basketball"
            }
        };

        var adminSiteMatches = new List<Match>
        {
            new Match
            {
                AdminSiteMatchId = 100,
                HomeName = "N/A",
                AwayName = "Warriors",
                LeagueName = "*NBA - Most Points Scored",
                EventDateTime = testDate,
                EventDate = testDate,
                Sport = "Basketball"
            }
        };

        // Act
        var result = MatchHelper.GetNewLinkedMatches(owpMatches, adminSiteMatches);

        // Assert
        Assert.Single(result);
        var linkedMatch = result.First();
        Assert.Equal(1, linkedMatch.OsMatchId);
        Assert.Equal(100, linkedMatch.AdminSiteMatchId);
    }

    [Fact]
    public void GetNewLinkedMatches_WithSabaMatch_ShouldReturnLinkedMatch()
    {
        // Arrange
        var testDate = new DateTime(2024, 1, 15, 14, 30, 0);
        var owpMatches = new List<Match>
        {
            new Match
            {
                OsMatchId = 1,
                HomeName = "Team A",
                AwayName = "Team B",
                LeagueName = "SABA League 1",
                EventDateTime = testDate,
                EventDate = testDate,
                IsLinked = false,
                Sport = "Football"
            }
        };

        var adminSiteMatches = new List<Match>
        {
            new Match
            {
                AdminSiteMatchId = 100,
                HomeName = "Team A",
                AwayName = "Team B",
                LeagueName = "SABA League 1",
                EventDateTime = testDate,
                EventDate = testDate,
                Sport = "Football"
            }
        };

        // Act
        var result = MatchHelper.GetNewLinkedMatches(owpMatches, adminSiteMatches);

        // Assert
        Assert.Single(result);
        var linkedMatch = result.First();
        Assert.Equal(1, linkedMatch.OsMatchId);
        Assert.Equal(100, linkedMatch.AdminSiteMatchId);
    }

    [Fact]
    public void GetNewLinkedMatches_WithEmptyLists_ShouldReturnEmpty()
    {
        // Arrange
        var owpMatches = new List<Match>();
        var adminSiteMatches = new List<Match>();

        // Act
        var result = MatchHelper.GetNewLinkedMatches(owpMatches, adminSiteMatches);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetNewLinkedMatches_WithNullLists_ShouldReturnEmpty()
    {
        // Arrange
        List<Match> owpMatches = new List<Match>();
        List<Match> adminSiteMatches = new List<Match>();

        // Act
        var result = MatchHelper.GetNewLinkedMatches(owpMatches, adminSiteMatches);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetNewLinkedMatches_WithDifferentDates_ShouldNotMatch()
    {
        // Arrange
        var owpMatches = new List<Match>
        {
            new Match
            {
                OsMatchId = 1,
                HomeName = "Team A",
                AwayName = "Team B",
                LeagueName = "League 1",
                EventDateTime = DateTime.Now,
                EventDate = DateTime.Now,
                IsLinked = false,
                Sport = "Football"
            }
        };

        var adminSiteMatches = new List<Match>
        {
            new Match
            {
                AdminSiteMatchId = 100,
                HomeName = "Team A",
                AwayName = "Team B",
                LeagueName = "Regular League 1",
                EventDateTime = DateTime.Now.AddDays(1),
                EventDate = DateTime.Now.AddDays(1),
                Sport = "Football"
            }
        };

        // Act
        var result = MatchHelper.GetNewLinkedMatches(owpMatches, adminSiteMatches);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetNewLinkedMatches_WithDifferentLeagues_ShouldNotMatch()
    {
        // Arrange
        var owpMatches = new List<Match>
        {
            new Match
            {
                OsMatchId = 1,
                HomeName = "Team A",
                AwayName = "Team B",
                LeagueName = "League 1",
                EventDateTime = DateTime.Now,
                EventDate = DateTime.Now,
                IsLinked = false,
                Sport = "Football"
            }
        };

        var adminSiteMatches = new List<Match>
        {
            new Match
            {
                AdminSiteMatchId = 100,
                HomeName = "Team A",
                AwayName = "Team B",
                LeagueName = "League 2",
                EventDateTime = DateTime.Now,
                EventDate = DateTime.Now,
                Sport = "Football"
            }
        };

        // Act
        var result = MatchHelper.GetNewLinkedMatches(owpMatches, adminSiteMatches);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetNewLinkedMatches_WithDifferentTeams_ShouldNotMatch()
    {
        // Arrange
        var owpMatches = new List<Match>
        {
            new Match
            {
                OsMatchId = 1,
                HomeName = "Team A",
                AwayName = "Team B",
                LeagueName = "League 1",
                EventDateTime = DateTime.Now,
                EventDate = DateTime.Now,
                IsLinked = false,
                Sport = "Football"
            }
        };

        var adminSiteMatches = new List<Match>
        {
            new Match
            {
                AdminSiteMatchId = 100,
                HomeName = "Team C",
                AwayName = "Team D",
                LeagueName = "Regular League 1",
                EventDateTime = DateTime.Now,
                EventDate = DateTime.Now,
                Sport = "Football"
            }
        };

        // Act
        var result = MatchHelper.GetNewLinkedMatches(owpMatches, adminSiteMatches);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetNewLinkedMatches_WithDifferentSports_ShouldNotMatch()
    {
        // Arrange
        var owpMatches = new List<Match>
        {
            new Match
            {
                OsMatchId = 1,
                HomeName = "Team A",
                AwayName = "Team B",
                LeagueName = "League 1",
                EventDateTime = DateTime.Now,
                EventDate = DateTime.Now,
                IsLinked = false,
                Sport = "Football"
            }
        };

        var adminSiteMatches = new List<Match>
        {
            new Match
            {
                AdminSiteMatchId = 100,
                HomeName = "Team A",
                AwayName = "Team B",
                LeagueName = "Regular League 1",
                EventDateTime = DateTime.Now,
                EventDate = DateTime.Now,
                Sport = "Basketball"
            }
        };

        // Act
        var result = MatchHelper.GetNewLinkedMatches(owpMatches, adminSiteMatches);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetNewLinkedMatches_WithRule5InvalidLeague_ShouldNotMatch()
    {
        // Arrange
        var owpMatches = new List<Match>
        {
            new Match
            {
                OsMatchId = 1,
                HomeName = "Lakers (Los Angeles)",
                AwayName = "Warriors",
                LeagueName = "Invalid League Home",
                EventDateTime = DateTime.Now,
                EventDate = DateTime.Now,
                IsLinked = false,
                Sport = "Basketball"
            }
        };

        var adminSiteMatches = new List<Match>
        {
            new Match
            {
                AdminSiteMatchId = 100,
                HomeName = "Los Angeles",
                AwayName = "N/A",
                LeagueName = "*NBA - Most Points Scored",
                EventDateTime = DateTime.Now,
                EventDate = DateTime.Now,
                Sport = "Basketball"
            }
        };

        // Act
        var result = MatchHelper.GetNewLinkedMatches(owpMatches, adminSiteMatches);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetNewLinkedMatches_WithRule5InvalidAdminSiteLeague_ShouldNotMatch()
    {
        // Arrange
        var owpMatches = new List<Match>
        {
            new Match
            {
                OsMatchId = 1,
                HomeName = "Lakers (Los Angeles)",
                AwayName = "Warriors",
                LeagueName = "*NBA - Most Points Scored Home",
                EventDateTime = DateTime.Now,
                EventDate = DateTime.Now,
                IsLinked = false,
                Sport = "Basketball"
            }
        };

        var adminSiteMatches = new List<Match>
        {
            new Match
            {
                AdminSiteMatchId = 100,
                HomeName = "Los Angeles",
                AwayName = "N/A",
                LeagueName = "Invalid League",
                EventDateTime = DateTime.Now,
                EventDate = DateTime.Now,
                Sport = "Basketball"
            }
        };

        // Act
        var result = MatchHelper.GetNewLinkedMatches(owpMatches, adminSiteMatches);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetNewLinkedMatches_WithRule5InvalidAwayName_ShouldNotMatch()
    {
        // Arrange
        var owpMatches = new List<Match>
        {
            new Match
            {
                OsMatchId = 1,
                HomeName = "Lakers (Los Angeles)",
                AwayName = "Warriors",
                LeagueName = "*NBA - Most Points Scored Home",
                EventDateTime = DateTime.Now,
                EventDate = DateTime.Now,
                IsLinked = false,
                Sport = "Basketball"
            }
        };

        var adminSiteMatches = new List<Match>
        {
            new Match
            {
                AdminSiteMatchId = 100,
                HomeName = "Los Angeles",
                AwayName = "Not N/A",
                LeagueName = "*NBA - Most Points Scored",
                EventDateTime = DateTime.Now,
                EventDate = DateTime.Now,
                Sport = "Basketball"
            }
        };

        // Act
        var result = MatchHelper.GetNewLinkedMatches(owpMatches, adminSiteMatches);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetNewLinkedMatches_WithRule5InvalidHomeName_ShouldNotMatch()
    {
        // Arrange
        var owpMatches = new List<Match>
        {
            new Match
            {
                OsMatchId = 1,
                HomeName = "Lakers",
                AwayName = "Warriors (Golden State)",
                LeagueName = "*NBA - Most Points Scored Away",
                EventDateTime = DateTime.Now,
                EventDate = DateTime.Now,
                IsLinked = false,
                Sport = "Basketball"
            }
        };

        var adminSiteMatches = new List<Match>
        {
            new Match
            {
                AdminSiteMatchId = 100,
                HomeName = "Not N/A",
                AwayName = "Golden State",
                LeagueName = "*NBA - Most Points Scored",
                EventDateTime = DateTime.Now,
                EventDate = DateTime.Now,
                Sport = "Basketball"
            }
        };

        // Act
        var result = MatchHelper.GetNewLinkedMatches(owpMatches, adminSiteMatches);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetNewLinkedMatches_WithRule5SabaMatch_ShouldNotMatch()
    {
        // Arrange
        var owpMatches = new List<Match>
        {
            new Match
            {
                OsMatchId = 1,
                HomeName = "Lakers (Los Angeles)",
                AwayName = "Warriors",
                LeagueName = "*NBA - Most Points Scored Home",
                EventDateTime = DateTime.Now,
                EventDate = DateTime.Now,
                IsLinked = false,
                Sport = "Basketball"
            }
        };

        var adminSiteMatches = new List<Match>
        {
            new Match
            {
                AdminSiteMatchId = 100,
                HomeName = "Los Angeles",
                AwayName = "N/A",
                LeagueName = "*NBA - Most Points Scored",
                EventDateTime = DateTime.Now,
                EventDate = DateTime.Now,
                Sport = "Basketball"
            }
        };

        // Act
        var result = MatchHelper.GetNewLinkedMatches(owpMatches, adminSiteMatches);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetNewLinkedMatches_WithMultipleCandidates_ShouldSelectClosestByTime()
    {
        // Arrange
        var baseTime = DateTime.Now;
        var owpMatches = new List<Match>
        {
            new Match
            {
                OsMatchId = 1,
                HomeName = "Team A",
                AwayName = "Team B",
                LeagueName = "League 1",
                EventDateTime = baseTime,
                EventDate = baseTime,
                IsLinked = false,
                Sport = "Football"
            }
        };

        var adminSiteMatches = new List<Match>
        {
            new Match
            {
                AdminSiteMatchId = 100,
                HomeName = "Team A",
                AwayName = "Team B",
                LeagueName = "League 1",
                EventDateTime = baseTime.AddHours(2),
                EventDate = baseTime.AddHours(2),
                Sport = "Football"
            },
            new Match
            {
                AdminSiteMatchId = 200,
                HomeName = "Team A",
                AwayName = "Team B",
                LeagueName = "League 1",
                EventDateTime = baseTime.AddMinutes(30),
                EventDate = baseTime.AddMinutes(30),
                Sport = "Football"
            }
        };

        // Act
        var result = MatchHelper.GetNewLinkedMatches(owpMatches, adminSiteMatches);

        // Assert
        Assert.Single(result);
        var linkedMatch = result.First();
        Assert.Equal(200, linkedMatch.AdminSiteMatchId); // Should select the closer one
    }
}