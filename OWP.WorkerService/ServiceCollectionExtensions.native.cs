using OWP.Adminsite.Services;
using Polly;
using Polly.Retry;

namespace OWP.WorkerService
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddControllerAuthorization(this IServiceCollection services)
        {
           
            services.AddHttpClient<SyncMatchProviderIdService, SyncMatchProviderIdImp>()
               
                .AddResilienceHandler("",options =>
                {
                    options.AddRetry(new RetryStrategyOptions<HttpResponseMessage>()
                    {
                       MaxRetryAttempts = 3, // Retries up to 3 times
                       BackoffType = DelayBackoffType.Constant, // Exponential backoff
                       Delay = TimeSpan.FromSeconds(2), // First retry after 2 seconds
                       ShouldHandle = config =>
                       {
                           return new ValueTask<bool>();
                       }
                    });
                });
            return services;
        }
    }
}
