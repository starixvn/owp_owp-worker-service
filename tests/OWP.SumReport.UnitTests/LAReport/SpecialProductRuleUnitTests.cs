namespace OWP.SumReport.UnitTests.LAReport;

using SumReport.LAReport.Enum;
using SumReport.LAReport.Misc;
using SumReport.LAReport.Models;
using Xunit;

public class SpecialProductRuleUnitTests
{
    private readonly IList<Assignment> assignments = new List<Assignment>
            {
                new()
                {
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OWPLeagueName = "leaguemar27",
                    MinorBettypeId = null,
                    OWPMatchId = 428031,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 25, 15, 0, 0)
                },
                new()
                {
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OWPLeagueName = "leaguemar27 - CORNERS",
                    MinorBettypeId = -4,
                    OWPMatchId = 428035,
                    ParentMatchId = 428031,
                    EventDateTime = new DateTime(2020, 7, 25, 15, 0, 0)
                },
                new()
                {
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OWPLeagueName = "leaguemar28",
                    MinorBettypeId = -6,
                    OWPMatchId = 428029,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 25, 15, 0, 0)
                },
                new()
                {
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OWPLeagueName = "VIETNAM CHAMPIONSHIP U19 QUALIFIERS FANTASY MATCH",
                    MinorBettypeId = null,
                    OWPMatchId = 425385,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 26, 15, 0, 0)
                },
                new()
                {
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OWPLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS - CORNERS - 5 mins play",
                    MinorBettypeId = -4,
                    OWPMatchId = 425386,
                    ParentMatchId = 123456,
                    EventDateTime = new DateTime(2020, 7, 26, 15, 0, 0)
                },
                new()
                {
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OWPLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS - CORNERS - 15 mins play",
                    MinorBettypeId = -100,
                    OWPMatchId = 425387,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 26, 15, 0, 0)
                },
                new()
                {
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OWPLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS - CORNERS - 20 mins play",
                    MinorBettypeId = null,
                    OWPMatchId = 425388,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 26, 15, 0, 0)
                },
                new()
                {
                    AssignmentId = 131,
                    UserId = 15477090,
                    OWPLeagueName = "*ENGLISH PREMIER LEAGUE",
                    MinorBettypeId = null,
                    OWPMatchId = 425998,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2022, 4, 9, 15, 0, 0)
                },
                new()
                {
                    AssignmentId = 131,
                    UserId = 15477090,
                    OWPLeagueName = "*ENGLISH PREMIER LEAGUE - CORNERS",
                    MinorBettypeId = -4,
                    OWPMatchId = 427128,
                    ParentMatchId = 425998,
                    EventDateTime = new DateTime(2022, 4, 9, 15, 0, 0)
                }
            };
    [Fact]
    public void Excute_CornerRule_ShouldReturnFT_CORNER_5ML()
    {
        // Arrange
        var firstRule = new NotCornerRule();
        var secondRule = new IsCornerRule();
        firstRule.SetSuccessor(secondRule);
        var assignment = assignments.FirstOrDefault(x => x.OWPMatchId == 427128);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.FT_CORNER_5ML, actual);
    }
    [Fact]
    public void Excute_NotCornerRule_ShouldReturnTOTAL_BOOKING_PLUS()
    {
        // Arrange
        var firstRule = new NotCornerRule();
        var secondRule = new IsCornerRule();
        firstRule.SetSuccessor(secondRule);
        var assignment = assignments.FirstOrDefault(x => x.OWPMatchId == 428029);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.TOTAL_BOOKING_PLUS, actual);
    }

    [Fact]
    public void Excute_NotCornerRule_ShouldReturnOTHER_SPECIAL_PRODUCTS()
    {
        // Arrange
        var firstRule = new NotCornerRule();
        var secondRule = new IsCornerRule();
        firstRule.SetSuccessor(secondRule);
        var assignment = assignments.FirstOrDefault(x => x.OWPMatchId == 425388);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.OTHER_SPECIAL_PRODUCTS, actual);
    }

    [Fact]
    public void Excute_NotCornerRule_ShouldReturnGOALKICK()
    {
        // Arrange
        var firstRule = new NotCornerRule();
        var secondRule = new IsCornerRule();
        firstRule.SetSuccessor(secondRule);
        var assignment = assignments.FirstOrDefault(x => x.OWPMatchId == 425387);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.GOAL_KICK, actual);
    }

    [Fact]
    public void Excute_IsCornerRule_ShouldReturnFT_CORNER()
    {
        // Arrange
        var firstRule = new NotCornerRule();
        var secondRule = new IsCornerRule();
        firstRule.SetSuccessor(secondRule);
        var assignment = assignments.FirstOrDefault(x => x.OWPMatchId == 428035);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.FT_CORNER, actual);
    }

    [Fact]
    public void Excute_IsCornerRule_ShouldReturnCORNER_PLUS()
    {
        // Arrange
        var firstRule = new NotCornerRule();
        var secondRule = new IsCornerRule();
        firstRule.SetSuccessor(secondRule);
        var assignment = assignments.FirstOrDefault(x => x.OWPMatchId == 425386);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.CORNER_PLUS, actual);
    }
}