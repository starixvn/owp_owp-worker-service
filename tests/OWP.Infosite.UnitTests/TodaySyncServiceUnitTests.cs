namespace OWP.Infosite.UnitTests;

using Criteria;
using Fanex.Data.Repository;
using Models;
using NSubstitute;
using Services;
using Xunit;

public class TodaySyncServiceUnitTests
{
    private readonly ApiService apiService;

    private readonly DateTimeService dateTimeService;

    private readonly IDynamicRepository repository;

    private readonly UpdateInfositeIdService updateInfositeIdService;
    private readonly EventPickingLogsService eventPickingLogsService;
    private readonly ProviderService providerService;

    private readonly DateTime from = new(2018, 1, 1);

    public TodaySyncServiceUnitTests()
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
    public async Task Sync_Always_UpdateMatchToRepository()
    {
        // Arrange
        var syncService = new TodaySyncServiceImp(dateTimeService, apiService, repository, updateInfositeIdService, eventPickingLogsService, providerService);

        // Act
        await syncService.Sync();

        // Assert
        await repository.Received()
            .ExecuteAsync(Arg.Compat.Is<PushInfoSiteMatchCommand>(x
                => x.Xml.Contains("Test League")
                    && x.Xml.Contains("Test Home")
                    && x.Xml.Contains("Test Away")));
    }
}