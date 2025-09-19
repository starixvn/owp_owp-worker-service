using OWP.AutoGenerateMaxbetSetting.Criteria;

namespace OWP.AutoGenerateMaxbetSetting.UnitTests.Criteria;

public class GetMatchesCriteriaTests
{
    [Fact]
    public void Constructor_ShouldCreateInstanceWithDefaultValues()
    {
        // Act
        var criteria = new GetMatchesCriteria();

        // Assert
        criteria.Should().NotBeNull();
        criteria.EventDate.Should().Be(default(DateTime));
    }

    [Fact]
    public void EventDate_ShouldSetAndGetCorrectValue()
    {
        // Arrange
        var criteria = new GetMatchesCriteria();
        var eventDate = new DateTime(2024, 1, 15, 14, 30, 0);

        // Act
        criteria.EventDate = eventDate;

        // Assert
        criteria.EventDate.Should().Be(eventDate);
    }

    [Fact]
    public void GetSettingKey_ShouldReturnCorrectValue()
    {
        // Arrange
        var criteria = new GetMatchesCriteria();

        // Act
        var result = criteria.GetSettingKey();

        // Assert
        result.Should().Be("OWP_NewLeagueSetting_GetNoSettingsMatches");
    }

    [Fact]
    public void IsValid_ShouldAlwaysReturnTrue()
    {
        // Arrange
        var criteria = new GetMatchesCriteria();

        // Act
        var result = criteria.IsValid();

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("2024-01-15 14:30:00")]
    [InlineData("2024-01-01 00:00:00")]
    [InlineData("2024-12-31 23:59:59")]
    public void IsValid_WithDifferentDates_ShouldAlwaysReturnTrue(string eventDateStr)
    {
        // Arrange
        var criteria = new GetMatchesCriteria
        {
            EventDate = DateTime.Parse(eventDateStr)
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ObjectInitializer_ShouldWorkCorrectly()
    {
        // Arrange
        var eventDate = new DateTime(2024, 1, 15, 14, 30, 0);

        // Act
        var criteria = new GetMatchesCriteria
        {
            EventDate = eventDate
        };

        // Assert
        criteria.EventDate.Should().Be(eventDate);
        criteria.GetSettingKey().Should().Be("OWP_NewLeagueSetting_GetNoSettingsMatches");
        criteria.IsValid().Should().BeTrue();
    }
}
