using Fanex.Data.Repository;
using Olympus.OWP.Log.Data.Repository.Services;
using Polly;
using Polly.Wrap;


namespace OWP.Services
{
    public class ResiliencePolicy
    {
        private readonly AsyncPolicyWrap _policy;

        public ResiliencePolicy(int retryCount,
            Func<int, TimeSpan> sleepDurationProvider,
            Action<Exception,TimeSpan,int,Context> onRetry)
        {
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(retryCount, sleepDurationProvider,
                    onRetry);

           
            var circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(retryCount, TimeSpan.FromSeconds(10),
                    (ex, duration) => {},
                    () => {
                        
                        },
                    () =>
                    {
                       
                    });

            _policy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);
        }

        public async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken token = default)
        {
            return await _policy.ExecuteAsync(async () => await action(token));
        }
    }
}
