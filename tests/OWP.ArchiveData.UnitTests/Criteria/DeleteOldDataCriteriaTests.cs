using OWP.ArchiveData.Criteria;

namespace OWP.ArchiveData.UnitTests.Criteria;

public class DeleteOldDataCriteriaTests
{
    [Fact]
    public void GetSettingKey_ShouldReturnCorrectValue()
    {
        // Arrange
        var criteria = new DeleteOldDataCriteria();

        // Act
        var result = criteria.GetSettingKey();

        // Assert
        result.Should().Be("OWP_DeleteOldData");
    }

    [Fact]
    public void IsValid_ShouldAlwaysReturnTrue()
    {
        // Arrange
        var criteria = new DeleteOldDataCriteria();

        // Act
        var result = criteria.IsValid();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Date_ShouldBeSettable()
    {
        // Arrange
        var criteria = new DeleteOldDataCriteria();
        var testDate = new DateTime(2023, 1, 1, 10, 30, 0);

        // Act
        criteria.Date = testDate;

        // Assert
        criteria.Date.Should().Be(testDate);
    }

    [Theory]
    [InlineData("2023-01-01 00:00:00")]
    [InlineData("2023-12-31 23:59:59")]
    [InlineData("2024-02-29 12:00:00")] // Leap year
    public void Date_ShouldHandleVariousDateTimeValues(string dateTimeStr)
    {
        // Arrange
        var criteria = new DeleteOldDataCriteria();
        var expectedDate = DateTime.Parse(dateTimeStr);

        // Act
        criteria.Date = expectedDate;

        // Assert
        criteria.Date.Should().Be(expectedDate);
    }
}
