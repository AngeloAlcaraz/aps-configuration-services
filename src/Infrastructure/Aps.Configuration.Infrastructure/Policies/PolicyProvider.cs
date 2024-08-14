namespace Aps.Configuration.Infrastructure.Policies;

public class PolicyProvider : IPolicyProvider
{
    private readonly ILogger<PolicyProvider> _logger;

    public PolicyProvider(ILogger<PolicyProvider> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets an asynchronous retry policy that handles specific exceptions with exponential backoff.
    /// </summary>
    /// <returns>An <see cref="IAsyncPolicy"/> that defines the retry policy.</returns>
    public IAsyncPolicy GetRetryPolicy()
    {
        return Policy
            .Handle<TimeoutException>()
            .Or<MongoConnectionException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), // Exponential backoff
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning("Retry {RetryCount} due to: {ExceptionMessage}", retryCount, exception.Message);
                });
    }
}