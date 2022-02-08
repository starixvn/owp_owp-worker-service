using Fanex.Data.Repository;
using Fanex.Logging;
using NSubstitute;
using OWP.Infosite.Criteria;
using OWP.Infosite.Services;
using Xunit;

namespace OWP.Infosite.UnitTests;

public class EventDeletedSyncServiceUnitTests
{
    private readonly ApiService apiService;

    private readonly DateTimeService dateTimeService;

    private readonly IDynamicRepository repository;

    private readonly ILogger logger;

    private readonly DateTime from = new DateTime(2018, 1, 1);

    public EventDeletedSyncServiceUnitTests()
    {
        apiService = Substitute.For<ApiService>();
        repository = Substitute.For<IDynamicRepository>();
        dateTimeService = Substitute.For<DateTimeService>();
        logger = Substitute.For<ILogger>();

        apiService.GetInvalidEventKeysByDateAsync(Arg.Compat.Any<DateTime>())
           .Returns(new List<string> { "1-1-True", "2-3-True", "3-3-False" });

        dateTimeService.Today.Returns(from);
        dateTimeService.Now.Returns(from);
    }

    [Fact]
    public async Task Sync_FirstTime_SyncEventDeleted()
    {
        // Arrange
        var syncService = new SyncEventDeletedServiceImp(dateTimeService, apiService, repository, logger);

        // Act
        await syncService.Sync();

        // Assert
        await apiService.Received()
            .GetInvalidEventKeysByDateAsync(Arg.Compat.Any<DateTime>());
    }

    [Fact]
    public async Task Sync_Always_UpdateMatchToRepository()
    {
        // Arrange
        var syncService = new SyncEventDeletedServiceImp(dateTimeService, apiService, repository, logger);

        // Act
        await syncService.Sync();

        // Assert
        await repository
            .Received()
            .ExecuteAsync(Arg.Compat.Is<SyncEventDeletedInfoSiteMatchCriteria>(x => x.Ids.Contains("1") && x.Ids.Contains("2") && x.Ids.Contains("3")));
    }
}