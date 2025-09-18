namespace OWP.MatchReadyNotification.UnitTests;

using Constants;
using Fanex.Caching;
using NSubstitute;
using Services;
using Xunit;

public class CacheManipulatorUnitTests
{
    private readonly ICache _cache;
    private readonly CacheManipulator _cacheManipulator;

    public CacheManipulatorUnitTests()
    {
        _cache = Substitute.For<ICache>();
        _cacheManipulator = new CacheManipulatorImp(_cache);
    }

    [Fact]
    public async Task GetLastTimeNotReadyNotification_WhenCacheHasValue_ShouldReturnValue()
    {
        // Arrange
        var expectedValue = "2024-01-01 10:00:00";
        var cacheKey = Constant.KeyLastTimeNotification;
        _cache.TryGetAsync<string>(cacheKey).Returns((true, expectedValue));

        // Act
        var result = await _cacheManipulator.GetLastTimeNotReadyNotification();

        // Assert
        Assert.Equal(expectedValue, result);
        await _cache.Received(1).TryGetAsync<string>(cacheKey);
    }

    [Fact]
    public async Task GetLastTimeNotReadyNotification_WhenCacheIsEmpty_ShouldReturnEmptyString()
    {
        // Arrange
        var cacheKey = Constant.KeyLastTimeNotification;
        _cache.TryGetAsync<string>(cacheKey).Returns((false, null));

        // Act
        var result = await _cacheManipulator.GetLastTimeNotReadyNotification();

        // Assert
        Assert.Equal(string.Empty, result);
        await _cache.Received(1).TryGetAsync<string>(cacheKey);
    }

    [Fact]
    public async Task GetLastTimeNotReadyNotification_WhenCacheReturnsNull_ShouldReturnEmptyString()
    {
        // Arrange
        var cacheKey = Constant.KeyLastTimeNotification;
        _cache.TryGetAsync<string>(cacheKey).Returns((true, null));

        // Act
        var result = await _cacheManipulator.GetLastTimeNotReadyNotification();

        // Assert
        Assert.Equal(string.Empty, result);
        await _cache.Received(1).TryGetAsync<string>(cacheKey);
    }

    [Fact]
    public async Task SaveLastTimeNotification_WhenCalled_ShouldSetCacheWithCorrectKeyAndOptions()
    {
        // Arrange
        var lastTimeNotification = "2024-01-01 11:00:00";
        var cacheKey = Constant.KeyLastTimeNotification;
        var expectedExpiration = TimeSpan.FromMinutes(Constant.CacheExpiredInDays);

        // Act
        await _cacheManipulator.SaveLastTimeNotification(lastTimeNotification);

        // Assert
        await _cache.Received(1).SetAsync(
            cacheKey,
            lastTimeNotification,
            Arg.Any<CacheItemOptions>());
    }

    [Fact]
    public async Task SaveLastTimeNotification_WithEmptyString_ShouldStillSetCache()
    {
        // Arrange
        var lastTimeNotification = string.Empty;
        var cacheKey = Constant.KeyLastTimeNotification;

        // Act
        await _cacheManipulator.SaveLastTimeNotification(lastTimeNotification);

        // Assert
        await _cache.Received(1).SetAsync(cacheKey, lastTimeNotification, Arg.Any<CacheItemOptions>());
    }

    [Fact]
    public async Task SaveLastTimeNotification_WithNullString_ShouldStillSetCache()
    {
        // Arrange
        string lastTimeNotification = null;
        var cacheKey = Constant.KeyLastTimeNotification;

        // Act
        await _cacheManipulator.SaveLastTimeNotification(lastTimeNotification);

        // Assert
        await _cache.Received(1).SetAsync(cacheKey, lastTimeNotification, Arg.Any<CacheItemOptions>());
    }

    [Fact]
    public async Task SaveLastTimeNotification_WhenCacheThrowsException_ShouldPropagateException()
    {
        // Arrange
        var lastTimeNotification = "2024-01-01 11:00:00";
        _cache.SetAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CacheItemOptions>())
            .Returns(Task.FromException(new InvalidOperationException("Cache error")));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _cacheManipulator.SaveLastTimeNotification(lastTimeNotification));
    }

    [Fact]
    public async Task GetLastTimeNotReadyNotification_WhenCacheThrowsException_ShouldPropagateException()
    {
        // Arrange
        var cacheKey = Constant.KeyLastTimeNotification;
        _cache.TryGetAsync<string>(cacheKey)
            .Returns(Task.FromException<(bool, string)>(new InvalidOperationException("Cache error")));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _cacheManipulator.GetLastTimeNotReadyNotification());
    }
}

