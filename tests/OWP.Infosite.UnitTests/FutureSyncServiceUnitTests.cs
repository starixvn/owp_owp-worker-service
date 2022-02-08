using Fanex.Caching;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using NSubstitute;
using OWP.Infosite.Models;
using OWP.Infosite.Services;
using OWP.Infosite.Settings;
using Xunit;

namespace OWP.Infosite.UnitTests;

public class FutureSyncServiceUnitTests
{
    private readonly IOptions<InfoSiteSettings> appSettings;

    private readonly ApiService apiService;

    private readonly DateTimeService dateTimeService;

    private readonly IDynamicRepository repository;

    private readonly UpdateInfositeIdService updateInfositeIdService;

    private readonly ICache cacheService;

    private readonly DateTime from = new DateTime(2018, 1, 1);

    private readonly DateTime furetureFrom = new DateTime(2018, 1, 3);

    private readonly DateTime furetureTo = new DateTime(2018, 1, 10);

    private readonly DateTime minDate = new DateTime(2000, 1, 1);

    public FutureSyncServiceUnitTests()
    {
        appSettings = Options.Create(new InfoSiteSettings());
        apiService = Substitute.For<ApiService>();
        repository = Substitute.For<IDynamicRepository>();
        dateTimeService = Substitute.For<DateTimeService>();
        updateInfositeIdService = Substitute.For<UpdateInfositeIdService>();
        cacheService = Substitute.For<ICache>();

        apiService.QueryAsync(Arg.Compat.Any<ParamsQueryModel>())
           .Returns(SampleDataTests.ListCommonMatches);

        dateTimeService.Today.Returns(from);
        dateTimeService.Now.Returns(from);
    }

    [Fact]
    public async Task Sync_GetDataForSevenFutureDay()
    {
        // Arrange
        var syncService = new FutureSyncServiceImp(dateTimeService, apiService, repository, updateInfositeIdService);

        // Act
        await syncService.Sync();

        // Assert
        await apiService.Received(10).QueryAsync(Arg.Compat.Any<ParamsQueryModel>());
    }
}