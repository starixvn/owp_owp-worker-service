using Fanex.Data.Repository;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OWP.ArchiveData.Services;

namespace OWP.ArchiveData.UnitTests.DependenceInjection;

public class DependenceInjectionTests
{
    [Fact]
    public void AddArchiveDataServices_ShouldRegisterRepository()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockRepository = Substitute.For<IDynamicRepository>();
        services.AddScoped<IDynamicRepository>(_ => mockRepository);

        // Act
        services.AddArchiveDataServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var repository = serviceProvider.GetService<OWP.ArchiveData.Repository>();

        repository.Should().NotBeNull();
        repository.Should().BeOfType<OWP.ArchiveData.RepositoryImp>();
    }

    [Fact]
    public void AddArchiveDataServices_ShouldRegisterArchiveDataService()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockRepository = Substitute.For<IDynamicRepository>();
        services.AddScoped<IDynamicRepository>(_ => mockRepository);

        // Act
        services.AddArchiveDataServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var archiveDataService = serviceProvider.GetService<ArchiveDataService>();

        archiveDataService.Should().NotBeNull();
        archiveDataService.Should().BeOfType<ArchiveDataServiceImp>();
    }

    [Fact]
    public void AddArchiveDataServices_ShouldRegisterAllRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockRepository = Substitute.For<IDynamicRepository>();
        services.AddScoped<IDynamicRepository>(_ => mockRepository);

        // Act
        services.AddArchiveDataServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        serviceProvider.GetService<OWP.ArchiveData.Repository>().Should().NotBeNull();
        serviceProvider.GetService<ArchiveDataService>().Should().NotBeNull();
    }

    [Fact]
    public void AddArchiveDataServices_ShouldRegisterServicesAsTransient()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockRepository = Substitute.For<IDynamicRepository>();
        services.AddScoped<IDynamicRepository>(_ => mockRepository);

        // Act
        services.AddArchiveDataServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        // Get services multiple times to verify they are transient
        var repository1 = serviceProvider.GetService<OWP.ArchiveData.Repository>();
        var repository2 = serviceProvider.GetService<OWP.ArchiveData.Repository>();
        var service1 = serviceProvider.GetService<ArchiveDataService>();
        var service2 = serviceProvider.GetService<ArchiveDataService>();

        repository1.Should().NotBeSameAs(repository2);
        service1.Should().NotBeSameAs(service2);
    }

    [Fact]
    public void AddArchiveDataServices_ShouldAllowMultipleCalls()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockRepository = Substitute.For<IDynamicRepository>();
        services.AddScoped<IDynamicRepository>(_ => mockRepository);

        // Act
        services.AddArchiveDataServices();
        services.AddArchiveDataServices(); // Call multiple times

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        serviceProvider.GetService<OWP.ArchiveData.Repository>().Should().NotBeNull();
        serviceProvider.GetService<ArchiveDataService>().Should().NotBeNull();
    }

    [Fact]
    public void AddArchiveDataServices_ShouldWorkWithExistingServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(); // Add some existing services
        var mockRepository = Substitute.For<IDynamicRepository>();
        services.AddScoped<IDynamicRepository>(_ => mockRepository);

        // Act
        services.AddArchiveDataServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        // Verify both existing and new services work
        serviceProvider.GetService<Microsoft.Extensions.Logging.ILoggerFactory>().Should().NotBeNull();
        serviceProvider.GetService<OWP.ArchiveData.Repository>().Should().NotBeNull();
        serviceProvider.GetService<ArchiveDataService>().Should().NotBeNull();
    }

    [Fact]
    public void AddArchiveDataServices_ShouldReturnSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockRepository = Substitute.For<IDynamicRepository>();
        services.AddScoped<IDynamicRepository>(_ => mockRepository);

        // Act
        var result = services.AddArchiveDataServices();

        // Assert
        result.Should().BeSameAs(services);
    }
}
