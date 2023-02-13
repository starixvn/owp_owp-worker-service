namespace OWP.SumReport.UnitTests.LAReport;

using SumReport.LAReport.Enum;
using SumReport.LAReport.Misc;
using SumReport.LAReport.Models;
using Xunit;

public class MultipleAndSingleMatchesRuleUnitTests
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
                    ProviderName = "BET/TV",
                    OwpLeagueName = "leaguemar27 - CORNERS",
                    MinorBettypeId = -4,
                    OwpMatchId = 428035,
                    ParentMatchId = 428031,
                    EventDateTime = new DateTime(2020, 7, 25, 15, 0, 0)
                },
                new()
                {
                    UserId = 15477090,
                    ProviderName = "MRB",
                    OwpLeagueName = "leaguemar28",
                    MinorBettypeId = -6,
                    OwpMatchId = 428029,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 25, 15, 0, 0)
                },
                new()
                {
                    UserId = 15477090,
                    ProviderName = "FC",
                    OwpLeagueName = "VIETNAM CHAMPIONSHIP U19 QUALIFIERS FANTASY MATCH",
                    MinorBettypeId = null,
                    OwpMatchId = 425385,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 26, 15, 0, 0)
                },
                new()
                {
                    UserId = 15477090,
                    ProviderName = "TV",
                    OwpLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS - CORNERS - 5 mins play",
                    MinorBettypeId = -4,
                    OwpMatchId = 425386,
                    ParentMatchId = 123456,
                    EventDateTime = new DateTime(2020, 7, 26, 15, 0, 0)
                },
                new()
                {
                    UserId = 15477090,
                    ProviderName = "Reporter",
                    OwpLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS - CORNERS - 15 mins play",
                    MinorBettypeId = -100,
                    OwpMatchId = 425387,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 26, 15, 0, 0)
                },
                new()
                {
                    UserId = 15477090,
                    ProviderName = "IPSD/TV",
                    OwpLeagueName = "SABA VIETNAM CHAMPIONSHIP U19 QUALIFIERS - CORNERS - 20 mins play",
                    MinorBettypeId = null,
                    OwpMatchId = 425388,
                    ParentMatchId = null,
                    EventDateTime = new DateTime(2020, 7, 26, 15, 0, 0)
                }
            };

    [Fact]
    public void Execute_EmptyProviderName_ShouldReturnOTHERS()
    {
        // Arrange
        var firstRule = new SingleMatchesRule();
        var assignment = new Assignment { ProviderName = string.Empty };

        // Act
        var actual = firstRule.Execute(assignment);

        // Assert
        Assert.Equal(ReportType.Others, actual);
    }

    [Fact]
    public void Execute_MultipleMatchesRule_ShouldReturnONLY_PROVIDER()
    {
        // Arrange
        var firstRule = new SingleMatchesRule();
        var secondRule = new IsSingleMatchesRule();
        firstRule.NextRule = secondRule;
        var assignment = assignments.FirstOrDefault(x => x.OwpMatchId == 428031);

        // Act
        var actual = firstRule.Execute(assignment);

        // Assert
        Assert.Equal(ReportType.OnlyProvider, actual);
    }

    [Fact]
    public void Execute_MultipleMatchesRule_ShouldReturnPROVIDER_TV()
    {
        // Arrange
        var firstRule = new SingleMatchesRule();
        var secondRule = new IsSingleMatchesRule();
        firstRule.NextRule = secondRule;
        var assignment = assignments.FirstOrDefault(x => x.OwpMatchId == 428035);

        // Act
        var actual = firstRule.Execute(assignment);

        // Assert
        Assert.Equal(ReportType.MixedProviderTvReporter, actual);
    }

    [Fact]
    public void Execute_SingleMatchesRule_ShouldReturnPROVIDER()
    {
        // Arrange
        var firstRule = new SingleMatchesRule();
        var secondRule = new IsSingleMatchesRule();
        firstRule.NextRule = secondRule;
        var assignment = assignments.FirstOrDefault(x => x.OwpMatchId == 425385);

        // Act
        var actual = firstRule.Execute(assignment);

        // Assert
        Assert.Equal(ReportType.OnlyProvider, actual);
    }

    [Fact]
    public void Execute_SingleMatchesRule_ShouldReturnTV()
    {
        // Arrange
        var firstRule = new SingleMatchesRule();
        var secondRule = new IsSingleMatchesRule();
        firstRule.NextRule = secondRule;
        var assignment = assignments.FirstOrDefault(x => x.OwpMatchId == 425386);

        // Act
        var actual = firstRule.Execute(assignment);

        // Assert
        Assert.Equal(ReportType.OnlyTv, actual);
    }

    [Fact]
    public void Execute_SingleMatchesRule_ShouldReturnREPORTER()
    {
        // Arrange
        var firstRule = new SingleMatchesRule();
        var secondRule = new IsSingleMatchesRule();
        firstRule.NextRule = secondRule;
        var assignment = assignments.FirstOrDefault(x => x.OwpMatchId == 425387);

        // Act
        var actual = firstRule.Execute(assignment);

        // Assert
        Assert.Equal(ReportType.OnlyReporter, actual);
    }

    [Fact]
    public void Execute_SingleMatchesRule_ShouldReturnMIXED_PROVIDER_TV_REPORTER()
    {
        // Arrange
        var firstRule = new SingleMatchesRule();
        var secondRule = new IsSingleMatchesRule();
        firstRule.NextRule = secondRule;
        var assignment = assignments.FirstOrDefault(x => x.OwpMatchId == 425388);

        // Act
        var actual = firstRule.Execute(assignment);

        // Assert
        Assert.Equal(ReportType.MixedProviderTvReporter, actual);
    }
}