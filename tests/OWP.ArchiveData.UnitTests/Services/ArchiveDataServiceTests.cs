using NSubstitute;
using OWP.ArchiveData.Services;

namespace OWP.ArchiveData.UnitTests.Services;

public class ArchiveDataServiceTests
{
    private readonly OWP.ArchiveData.Repository _mockRepository;
    private readonly ArchiveDataServiceImp _archiveDataService;

    public ArchiveDataServiceTests()
    {
        _mockRepository = Substitute.For<OWP.ArchiveData.Repository>();
        _archiveDataService = new ArchiveDataServiceImp(_mockRepository);
    }

    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Arrange
        var mockRepository = Substitute.For<OWP.ArchiveData.Repository>();

        // Act
        var service = new ArchiveDataServiceImp(mockRepository);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task CopyDataToOldTable_ShouldCallRepositoryWithCorrectDate()
    {
        // Arrange
        var inputDate = new DateTime(2023, 1, 1);
        var expectedDate = inputDate.AddDays(-35); // 35 days before input date
        _mockRepository.CopyDataToOldTable(Arg.Any<DateTime>())
            .Returns(Task.CompletedTask);

        // Act
        await _archiveDataService.CopyDataToOldTable(inputDate);

        // Assert
        _mockRepository.Received(1).CopyDataToOldTable(expectedDate);
    }

    [Fact]
    public async Task DeleteOldData_ShouldCallRepositoryWithCorrectDate()
    {
        // Arrange
        var inputDate = new DateTime(2023, 1, 1);
        var expectedDate = inputDate.AddDays(-40); // 40 days before input date
        _mockRepository.DeleteOldData(Arg.Any<DateTime>())
            .Returns(Task.CompletedTask);

        // Act
        await _archiveDataService.DeleteOldData(inputDate);

        // Assert
        _mockRepository.Received(1).DeleteOldData(expectedDate);
    }

    [Fact]
    public async Task ArchiveOldMessage_ShouldCallRepositoryWithSameDate()
    {
        // Arrange
        var inputDate = new DateTime(2023, 1, 1);
        _mockRepository.ArchiveOldMessage(Arg.Any<DateTime>())
            .Returns(Task.CompletedTask);

        // Act
        await _archiveDataService.ArchiveOldMessage(inputDate);

        // Assert
        _mockRepository.Received(1).ArchiveOldMessage(inputDate);
    }

    [Fact]
    public async Task DeleteOldNoOfferMatch_ShouldCallRepositoryWithCorrectDate()
    {
        // Arrange
        var inputDate = new DateTime(2023, 1, 1);
        var expectedDate = inputDate.AddDays(-40); // 40 days before input date
        _mockRepository.DeleteOldNoOfferMatch(Arg.Any<DateTime>())
            .Returns(Task.CompletedTask);

        // Act
        await _archiveDataService.DeleteOldNoOfferMatch(inputDate);

        // Assert
        _mockRepository.Received(1).DeleteOldNoOfferMatch(expectedDate);
    }

    [Fact]
    public async Task DeleteOldNotify_ShouldCallRepositoryWithCorrectDate()
    {
        // Arrange
        var inputDate = new DateTime(2023, 1, 1);
        var expectedDate = inputDate.AddDays(-40); // 40 days before input date
        _mockRepository.DeleteOldNotify(Arg.Any<DateTime>())
            .Returns(Task.CompletedTask);

        // Act
        await _archiveDataService.DeleteOldNotify(inputDate);

        // Assert
        _mockRepository.Received(1).DeleteOldNotify(expectedDate);
    }

    [Theory]
    [InlineData("2023-01-01 00:00:00", "2022-11-27 00:00:00")] // 35 days before
    [InlineData("2023-06-15 12:30:45", "2023-05-11 12:30:45")] // 35 days before with time
    [InlineData("2024-02-29 23:59:59", "2024-01-25 23:59:59")] // Leap year
    public async Task CopyDataToOldTable_WithVariousDates_ShouldCalculateCorrectDate(string inputDateStr, string expectedDateStr)
    {
        // Arrange
        var inputDate = DateTime.Parse(inputDateStr);
        var expectedDate = DateTime.Parse(expectedDateStr);
        _mockRepository.CopyDataToOldTable(Arg.Any<DateTime>())
            .Returns(Task.CompletedTask);

        // Act
        await _archiveDataService.CopyDataToOldTable(inputDate);

        // Assert
        _mockRepository.Received(1).CopyDataToOldTable(expectedDate);
    }

    [Theory]
    [InlineData("2023-01-01 00:00:00", "2022-11-22 00:00:00")] // 40 days before
    [InlineData("2023-06-15 12:30:45", "2023-05-06 12:30:45")] // 40 days before with time
    [InlineData("2024-02-29 23:59:59", "2024-01-20 23:59:59")] // Leap year
    public async Task DeleteOldData_WithVariousDates_ShouldCalculateCorrectDate(string inputDateStr, string expectedDateStr)
    {
        // Arrange
        var inputDate = DateTime.Parse(inputDateStr);
        var expectedDate = DateTime.Parse(expectedDateStr);
        _mockRepository.DeleteOldData(Arg.Any<DateTime>())
            .Returns(Task.CompletedTask);

        // Act
        await _archiveDataService.DeleteOldData(inputDate);

        // Assert
        _mockRepository.Received(1).DeleteOldData(expectedDate);
    }

    [Theory]
    [InlineData("2023-01-01 00:00:00", "2022-11-22 00:00:00")] // 40 days before
    [InlineData("2023-06-15 12:30:45", "2023-05-06 12:30:45")] // 40 days before with time
    [InlineData("2024-02-29 23:59:59", "2024-01-20 23:59:59")] // Leap year
    public async Task DeleteOldNoOfferMatch_WithVariousDates_ShouldCalculateCorrectDate(string inputDateStr, string expectedDateStr)
    {
        // Arrange
        var inputDate = DateTime.Parse(inputDateStr);
        var expectedDate = DateTime.Parse(expectedDateStr);
        _mockRepository.DeleteOldNoOfferMatch(Arg.Any<DateTime>())
            .Returns(Task.CompletedTask);

        // Act
        await _archiveDataService.DeleteOldNoOfferMatch(inputDate);

        // Assert
        _mockRepository.Received(1).DeleteOldNoOfferMatch(expectedDate);
    }

    [Theory]
    [InlineData("2023-01-01 00:00:00", "2022-11-22 00:00:00")] // 40 days before
    [InlineData("2023-06-15 12:30:45", "2023-05-06 12:30:45")] // 40 days before with time
    [InlineData("2024-02-29 23:59:59", "2024-01-20 23:59:59")] // Leap year
    public async Task DeleteOldNotify_WithVariousDates_ShouldCalculateCorrectDate(string inputDateStr, string expectedDateStr)
    {
        // Arrange
        var inputDate = DateTime.Parse(inputDateStr);
        var expectedDate = DateTime.Parse(expectedDateStr);
        _mockRepository.DeleteOldNotify(Arg.Any<DateTime>())
            .Returns(Task.CompletedTask);

        // Act
        await _archiveDataService.DeleteOldNotify(inputDate);

        // Assert
        _mockRepository.Received(1).DeleteOldNotify(expectedDate);
    }

    [Fact]
    public async Task ArchiveOldMessage_WithVariousDates_ShouldPassSameDate()
    {
        // Arrange
        var testDates = new[]
        {
            new DateTime(2023, 1, 1, 0, 0, 0),
            new DateTime(2023, 6, 15, 12, 30, 45),
            new DateTime(2024, 2, 29, 23, 59, 59) // Leap year
        };

        _mockRepository.ArchiveOldMessage(Arg.Any<DateTime>())
            .Returns(Task.CompletedTask);

        foreach (var testDate in testDates)
        {
            // Act
            await _archiveDataService.ArchiveOldMessage(testDate);

            // Assert
            _mockRepository.Received(1).ArchiveOldMessage(testDate);
        }
    }
}
