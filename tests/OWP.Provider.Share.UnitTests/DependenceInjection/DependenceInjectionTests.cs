using Fanex.Data.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using OWP.Provider.Share;
using OWP.Provider.Share.AgentConfirm;
using OWP.Provider.Share.AgentConfirm.Services;

namespace OWP.Provider.Share.UnitTests.DependenceInjection;

public class DependenceInjectionTests
{
    [Fact]
    public void AddProviderServices_ShouldRegisterSyncAgentConfirmHandler()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockRepository = new Mock<IDynamicRepository>();
        services.AddScoped<IDynamicRepository>(_ => mockRepository.Object);

        // Act
        services.AddProviderServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var handler = serviceProvider.GetService<SyncAgentConfirmHandler>();

        handler.Should().NotBeNull();
        handler.Should().BeOfType<SyncAgentConfirmHandlerImp>();
    }

    [Fact]
    public void AddProviderServices_ShouldRegisterRepository()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockRepository = new Mock<IDynamicRepository>();
        services.AddScoped<IDynamicRepository>(_ => mockRepository.Object);

        // Act
        services.AddProviderServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var repository = serviceProvider.GetService<Repository>();

        repository.Should().NotBeNull();
        repository.Should().BeOfType<RepositoryImp>();
    }

    [Fact]
    public void AddProviderServices_ShouldRegisterServicesAsTransient()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockRepository = new Mock<IDynamicRepository>();
        services.AddScoped<IDynamicRepository>(_ => mockRepository.Object);

        // Act
        services.AddProviderServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        // Get services multiple times to verify they are transient
        var handler1 = serviceProvider.GetService<SyncAgentConfirmHandler>();
        var handler2 = serviceProvider.GetService<SyncAgentConfirmHandler>();
        var repository1 = serviceProvider.GetService<Repository>();
        var repository2 = serviceProvider.GetService<Repository>();

        handler1.Should().NotBeSameAs(handler2);
        repository1.Should().NotBeSameAs(repository2);
    }

    [Fact]
    public void AddProviderServices_ShouldReturnSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddProviderServices();

        // Assert
        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddProviderServices_ShouldNotThrowException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var action = () => services.AddProviderServices();
        action.Should().NotThrow();
    }

    [Fact]
    public void AddProviderServices_ShouldRegisterAllRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockRepository = new Mock<IDynamicRepository>();
        services.AddScoped<IDynamicRepository>(_ => mockRepository.Object);

        // Act
        services.AddProviderServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        // Verify all services can be resolved
        serviceProvider.GetService<SyncAgentConfirmHandler>().Should().NotBeNull();
        serviceProvider.GetService<Repository>().Should().NotBeNull();
    }

    [Fact]
    public void AddProviderServices_ShouldAllowMultipleCalls()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockRepository = new Mock<IDynamicRepository>();
        services.AddScoped<IDynamicRepository>(_ => mockRepository.Object);

        // Act
        services.AddProviderServices();
        services.AddProviderServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        // Should still work after multiple calls
        serviceProvider.GetService<SyncAgentConfirmHandler>().Should().NotBeNull();
        serviceProvider.GetService<Repository>().Should().NotBeNull();
    }

    [Fact]
    public void AddProviderServices_ShouldWorkWithExistingServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(); // Add some existing services
        var mockRepository = new Mock<IDynamicRepository>();
        services.AddScoped<IDynamicRepository>(_ => mockRepository.Object);

        // Act
        services.AddProviderServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        // Verify both existing and new services work
        serviceProvider.GetService<ILoggerFactory>().Should().NotBeNull();
        serviceProvider.GetService<SyncAgentConfirmHandler>().Should().NotBeNull();
        serviceProvider.GetService<Repository>().Should().NotBeNull();
    }
}

