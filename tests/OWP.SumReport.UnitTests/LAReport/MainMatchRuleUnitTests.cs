namespace OWP.SumReport.UnitTests.LAReport;

using SumReport.LAReport.Enum;
using SumReport.LAReport.Misc;
using SumReport.LAReport.Models;
using Xunit;

public class MainMatchRuleUnitTests
{
    private readonly IList<Assignment> assignments = new List<Assignment>
            {
                new()
                {
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OwpLeagueName = "leaguemar27",
                    MinorBettypeId = null,
                    OwpMatchId = 428031,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 25, 15, 0, 0)
                },
                new()
                {
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OwpLeagueName = "leaguemar27 - CORNERS",
                    MinorBettypeId = -4,
                    OwpMatchId = 428035,
                    ParentMatchId = 428031,
                    EventDateTime = new DateTime(2020, 7, 25, 15, 0, 0)
                },
                new()
                {
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OwpLeagueName = "leaguemar28",
                    MinorBettypeId = null,
                    OwpMatchId = 428029,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 25, 15, 0, 0)
                },
                new()
                {
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OwpLeagueName = "VIETNAM CHAMPIONSHIP U19 QUALIFIERS FANTASY MATCH",
                    MinorBettypeId = null,
                    OwpMatchId = 425385,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 26, 15, 0, 0)
                },
                new()
                {
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OwpLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS - CORNERS - 5 mins play",
                    MinorBettypeId = null,
                    OwpMatchId = 425386,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 26, 15, 0, 0)
                },
                new()
                {
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OwpLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS - CORNERS - 6 mins play",
                    MinorBettypeId = null,
                    OwpMatchId = 425389,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 26, 15, 0, 0)
                },
                new()
                {
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OwpLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS - CORNERS - 15 mins play",
                    MinorBettypeId = null,
                    OwpMatchId = 425387,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 26, 15, 0, 0)
                },
                new()
                {
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OwpLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS - CORNERS - 16 mins play",
                    MinorBettypeId = null,
                    OwpMatchId = 425555,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 26, 15, 0, 0)
                },
                new()
                {
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OwpLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS - CORNERS - 20 mins play",
                    MinorBettypeId = null,
                    OwpMatchId = 425388,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 26, 15, 0, 0)
                },
                new()
                {
                    AssignmentId = 131,
                    UserId = 15477090,
                    OwpLeagueName = "*ENGLISH PREMIER LEAGUE",
                    MinorBettypeId = null,
                    OwpMatchId = 425998,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2022, 4, 9, 15, 0, 0)
                },
                new()
                {
                    AssignmentId = 131,
                    UserId = 15477090,
                    OwpLeagueName = "*ENGLISH PREMIER LEAGUE - CORNERS",
                    MinorBettypeId = -4,
                    OwpMatchId = 427128,
                    ParentMatchId = 425998,
                    EventDateTime = new DateTime(2022, 4, 9, 15, 0, 0)
                }
            };

    [Fact]
    public void Execute_MainMatchRule_ShouldReturn5ML()
    {
        // Arrange
        var firstRule = new FiveMajorLeagueRule(); 
        var secondRule = new IsFantasyRule();
        var thirdRule = new FtCornerRule();
        firstRule.SetSuccessor(secondRule);
        secondRule.SetSuccessor(thirdRule);
        var assignment = assignments.FirstOrDefault(x => x.OwpMatchId == 425998);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.MajorLeague, actual);
    }

    [Fact]
    public void Execute_MainMatchRule_ShouldReturnFTCorner5ML()
    {
        // Arrange
        var firstRule = new FtCorner5MlRule(); 
        var secondRule = new IsFantasyRule();
        var thirdRule = new FtCornerRule();
        firstRule.SetSuccessor(secondRule);
        secondRule.SetSuccessor(thirdRule);
        var assignment = assignments.FirstOrDefault(x => x.OwpMatchId == 425998);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.FtCorner5Ml, actual);
    }

    [Fact]
    public void Execute_MainMatchRule_ShouldReturnOTHERS()
    {
        // Arrange
        var firstRule = new FtCornerRule();
        var secondRule = new IsFantasyRule();
        var thirdRule = new ESoccerRule();
        firstRule.SetSuccessor(secondRule);
        secondRule.SetSuccessor(thirdRule);
        var assignment = assignments.FirstOrDefault(x => x.OwpMatchId == 428029);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.Others, actual);
    }

    [Fact]
    public void Execute_FTCornerRule_ShouldReturnFT_CORNER()
    {
        // Arrange
        var firstRule = new FtCornerRule();
        var secondRule = new IsFantasyRule();
        var thirdRule = new ESoccerRule();
        firstRule.SetSuccessor(secondRule);
        secondRule.SetSuccessor(thirdRule);
        var assignment = assignments.FirstOrDefault(x => x.OwpMatchId == 428031);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.FtCorner, actual);
    }

    [Fact]
    public void Execute_IsFantasyRule_ShouldReturnFANTASY()
    {
        // Arrange
        var firstRule = new FtCornerRule();
        var secondRule = new IsFantasyRule();
        var thirdRule = new ESoccerRule();
        firstRule.SetSuccessor(secondRule);
        secondRule.SetSuccessor(thirdRule);
        var assignment = assignments.FirstOrDefault(x => x.OwpMatchId == 425385);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.Fantasy, actual);
    }

    [Fact]
    public void Execute_ESoccerRule_ShouldReturnESOCCER_5MINS()
    {
        // Arrange
        var firstRule = new FtCornerRule();
        var secondRule = new IsFantasyRule();
        var thirdRule = new ESoccerRule();
        firstRule.SetSuccessor(secondRule);
        secondRule.SetSuccessor(thirdRule);
        var assignment = assignments.FirstOrDefault(x => x.OwpMatchId == 425386);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.Esoccer5Mins, actual);
    }

    [Fact]
    public void Execute_ESoccerRule_ShouldReturnESOCCER_6MINS()
    {
        // Arrange
        var firstRule = new FtCornerRule();
        var secondRule = new IsFantasyRule();
        var thirdRule = new ESoccerRule();
        firstRule.SetSuccessor(secondRule);
        secondRule.SetSuccessor(thirdRule);
        var assignment = assignments.FirstOrDefault(x => x.OwpMatchId == 425389);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.Esoccer6Mins, actual);
    }

    [Fact]
    public void Execute_ESoccerRule_ShouldReturnESOCCER_15MINS()
    {
        // Arrange
        var firstRule = new FtCornerRule();
        var secondRule = new IsFantasyRule();
        var thirdRule = new ESoccerRule();
        firstRule.SetSuccessor(secondRule);
        secondRule.SetSuccessor(thirdRule);
        var assignment = assignments.FirstOrDefault(x => x.OwpMatchId == 425387);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.Esoccer15Mins, actual);
    }

    [Fact]
    public void Execute_ESoccerRule_ShouldReturnESOCCER_16MINS()
    {
        // Arrange
        var firstRule = new FtCornerRule();
        var secondRule = new IsFantasyRule();
        var thirdRule = new ESoccerRule();
        firstRule.SetSuccessor(secondRule);
        secondRule.SetSuccessor(thirdRule);
        var assignment = assignments.FirstOrDefault(x => x.OwpMatchId == 425555);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.Esoccer16Mins, actual);
    }

    [Fact]
    public void Execute_ESoccerRule_ShouldReturnESOCCER_20MINS()
    {
        // Arrange
        var firstRule = new FtCornerRule();
        var secondRule = new IsFantasyRule();
        var thirdRule = new ESoccerRule();
        firstRule.SetSuccessor(secondRule);
        secondRule.SetSuccessor(thirdRule);
        var assignment = assignments.FirstOrDefault(x => x.OwpMatchId == 425388);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.Esoccer20Mins, actual);
    }
}