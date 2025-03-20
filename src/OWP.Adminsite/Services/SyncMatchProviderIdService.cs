using Fanex.Data.Repository;
using Newtonsoft.Json;
using Olympus.OWP.Log.Data.Repository.Criteria;
using Olympus.OWP.Log.Data.Repository.Services;
using OWP.Adminsite.Criteria;
using OWP.Constants;
using OWP.Services;


namespace OWP.Adminsite.Services
{
    public interface SyncMatchProviderIdService
    {
        public Task Execute(DateTime date);
    }

    public class SyncMatchProviderIdImp(
        IApiService apiService,
        IDynamicRepository dynamicRepository,
        LogWriterService logWriter)
        : SyncMatchProviderIdService
    {
        private readonly ResiliencePolicy resiliencePolicy = new(int.MaxValue, retryAttempt =>
            TimeSpan.FromMinutes(60), (exception, timeSpan, retryCount, context) =>
        {
            logWriter.Execute(new AdminSiteLogCriteria
            {
                Action = "SyncProviderMatchId",
                CreatedDate = DateTime.Now.AddDays(12),
                Payload = $"Retry {retryCount} after {timeSpan.TotalMinutes} mins due to {exception.Message}"
            });
        });

        public async Task Execute(DateTime date)
        {
            await resiliencePolicy.ExecuteAsync(async t =>
            {
                var providerMatchIdTw = await apiService.QueryProviderMatchId(date);
                var providerMatchIdInfo = providerMatchIdTw.content
                    .Select(x => new
                    {
                        CreatedDate = DateTime.Now.AddHours(12),
                        MatchId = x.IbcMatchId,
                        x.ProviderMatchId,
                        x.ProviderId,
                        Provider = ProviderConst.Provider.GetValueOrDefault(x.ProviderId, string.Empty)
                    });


                await dynamicRepository.ExecuteAsync(new InsertProviderMatchIdCriteria()
                {
                    ProviderMatchIdJson = JsonConvert.SerializeObject(providerMatchIdInfo)
                }, cancellationToken: t);
                return true;
            });
           
        }
    }
}
