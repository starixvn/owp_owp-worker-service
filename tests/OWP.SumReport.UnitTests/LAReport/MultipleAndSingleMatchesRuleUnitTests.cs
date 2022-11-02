using OWP.SumReport.LAReport.Enum;
using OWP.SumReport.LAReport.Misc;
using OWP.SumReport.LAReport.Models;
using Xunit;

namespace OWP.SumReport.UnitTests.LAReport;

public class MultipleAndSingleMatchesRuleUnitTests
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
                    ProviderName = "BET/TV",
                    OWPLeagueName = "leaguemar27 - CORNERS",
                    MinorBettypeId = -4,
                    OWPMatchId = 428035,
                    ParentMatchId = 428031,
                    EventDateTime = new DateTime(2020, 7, 25, 15, 0, 0)
                },
                new Assignment
                {
                    UserId = 15477090,
                    ProviderName = "MRB",
                    OWPLeagueName = "leaguemar28",
                    MinorBettypeId = -6,
                    OWPMatchId = 428029,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 25, 15, 0, 0)
                },
                new Assignment
                {
                    UserId = 15477090,
                    ProviderName = "FC",
                    OWPLeagueName = "VIETNAM CHAMPIONSHIP U19 QUALIFIERS FANTASY MATCH",
                    MinorBettypeId = null,
                    OWPMatchId = 425385,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 26, 15, 0, 0)
                },
                new Assignment
                {
                    UserId = 15477090,
                    ProviderName = "TV",
                    OWPLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS - CORNERS - 5 mins play",
                    MinorBettypeId = -4,
                    OWPMatchId = 425386,
                    ParentMatchId = 123456,
                    EventDateTime = new DateTime(2020, 7, 26, 15, 0, 0)
                },
                new Assignment
                {
                    UserId = 15477090,
                    ProviderName = "Reporter",
                    OWPLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS - CORNERS - 15 mins play",
                    MinorBettypeId = -100,
                    OWPMatchId = 425387,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 26, 15, 0, 0)
                },
                new Assignment
                {
                    UserId = 15477090,
                    ProviderName = "IPSD/TV",
                    OWPLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS - CORNERS - 20 mins play",
                    MinorBettypeId = null,
                    OWPMatchId = 425388,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 26, 15, 0, 0)
                }
            };

    [Fact]
    public void Excute_EmptyProviderName_ShouldReturnOTHERS()
    {
        // Arrange
        var firstRule = new SingleMatchesRule();
        var secondRule = new IsSingleMatchesRule();
        var assignment = new Assignment { ProviderName = string.Empty };

        // Act
        var actual = firstRule.Execute(assignment);

        // Assert
        Assert.Equal(ReportType.OTHERS, actual);
    }

    [Fact]
    public void Excute_MultipleMatchesRule_ShouldReturnONLY_PROVIDER()
    {
        // Arrange
        var firstRule = new SingleMatchesRule();
        var secondRule = new IsSingleMatchesRule();
        firstRule.NextRule = secondRule;
        var assignment = assignments.FirstOrDefault(x => x.OWPMatchId == 428031);

        // Act
        var actual = firstRule.Execute(assignment);

        // Assert
        Assert.Equal(ReportType.ONLY_PROVIDER, actual);
    }

    [Fact]
    public void Excute_MultipleMatchesRule_ShouldReturnPROVIDER_TV()
    {
        // Arrange
        var firstRule = new SingleMatchesRule();
        var secondRule = new IsSingleMatchesRule();
        firstRule.NextRule = secondRule;
        var assignment = assignments.FirstOrDefault(x => x.OWPMatchId == 428035);

        // Act
        var actual = firstRule.Execute(assignment);

        // Assert
        Assert.Equal(ReportType.MIXED_PROVIDER_TV_REPORTER, actual);
    }

    [Fact]
    public void Excute_SingleMatchesRule_ShouldReturnPROVIDER()
    {
        // Arrange
        var firstRule = new SingleMatchesRule();
        var secondRule = new IsSingleMatchesRule();
        firstRule.NextRule = secondRule;
        var assignment = assignments.FirstOrDefault(x => x.OWPMatchId == 425385);

        // Act
        var actual = firstRule.Execute(assignment);

        // Assert
        Assert.Equal(ReportType.ONLY_PROVIDER, actual);
    }

    [Fact]
    public void Excute_SingleMatchesRule_ShouldReturnTV()
    {
        // Arrange
        var firstRule = new SingleMatchesRule();
        var secondRule = new IsSingleMatchesRule();
        firstRule.NextRule = secondRule;
        var assignment = assignments.FirstOrDefault(x => x.OWPMatchId == 425386);

        // Act
        var actual = firstRule.Execute(assignment);

        // Assert
        Assert.Equal(ReportType.ONLY_TV, actual);
    }

    [Fact]
    public void Excute_SingleMatchesRule_ShouldReturnREPORTER()
    {
        // Arrange
        var firstRule = new SingleMatchesRule();
        var secondRule = new IsSingleMatchesRule();
        firstRule.NextRule = secondRule;
        var assignment = assignments.FirstOrDefault(x => x.OWPMatchId == 425387);

        // Act
        var actual = firstRule.Execute(assignment);

        // Assert
        Assert.Equal(ReportType.ONLY_REPORTER, actual);
    }

    [Fact]
    public void Excute_SingleMatchesRule_ShouldReturnMIXED_PROVIDER_TV_REPORTER()
    {
        // Arrange
        var firstRule = new SingleMatchesRule();
        var secondRule = new IsSingleMatchesRule();
        firstRule.NextRule = secondRule;
        var assignment = assignments.FirstOrDefault(x => x.OWPMatchId == 425388);

        // Act
        var actual = firstRule.Execute(assignment);

        // Assert
        Assert.Equal(ReportType.MIXED_PROVIDER_TV_REPORTER, actual);
    }
}