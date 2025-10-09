using Fanex.Data.Repository;
using NSubstitute;
using OWP.ArchiveData;

namespace OWP.ArchiveData.UnitTests.Repository;

public class RepositoryTests
{
    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Arrange
        var mockRepository = Substitute.For<IDynamicRepository>();

        // Act
        var repository = new RepositoryImp(mockRepository);

        // Assert
        repository.Should().NotBeNull();
    }

    [Fact]
    public void CopyDataToOldTable_ShouldBeCallable()
    {
        // Arrange
        var mockRepository = Substitute.For<IDynamicRepository>();
        var repository = new RepositoryImp(mockRepository);
        var date = new DateTime(2023, 1, 1);

        // Act & Assert
        // This test verifies the method signature is correct
        var method = typeof(RepositoryImp).GetMethod("CopyDataToOldTable");
        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(Task));
    }

    [Fact]
    public void ArchiveOldMessage_ShouldBeCallable()
    {
        // Arrange
        var mockRepository = Substitute.For<IDynamicRepository>();
        var repository = new RepositoryImp(mockRepository);
        var date = new DateTime(2023, 1, 1);

        // Act & Assert
        // This test verifies the method signature is correct
        var method = typeof(RepositoryImp).GetMethod("ArchiveOldMessage");
        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(Task));
    }

    [Fact]
    public void DeleteOldNoOfferMatch_ShouldBeCallable()
    {
        // Arrange
        var mockRepository = Substitute.For<IDynamicRepository>();
        var repository = new RepositoryImp(mockRepository);
        var date = new DateTime(2023, 1, 1);

        // Act & Assert
        // This test verifies the method signature is correct
        var method = typeof(RepositoryImp).GetMethod("DeleteOldNoOfferMatch");
        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(Task));
    }

    [Fact]
    public void DeleteOldData_ShouldBeCallable()
    {
        // Arrange
        var mockRepository = Substitute.For<IDynamicRepository>();
        var repository = new RepositoryImp(mockRepository);
        var date = new DateTime(2023, 1, 1);

        // Act & Assert
        // This test verifies the method signature is correct
        var method = typeof(RepositoryImp).GetMethod("DeleteOldData");
        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(Task));
    }

    [Fact]
    public void DeleteOldNotify_ShouldBeCallable()
    {
        // Arrange
        var mockRepository = Substitute.For<IDynamicRepository>();
        var repository = new RepositoryImp(mockRepository);
        var date = new DateTime(2023, 1, 1);

        // Act & Assert
        // This test verifies the method signature is correct
        var method = typeof(RepositoryImp).GetMethod("DeleteOldNotify");
        method.Should().NotBeNull();
        method!.ReturnType.Should().Be(typeof(Task));
    }

    [Fact]
    public async Task DeleteOldData_WithInvalidDate_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var mockRepository = Substitute.For<IDynamicRepository>();
        var repository = new RepositoryImp(mockRepository);
        var date = DateTime.Today.AddDays(-30); // Invalid date (not old enough)

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            repository.DeleteOldData(date));
    }

    [Fact]
    public async Task DeleteOldNotify_WithInvalidDate_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var mockRepository = Substitute.For<IDynamicRepository>();
        var repository = new RepositoryImp(mockRepository);
        var date = DateTime.Today.AddDays(-30); // Invalid date (not old enough)

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            repository.DeleteOldNotify(date));
    }

    [Theory]
    [InlineData(-34)] // Just under the limit
    [InlineData(-30)] // Well under the limit
    [InlineData(0)] // Today
    [InlineData(10)] // Future date
    public async Task DeleteOldData_WithVariousInvalidDates_ShouldThrowArgumentOutOfRangeException(int daysOffset)
    {
        // Arrange
        var mockRepository = Substitute.For<IDynamicRepository>();
        var repository = new RepositoryImp(mockRepository);
        var date = DateTime.Today.AddDays(daysOffset);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            repository.DeleteOldData(date));
    }

    [Theory]
    [InlineData(-34)] // Just under the limit
    [InlineData(-30)] // Well under the limit
    [InlineData(0)] // Today
    [InlineData(10)] // Future date
    public async Task DeleteOldNotify_WithVariousInvalidDates_ShouldThrowArgumentOutOfRangeException(int daysOffset)
    {
        // Arrange
        var mockRepository = Substitute.For<IDynamicRepository>();
        var repository = new RepositoryImp(mockRepository);
        var date = DateTime.Today.AddDays(daysOffset);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            repository.DeleteOldNotify(date));
    }
}
