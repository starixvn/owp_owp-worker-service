using OWP.SumReportData.LAReport.Enum;
using OWP.SumReportData.LAReport.Misc;
using OWP.SumReportData.LAReport.Models;
using Xunit;

namespace OWP.SumReportData.UnitTests.LAReport;

public class MainMatchRuleUnitTests
{
    private readonly IList<Assignment> assignments = new List<Assignment>
            {
                new Assignment
                {
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OWPLeagueName = "leaguemar27",
                    MinorBettypeId = null,
                    OWPMatchId = 428031,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 25, 15, 0, 0)
                },
                new Assignment
                {
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OWPLeagueName = "leaguemar27 - CORNERS",
                    MinorBettypeId = -4,
                    OWPMatchId = 428035,
                    ParentMatchId = 428031,
                    EventDateTime = new DateTime(2020, 7, 25, 15, 0, 0)
                },
                new Assignment
                {
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OWPLeagueName = "leaguemar28",
                    MinorBettypeId = null,
                    OWPMatchId = 428029,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 25, 15, 0, 0)
                },
                new Assignment
                {
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OWPLeagueName = "VIETNAM CHAMPIONSHIP U19 QUALIFIERS FANTASY MATCH",
                    MinorBettypeId = null,
                    OWPMatchId = 425385,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 26, 15, 0, 0)
                },
                new Assignment
                {
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OWPLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS - CORNERS - 5 mins play",
                    MinorBettypeId = null,
                    OWPMatchId = 425386,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 26, 15, 0, 0)
                },
                new Assignment
                {
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OWPLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS - CORNERS - 6 mins play",
                    MinorBettypeId = null,
                    OWPMatchId = 425389,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 26, 15, 0, 0)
                },
                new Assignment
                {
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OWPLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS - CORNERS - 15 mins play",
                    MinorBettypeId = null,
                    OWPMatchId = 425387,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 26, 15, 0, 0)
                },
                new Assignment
                {
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OWPLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS - CORNERS - 16 mins play",
                    MinorBettypeId = null,
                    OWPMatchId = 425555,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 26, 15, 0, 0)
                },
                new Assignment
                {
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OWPLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS - CORNERS - 20 mins play",
                    MinorBettypeId = null,
                    OWPMatchId = 425388,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 26, 15, 0, 0)
                }
            };

    [Fact]
    public void Execute_MainMatchRule_ShouldReturnOTHERS()
    {
        // Arrange
        var firstRule = new FTCornerRule();
        var secondRule = new IsFantasyRule();
        var thirdRule = new ESoccerRule();
        firstRule.SetSuccessor(secondRule);
        secondRule.SetSuccessor(thirdRule);
        var assignment = assignments.FirstOrDefault(x => x.OWPMatchId == 428029);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.OTHERS, actual);
    }

    [Fact]
    public void Execute_FTCornerRule_ShouldReturnFT_CORNER()
    {
        // Arrange
        var firstRule = new FTCornerRule();
        var secondRule = new IsFantasyRule();
        var thirdRule = new ESoccerRule();
        firstRule.SetSuccessor(secondRule);
        secondRule.SetSuccessor(thirdRule);
        var assignment = assignments.FirstOrDefault(x => x.OWPMatchId == 428031);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.FT_CORNER, actual);
    }

    [Fact]
    public void Execute_IsFantasyRule_ShouldReturnFANTASY()
    {
        // Arrange
        var firstRule = new FTCornerRule();
        var secondRule = new IsFantasyRule();
        var thirdRule = new ESoccerRule();
        firstRule.SetSuccessor(secondRule);
        secondRule.SetSuccessor(thirdRule);
        var assignment = assignments.FirstOrDefault(x => x.OWPMatchId == 425385);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.FANTASY, actual);
    }

    [Fact]
    public void Execute_ESoccerRule_ShouldReturnESOCCER_5MINS()
    {
        // Arrange
        var firstRule = new FTCornerRule();
        var secondRule = new IsFantasyRule();
        var thirdRule = new ESoccerRule();
        firstRule.SetSuccessor(secondRule);
        secondRule.SetSuccessor(thirdRule);
        var assignment = assignments.FirstOrDefault(x => x.OWPMatchId == 425386);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.ESOCCER_5MINS, actual);
    }

    [Fact]
    public void Execute_ESoccerRule_ShouldReturnESOCCER_6MINS()
    {
        // Arrange
        var firstRule = new FTCornerRule();
        var secondRule = new IsFantasyRule();
        var thirdRule = new ESoccerRule();
        firstRule.SetSuccessor(secondRule);
        secondRule.SetSuccessor(thirdRule);
        var assignment = assignments.FirstOrDefault(x => x.OWPMatchId == 425389);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.ESOCCER_6MINS, actual);
    }

    [Fact]
    public void Execute_ESoccerRule_ShouldReturnESOCCER_15MINS()
    {
        // Arrange
        var firstRule = new FTCornerRule();
        var secondRule = new IsFantasyRule();
        var thirdRule = new ESoccerRule();
        firstRule.SetSuccessor(secondRule);
        secondRule.SetSuccessor(thirdRule);
        var assignment = assignments.FirstOrDefault(x => x.OWPMatchId == 425387);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.ESOCCER_15MINS, actual);
    }

    [Fact]
    public void Execute_ESoccerRule_ShouldReturnESOCCER_16MINS()
    {
        // Arrange
        var firstRule = new FTCornerRule();
        var secondRule = new IsFantasyRule();
        var thirdRule = new ESoccerRule();
        firstRule.SetSuccessor(secondRule);
        secondRule.SetSuccessor(thirdRule);
        var assignment = assignments.FirstOrDefault(x => x.OWPMatchId == 425555);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.ESOCCER_16MINS, actual);
    }

    [Fact]
    public void Execute_ESoccerRule_ShouldReturnESOCCER_20MINS()
    {
        // Arrange
        var firstRule = new FTCornerRule();
        var secondRule = new IsFantasyRule();
        var thirdRule = new ESoccerRule();
        firstRule.SetSuccessor(secondRule);
        secondRule.SetSuccessor(thirdRule);
        var assignment = assignments.FirstOrDefault(x => x.OWPMatchId == 425388);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.ESOCCER_20MINS, actual);
    }
}