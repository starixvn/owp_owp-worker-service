using OWP.ArchiveData.Criteria;

namespace OWP.ArchiveData.UnitTests.Criteria;

public class GetNewUserTests
{
    [Fact]
    public void GetSettingKey_ShouldReturnCorrectValue()
    {
        // Arrange
        var criteria = new GetNewUser();

        // Act
        var result = criteria.GetSettingKey();

        // Assert
        result.Should().Be("OWP_GetNewUser");
    }

    [Fact]
    public void IsValid_ShouldAlwaysReturnTrue()
    {
        // Arrange
        var criteria = new GetNewUser();

        // Act
        var result = criteria.IsValid();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Act
        var criteria = new GetNewUser();

        // Assert
        criteria.Should().NotBeNull();
    }
}
