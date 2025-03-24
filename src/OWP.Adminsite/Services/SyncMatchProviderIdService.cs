using Fanex.Data.Repository;
using Fanex.Logging;
using Newtonsoft.Json;
using Olympus.OWP.Log.Data.Repository.Criteria;
using Olympus.OWP.Log.Data.Repository.Services;
using OWP.Adminsite.Criteria;
using OWP.Constants;
using OWP.Provider.Share.AgentConfirm;
using OWP.Services;
using Polly;


namespace OWP.Adminsite.Services
{
    public interface SyncMatchProviderIdService
    {
        public Task Execute(DateTime date);
    }

    public class SyncMatchProviderIdImp
        : SyncMatchProviderIdService
    {
        private bool isTaskBeingRetried;
        private readonly ResiliencePolicy resiliencePolicy;
        private readonly IApiService apiService;
        private readonly SyncAgentConfirmHandler syncAgentConfirmHandler;
        private readonly IDynamicRepository dynamicRepository;
        private readonly LogWriterService logWriter;


        public SyncMatchProviderIdImp(
            IApiService apiService,
            SyncAgentConfirmHandler syncAgentConfirmHandler,
            IDynamicRepository dynamicRepository,
            LogWriterService logWriter)
        {
            this.apiService = apiService;
            this.syncAgentConfirmHandler = syncAgentConfirmHandler;
            this.dynamicRepository = dynamicRepository;
            this.logWriter = logWriter;
            this.resiliencePolicy = new ResiliencePolicy(int.MaxValue, ConfigSleepDuration, OnRetry);
        }
        

        public async Task Execute(DateTime date)
        {
            await resiliencePolicy.ExecuteAsync(async token =>
            {
                await SyncProviderMatchId(date, token);
                await CalculateAgentConfirm();
                return true;
            });
        }

        private async Task CalculateAgentConfirm()
        {
            if (isTaskBeingRetried)
            {
                await LogService.InfoAsync($"[SyncAgentConfirmJob]=> Job retry at: {DateTimeOffset.Now} GMT-4");

                var eventDate = DateTime.Now.Date.AddDays(-1); // gmt-4 => yesterday
                await syncAgentConfirmHandler.Handle(eventDate).ConfigureAwait(false);
                isTaskBeingRetried = false;
            }
        }
        private async Task SyncProviderMatchId(DateTime date, CancellationToken token)
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
            }, cancellationToken: token);
        }
        private void OnRetry(Exception exception, TimeSpan timeSpan, int retryCount, Context _)
        {
            LogService.InfoAsync($"[SyncProviderMatchIdJob]=> Job retry at: {DateTimeOffset.Now} GMT-4");
            logWriter.Execute(new AdminSiteLogCriteria
            {
                Action = "SyncProviderMatchId",
                CreatedDate = DateTime.Now.AddDays(12),
                Payload = $"Retry {retryCount} after {timeSpan.TotalMinutes} mins due to {exception.Message}"
            });
            isTaskBeingRetried = true;
        }

        private TimeSpan ConfigSleepDuration(int _)
            => TimeSpan.FromMinutes(60);
    }
}
