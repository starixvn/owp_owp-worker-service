using Fanex.Data.Repository;
using Fanex.Logging;
using Newtonsoft.Json;
using OWP.Adminsite.Criteria;
using OWP.Adminsite.Models;
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
        IDynamicRepository dynamicRepository,
        ILogger logger)
        : SyncMatchProviderIdService
    {
      


        public async Task Execute(DateTime date)
        {
            await SyncProviderMatchId(date);
            await CalculateAgentConfirm();
        }

        public async Task CalculateAgentConfirm()
        {
            await LogService.InfoAsync($"[SyncAgentConfirmJob]=> Job retry at: {DateTimeOffset.Now} GMT-4");

            var eventDate = DateTime.Now.Date.AddDays(-1); // gmt-4 => yesterday
            await syncAgentConfirmHandler.Handle(eventDate).ConfigureAwait(false);
        }
        public async Task SyncProviderMatchId(DateTime date, CancellationToken token = default)
        {
            var providerMatchIdTw = await apiService.QueryProviderMatchId(date);
            var providerMatchIdInfo = providerMatchIdTw.content
                .Where(x=>x.ProviderId!=202)
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

            await AlertIfHaveAnyAbnormal(providerMatchIdTw.content);
        }

        public async Task AlertIfHaveAnyAbnormal(IList<Content> providerMatchInfo)
        {
            var duplicateProviderMatchId = DetectDuplicateProviderMatchId(providerMatchInfo);
            if (duplicateProviderMatchId.Any())
            {
                await logger.ErrorAsync(JsonConvert.SerializeObject(new
                {
                    message = $"Duplicate ProviderMatchId at: {DateTimeOffset.Now.UtcDateTime.AddHours(8)}",
                    payload = duplicateProviderMatchId
                }));
            }
        }

        public static IList<Content> DetectDuplicateProviderMatchId(IList<Content> providerMatchInfo)
        {
            var duplicateGroups = providerMatchInfo
                .GroupBy(p => p.ProviderMatchId)
                .Where(g => g.Count() > 1);

            var duplicates = duplicateGroups
                .SelectMany(g => g)
                .ToList();

            return duplicates;
        }
    }
}
