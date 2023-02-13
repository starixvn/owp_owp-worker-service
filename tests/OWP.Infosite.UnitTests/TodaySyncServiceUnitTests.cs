using Fanex.Caching;
using Fanex.Data.Repository;
using NSubstitute;
using OWP.RabbitMq;
using OWP.Infosite.Criteria;
using OWP.Infosite.Models;
using OWP.Infosite.Services;
using Xunit;

namespace OWP.Infosite.UnitTests;

public class TodaySyncServiceUnitTests
{
    private readonly ApiService apiService;

    private readonly DateTimeService dateTimeService;

    private readonly IDynamicRepository repository;

    private readonly UpdateInfositeIdService updateInfositeIdService;

    private readonly ICache cacheService;

    private readonly RabbitMqClient rabbitMqClient;

    private readonly DateTime from = new DateTime(2018, 1, 1);

    public TodaySyncServiceUnitTests()
    {
        apiService = Substitute.For<ApiService>();
        repository = Substitute.For<IDynamicRepository>();
        dateTimeService = Substitute.For<DateTimeService>();
        updateInfositeIdService = Substitute.For<UpdateInfositeIdService>();
        cacheService = Substitute.For<ICache>();
        rabbitMqClient = Substitute.For<RabbitMqClient>();

        apiService.QueryAsync(Arg.Compat.Any<ParamsQueryModel>())
           .Returns(SampleDataTests.ListCommonMatches);

        dateTimeService.Today.Returns(from);
        dateTimeService.Now.Returns(from);
    }

    [Fact]
    public async Task Sync_Always_UpdateMatchToRepository()
    {
        // Arrange
        var syncService = new TodaySyncServiceImp(dateTimeService, rabbitMqClient, apiService, repository, cacheService, updateInfositeIdService);

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