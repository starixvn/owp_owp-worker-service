using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OWP;
using OWP.Services;
using Xunit;

namespace OWP.UnitTests;

public class DependenceInjectionTests
{
    [Fact]
    public void AddOwpCoreServices_ShouldRegisterHttpHandler()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddOwpCoreServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var httpHandler = serviceProvider.GetService<HttpHandler>();

        httpHandler.Should().NotBeNull();
        httpHandler.Should().BeOfType<HttpHandlerImp>();
    }

    [Fact]
    public void AddOwpCoreServices_ShouldRegisterHttpHandlerAsTransient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddOwpCoreServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var httpHandler1 = serviceProvider.GetService<HttpHandler>();
        var httpHandler2 = serviceProvider.GetService<HttpHandler>();

        httpHandler1.Should().NotBeNull();
        httpHandler2.Should().NotBeNull();
        httpHandler1.Should().NotBeSameAs(httpHandler2); // Transient registration
    }

    [Fact]
    public void AddOwpCoreServices_ShouldReturnSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddOwpCoreServices();

        // Assert
        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddOwpCoreServices_ShouldNotRegisterOtherServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddOwpCoreServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        // These services should not be registered by AddOwpCoreServices
        var resiliencePolicy = serviceProvider.GetService<ResiliencePolicy>();
        resiliencePolicy.Should().BeNull();
    }

    [Fact]
    public void AddOwpCoreServices_ShouldAllowMultipleCalls()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddOwpCoreServices();
        services.AddOwpCoreServices(); // Second call

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var httpHandler = serviceProvider.GetService<HttpHandler>();

        httpHandler.Should().NotBeNull();
        httpHandler.Should().BeOfType<HttpHandlerImp>();
    }

    [Fact]
    public void AddOwpCoreServices_WithExistingServices_ShouldNotConflict()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<object>("existing service");

        // Act
        services.AddOwpCoreServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var httpHandler = serviceProvider.GetService<HttpHandler>();
        var existingService = serviceProvider.GetService<object>();

        httpHandler.Should().NotBeNull();
        existingService.Should().Be("existing service");
    }
}
