namespace OWP.Infosite.UnitTests;

using Fanex.Data.Repository;
using Models;
using NSubstitute;
using RabbitMq;
using Services;
using Xunit;

public class FutureSyncServiceUnitTests
{
    private readonly ApiService apiService;

    private readonly DateTimeService dateTimeService;

    private readonly IDynamicRepository repository;

    private readonly UpdateInfositeIdService updateInfositeIdService;

    private readonly RabbitMqClient rabbitMqClient;

    private readonly DateTime from = new(2018, 1, 1);

    public FutureSyncServiceUnitTests()
    {
        apiService = Substitute.For<ApiService>();
        repository = Substitute.For<IDynamicRepository>();
        dateTimeService = Substitute.For<DateTimeService>();
        updateInfositeIdService = Substitute.For<UpdateInfositeIdService>();
        rabbitMqClient = Substitute.For<RabbitMqClient>();

        apiService.QueryAsync(Arg.Compat.Any<ParamsQueryModel>())
           .Returns(SampleDataTests.ListCommonMatches);

        dateTimeService.Today.Returns(from);
        dateTimeService.Now.Returns(from);
    }

    [Fact]
    public async Task Sync_GetDataForSevenFutureDay()
    {
        // Arrange
        var syncService = new FutureSyncServiceImp(dateTimeService, rabbitMqClient, apiService, repository, updateInfositeIdService);

        // Act
        await syncService.Sync();

        // Assert
        await apiService.Received(10).QueryAsync(Arg.Compat.Any<ParamsQueryModel>());
    }
}