using OWP.Infosite.Services;

namespace OWP.Infosite.UnitTests;

using Fanex.Data.Repository;
using Models;
using NSubstitute;
using Services;
using Xunit;

public class FutureSyncServiceUnitTests
{
    private readonly ApiService apiService;

    private readonly DateTimeService dateTimeService;

    private readonly IDynamicRepository repository;

    private readonly UpdateInfositeIdService updateInfositeIdService;
    private readonly EventPickingLogsService eventPickingLogsService;
    private readonly ProviderService providerService;

    private readonly DateTime from = new(2018, 1, 1);

    public FutureSyncServiceUnitTests()
    {
        apiService = Substitute.For<ApiService>();
        repository = Substitute.For<IDynamicRepository>();
        dateTimeService = Substitute.For<DateTimeService>();
        updateInfositeIdService = Substitute.For<UpdateInfositeIdService>();
        eventPickingLogsService = Substitute.For<EventPickingLogsService>();
        providerService = Substitute.For<ProviderService>();

        apiService.QueryAsync(Arg.Compat.Any<ParamsQueryModel>())
           .Returns(SampleDataTests.ListCommonMatches);

        dateTimeService.Today.Returns(from);
        dateTimeService.Now.Returns(from);
    }

    [Fact]
    public async Task Sync_GetDataForSevenFutureDay()
    {
        // Arrange
        var syncService = new FutureSyncServiceImp(dateTimeService, apiService, repository, updateInfositeIdService, eventPickingLogsService, providerService);

        // Act
        await syncService.Sync();

        // Assert
        await apiService.Received(10).QueryAsync(Arg.Compat.Any<ParamsQueryModel>());
    }
}