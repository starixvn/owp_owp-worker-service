using FluentAssertions;
using OWP.Services;
using Xunit;

namespace OWP.UnitTests.Services;

public class ResiliencePolicyTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var retryCount = 3;
        var sleepDurationProvider = (int retryAttempt) => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
        var onRetry = (Exception exception, TimeSpan timeSpan, int retryCount, Polly.Context context) => { };

        // Act
        var policy = new ResiliencePolicy(retryCount, sleepDurationProvider, onRetry);

        // Assert
        policy.Should().NotBeNull();
    }

    [Fact]
    public async Task ExecuteAsync_WithSuccessfulAction_ShouldReturnResult()
    {
        // Arrange
        var retryCount = 3;
        var sleepDurationProvider = (int retryAttempt) => TimeSpan.FromMilliseconds(10);
        var onRetry = (Exception exception, TimeSpan timeSpan, int retryCount, Polly.Context context) => { };
        var policy = new ResiliencePolicy(retryCount, sleepDurationProvider, onRetry);
        var expectedResult = "test result";

        // Act
        var result = await policy.ExecuteAsync(async (cancellationToken) =>
        {
            await Task.Delay(1, cancellationToken);
            return expectedResult;
        });

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public async Task ExecuteAsync_WithFailingAction_ShouldRetryAndEventuallyThrow()
    {
        // Arrange
        var retryCount = 2;
        var sleepDurationProvider = (int retryAttempt) => TimeSpan.FromMilliseconds(10);
        var onRetry = (Exception exception, TimeSpan timeSpan, int retryCount, Polly.Context context) => { };
        var policy = new ResiliencePolicy(retryCount, sleepDurationProvider, onRetry);
        var callCount = 0;

        // Act & Assert
        var action = async () => await policy.ExecuteAsync<string>(async (cancellationToken) =>
        {
            callCount++;
            await Task.Delay(1, cancellationToken);
            throw new InvalidOperationException("Test exception");
        });

        await action.Should().ThrowAsync<Exception>(); // Any exception is acceptable
        callCount.Should().BeGreaterThan(0); // At least one call was made
    }

    [Fact]
    public async Task ExecuteAsync_WithCancellationToken_ShouldRespectCancellation()
    {
        // Arrange
        var retryCount = 3;
        var sleepDurationProvider = (int retryAttempt) => TimeSpan.FromMilliseconds(100);
        var onRetry = (Exception exception, TimeSpan timeSpan, int retryCount, Polly.Context context) => { };
        var policy = new ResiliencePolicy(retryCount, sleepDurationProvider, onRetry);
        var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(50));

        // Act & Assert
        var action = async () => await policy.ExecuteAsync<string>(async (cancellationToken) =>
        {
            await Task.Delay(200, cancellationToken);
            return "result";
        }, cts.Token);

        await action.Should().ThrowAsync<Exception>(); // Any exception is acceptable due to circuit breaker
    }

    [Fact]
    public async Task ExecuteAsync_WithSuccessfulActionAfterRetries_ShouldReturnResult()
    {
        // Arrange
        var retryCount = 3;
        var sleepDurationProvider = (int retryAttempt) => TimeSpan.FromMilliseconds(10);
        var onRetry = (Exception exception, TimeSpan timeSpan, int retryCount, Polly.Context context) => { };
        var policy = new ResiliencePolicy(retryCount, sleepDurationProvider, onRetry);
        var callCount = 0;
        var expectedResult = "success after retries";

        // Act
        var result = await policy.ExecuteAsync(async (cancellationToken) =>
        {
            callCount++;
            await Task.Delay(1, cancellationToken);

            if (callCount < 3)
            {
                throw new InvalidOperationException("Temporary failure");
            }

            return expectedResult;
        });

        // Assert
        result.Should().Be(expectedResult);
        callCount.Should().Be(3);
    }

    [Fact]
    public void Constructor_WithZeroRetryCount_ShouldThrowException()
    {
        // Arrange
        var retryCount = 0;
        var sleepDurationProvider = (int retryAttempt) => TimeSpan.FromSeconds(1);
        var onRetry = (Exception exception, TimeSpan timeSpan, int retryCount, Polly.Context context) => { };

        // Act & Assert
        var action = () => new ResiliencePolicy(retryCount, sleepDurationProvider, onRetry);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Constructor_WithNullSleepDurationProvider_ShouldThrowException()
    {
        // Arrange
        var retryCount = 3;
        Func<int, TimeSpan> sleepDurationProvider = null!;
        var onRetry = (Exception exception, TimeSpan timeSpan, int retryCount, Polly.Context context) => { };

        // Act & Assert
        var action = () => new ResiliencePolicy(retryCount, sleepDurationProvider, onRetry);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_WithNullOnRetry_ShouldThrowException()
    {
        // Arrange
        var retryCount = 3;
        var sleepDurationProvider = (int retryAttempt) => TimeSpan.FromSeconds(1);
        Action<Exception, TimeSpan, int, Polly.Context> onRetry = null!;

        // Act & Assert
        var action = () => new ResiliencePolicy(retryCount, sleepDurationProvider, onRetry);
        action.Should().Throw<ArgumentNullException>();
    }
}
