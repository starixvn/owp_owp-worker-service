using OWP.AutoGenerateMaxbetSetting.Criteria;

namespace OWP.AutoGenerateMaxbetSetting.UnitTests.Criteria;

public class InsertNewLeagueSettingsCriteriaTests
{
    [Fact]
    public void Constructor_ShouldCreateInstanceWithDefaultValues()
    {
        // Act
        var criteria = new InsertNewLeagueSettingsCriteria();

        // Assert
        criteria.Should().NotBeNull();
        criteria.LeagueSettings.Should().BeNull();
    }

    [Theory]
    [InlineData("test data")]
    [InlineData("")]
    [InlineData("{\"test\": \"json\"}")]
    [InlineData("xml data")]
    public void LeagueSettings_ShouldSetAndGetCorrectValue(string leagueSettings)
    {
        // Arrange
        var criteria = new InsertNewLeagueSettingsCriteria();

        // Act
        criteria.LeagueSettings = leagueSettings;

        // Assert
        criteria.LeagueSettings.Should().Be(leagueSettings);
    }

    [Fact]
    public void GetSettingKey_ShouldReturnCorrectValue()
    {
        // Arrange
        var criteria = new InsertNewLeagueSettingsCriteria();

        // Act
        var result = criteria.GetSettingKey();

        // Assert
        result.Should().Be("OWP_NewLeagueSetting_LeagueSettingInsert");
    }

    [Theory]
    [InlineData("test data", true)]
    [InlineData("", false)]
    [InlineData("{\"test\": \"json\"}", true)]
    public void IsValid_ShouldReturnCorrectResult(string leagueSettings, bool expected)
    {
        // Arrange
        var criteria = new InsertNewLeagueSettingsCriteria
        {
            LeagueSettings = leagueSettings
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ObjectInitializer_ShouldWorkCorrectly()
    {
        // Arrange
        var leagueSettings = "test league settings data";

        // Act
        var criteria = new InsertNewLeagueSettingsCriteria
        {
            LeagueSettings = leagueSettings
        };

        // Assert
        criteria.LeagueSettings.Should().Be(leagueSettings);
        criteria.GetSettingKey().Should().Be("OWP_NewLeagueSetting_LeagueSettingInsert");
        criteria.IsValid().Should().BeTrue();
    }
}
