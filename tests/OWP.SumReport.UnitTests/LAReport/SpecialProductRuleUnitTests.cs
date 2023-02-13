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
                    MinorBettypeId = -6,
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
                    MinorBettypeId = -4,
                    OwpMatchId = 425386,
                    ParentMatchId = 123456,
                    EventDateTime = new DateTime(2020, 7, 26, 15, 0, 0)
                },
                new()
                {
                    UserId = 15477090,
                    ProviderName = "IPSD",
                    OwpLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS - CORNERS - 15 mins play",
                    MinorBettypeId = -100,
                    OwpMatchId = 425387,
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
    public void Excute_CornerRule_ShouldReturnFT_CORNER_5ML()
    {
        // Arrange
        var firstRule = new NotCornerRule();
        var secondRule = new IsCornerRule();
        firstRule.SetSuccessor(secondRule);
        var assignment = assignments.FirstOrDefault(x => x.OwpMatchId == 427128);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.FtCorner5Ml, actual);
    }
    [Fact]
    public void Excute_NotCornerRule_ShouldReturnTOTAL_BOOKING_PLUS()
    {
        // Arrange
        var firstRule = new NotCornerRule();
        var secondRule = new IsCornerRule();
        firstRule.SetSuccessor(secondRule);
        var assignment = assignments.FirstOrDefault(x => x.OwpMatchId == 428029);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.TotalBookingPlus, actual);
    }

    [Fact]
    public void Excute_NotCornerRule_ShouldReturnOTHER_SPECIAL_PRODUCTS()
    {
        // Arrange
        var firstRule = new NotCornerRule();
        var secondRule = new IsCornerRule();
        firstRule.SetSuccessor(secondRule);
        var assignment = assignments.FirstOrDefault(x => x.OwpMatchId == 425388);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.OtherSpecialProducts, actual);
    }

    [Fact]
    public void Excute_NotCornerRule_ShouldReturnGOALKICK()
    {
        // Arrange
        var firstRule = new NotCornerRule();
        var secondRule = new IsCornerRule();
        firstRule.SetSuccessor(secondRule);
        var assignment = assignments.FirstOrDefault(x => x.OwpMatchId == 425387);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.GoalKick, actual);
    }

    [Fact]
    public void Excute_IsCornerRule_ShouldReturnFT_CORNER()
    {
        // Arrange
        var firstRule = new NotCornerRule();
        var secondRule = new IsCornerRule();
        firstRule.SetSuccessor(secondRule);
        var assignment = assignments.FirstOrDefault(x => x.OwpMatchId == 428035);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.FtCorner, actual);
    }

    [Fact]
    public void Excute_IsCornerRule_ShouldReturnCORNER_PLUS()
    {
        // Arrange
        var firstRule = new NotCornerRule();
        var secondRule = new IsCornerRule();
        firstRule.SetSuccessor(secondRule);
        var assignment = assignments.FirstOrDefault(x => x.OwpMatchId == 425386);

        // Act
        var actual = firstRule.Execute(assignment, assignments);

        // Assert
        Assert.Equal(ReportType.CornerPlus, actual);
    }
}