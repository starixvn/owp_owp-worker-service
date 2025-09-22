namespace OWP.Infosite.UnitTests;

using Fanex.Data.Repository;
using NSubstitute;
using OWP.Infosite.Services;
using Xunit;

public class Sync180DayInfositeMatchServiceUnitTests
{
    private readonly DateTime from = new(2018, 1, 1);

    [Fact]
    public async Task Sync_GetDataForSevenFutureDay()
    {
        // Arrange
        var apiService = Substitute.For<ApiService>();
        var repository = Substitute.For<IDynamicRepository>();
        var dateTimeService = Substitute.For<DateTimeService>();
        var eventPickingLogsService = Substitute.For<EventPickingLogsService>();

        apiService.QueryInfoSiteMatchFor180Day()
           .Returns(SampleDataTests.ListMatchModelQuery180Day);

        dateTimeService.Today.Returns(from);
        dateTimeService.Now.Returns(from);
        var syncService = new Sync180DayInfositeMatchServiceImp(apiService, repository, eventPickingLogsService);

        // Act
        await syncService.Sync();

        // Assert
        await apiService.Received(1).QueryInfoSiteMatchFor180Day();
    }
}