namespace OWP.MatchReadyNotification.UnitTests;

using Fanex.Caching;
using Fanex.Data.Repository;
using NSubstitute;
using Services;
using Xunit;

public class HandlerIntegrationUnitTests
{
    private readonly IDynamicRepository repository;
    private readonly ICache cache;

    public HandlerIntegrationUnitTests()
    {
        repository = Substitute.For<IDynamicRepository>();
        cache = Substitute.For<ICache>();
    }

    private HandlerImp Create()
    {
        var matchReadyRepository = new MatchReadyRepositoryImp(repository);
        var receivedUserHelper = new ReceivedUserServicesImp(matchReadyRepository);
        var cacheManipulator = new CacheManipulatorImp(cache);
        var messageFactory = new NotReadyMessageFactoryImp(receivedUserHelper);

        var services = new HandlerImp(matchReadyRepository, cacheManipulator, messageFactory);

        return services;
    }
}