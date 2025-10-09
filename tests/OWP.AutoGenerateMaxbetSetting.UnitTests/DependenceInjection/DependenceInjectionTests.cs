using Fanex.Data.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OWP.AutoGenerateMaxbetSetting;
using OWP.AutoGenerateMaxbetSetting.Services;
using Olympus.OWP.Log.Data.Repository.Services;

namespace OWP.AutoGenerateMaxbetSetting.UnitTests.DependenceInjection;

public class DependenceInjectionTests
{
    [Fact]
    public void AddAutoGenerateNewLeagueSettingServices_ShouldRegisterLeagueSettingsRepository()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockRepository = Substitute.For<IDynamicRepository>();
        var mockLogger = Substitute.For<LogWriterService>();
        services.AddScoped<IDynamicRepository>(_ => mockRepository);
        services.AddScoped<LogWriterService>(_ => mockLogger);

        // Act
        services.AddAutoGenerateNewLeagueSettingServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var repository = serviceProvider.GetService<LeagueSettingsRepository>();

        repository.Should().NotBeNull();
        repository.Should().BeOfType<LeagueSettingsRepositoryImp>();
    }

    [Fact]
    public void AddAutoGenerateNewLeagueSettingServices_ShouldRegisterGeneratorService()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockRepository = Substitute.For<IDynamicRepository>();
        var mockLogger = Substitute.For<LogWriterService>();
        services.AddScoped<IDynamicRepository>(_ => mockRepository);
        services.AddScoped<LogWriterService>(_ => mockLogger);

        // Act
        services.AddAutoGenerateNewLeagueSettingServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var generatorService = serviceProvider.GetService<GeneratorService>();

        generatorService.Should().NotBeNull();
        generatorService.Should().BeOfType<GeneratorServiceImp>();
    }

    [Fact]
    public void AddAutoGenerateNewLeagueSettingServices_ShouldRegisterServicesAsTransient()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockRepository = Substitute.For<IDynamicRepository>();
        var mockLogger = Substitute.For<LogWriterService>();
        services.AddScoped<IDynamicRepository>(_ => mockRepository);
        services.AddScoped<LogWriterService>(_ => mockLogger);

        // Act
        services.AddAutoGenerateNewLeagueSettingServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        // Get services multiple times to verify they are transient
        var repository1 = serviceProvider.GetService<LeagueSettingsRepository>();
        var repository2 = serviceProvider.GetService<LeagueSettingsRepository>();
        var generator1 = serviceProvider.GetService<GeneratorService>();
        var generator2 = serviceProvider.GetService<GeneratorService>();

        repository1.Should().NotBeSameAs(repository2);
        generator1.Should().NotBeSameAs(generator2);
    }

    [Fact]
    public void AddAutoGenerateNewLeagueSettingServices_ShouldReturnSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddAutoGenerateNewLeagueSettingServices();

        // Assert
        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddAutoGenerateNewLeagueSettingServices_ShouldNotThrowException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var action = () => services.AddAutoGenerateNewLeagueSettingServices();
        action.Should().NotThrow();
    }

    [Fact]
    public void AddAutoGenerateNewLeagueSettingServices_ShouldRegisterAllRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockRepository = Substitute.For<IDynamicRepository>();
        var mockLogger = Substitute.For<LogWriterService>();
        services.AddScoped<IDynamicRepository>(_ => mockRepository);
        services.AddScoped<LogWriterService>(_ => mockLogger);

        // Act
        services.AddAutoGenerateNewLeagueSettingServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        // Verify all services can be resolved
        serviceProvider.GetService<LeagueSettingsRepository>().Should().NotBeNull();
        serviceProvider.GetService<GeneratorService>().Should().NotBeNull();
    }

    [Fact]
    public void AddAutoGenerateNewLeagueSettingServices_ShouldAllowMultipleCalls()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockRepository = Substitute.For<IDynamicRepository>();
        var mockLogger = Substitute.For<LogWriterService>();
        services.AddScoped<IDynamicRepository>(_ => mockRepository);
        services.AddScoped<LogWriterService>(_ => mockLogger);

        // Act
        services.AddAutoGenerateNewLeagueSettingServices();
        services.AddAutoGenerateNewLeagueSettingServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        // Should still work after multiple calls
        serviceProvider.GetService<LeagueSettingsRepository>().Should().NotBeNull();
        serviceProvider.GetService<GeneratorService>().Should().NotBeNull();
    }

    [Fact]
    public void AddAutoGenerateNewLeagueSettingServices_ShouldWorkWithExistingServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(); // Add some existing services
        var mockRepository = Substitute.For<IDynamicRepository>();
        var mockLogger = Substitute.For<LogWriterService>();
        services.AddScoped<IDynamicRepository>(_ => mockRepository);
        services.AddScoped<LogWriterService>(_ => mockLogger);

        // Act
        services.AddAutoGenerateNewLeagueSettingServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        // Verify both existing and new services work
        serviceProvider.GetService<ILoggerFactory>().Should().NotBeNull();
        serviceProvider.GetService<LeagueSettingsRepository>().Should().NotBeNull();
        serviceProvider.GetService<GeneratorService>().Should().NotBeNull();
    }
}
