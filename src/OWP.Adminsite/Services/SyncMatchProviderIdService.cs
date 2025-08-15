using Fanex.Data.Repository;
using Fanex.Logging;
using Newtonsoft.Json;
using OWP.Adminsite.Criteria;
using OWP.Constants;
using OWP.Provider.Share.AgentConfirm;


namespace OWP.Adminsite.Services
{
    public interface SyncMatchProviderIdService
    {
        public Task Execute(DateTime date);
    }

    public class SyncMatchProviderIdImp(
        IApiService apiService,
        SyncAgentConfirmHandler syncAgentConfirmHandler,
        IDynamicRepository dynamicRepository)
        : SyncMatchProviderIdService
    {
      


        public async Task Execute(DateTime date)
        {
            await SyncProviderMatchId(date);
            await CalculateAgentConfirm();
        }

        private async Task CalculateAgentConfirm()
        {
            await LogService.InfoAsync($"[SyncAgentConfirmJob]=> Job retry at: {DateTimeOffset.Now} GMT-4");

            var eventDate = DateTime.Now.Date.AddDays(-1); // gmt-4 => yesterday
            await syncAgentConfirmHandler.Handle(eventDate).ConfigureAwait(false);
        }
        private async Task SyncProviderMatchId(DateTime date, CancellationToken token = default)
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

            await dynamicRepository.ExecuteAsync(new InsertProviderMatchIdCriteria
            {
                ProviderMatchIdJson = JsonConvert.SerializeObject(providerMatchIdInfo)
            }, cancellationToken: token);
        }
    }
}
