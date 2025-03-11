using Fanex.Data.Repository;
using Newtonsoft.Json;
using OWP.Adminsite.Criteria;
using OWP.Constants;


namespace OWP.Adminsite.Services
{
    public interface SyncMatchProviderIdService
    {
        public Task Execute(DateTime date);
    }

    public class SyncMatchProviderIdImp(
        IApiService apiService, 
        IDynamicRepository dynamicRepository)
        : SyncMatchProviderIdService
    {
        public async Task Execute(DateTime date)
        {
            var providerMatchIdTw=await apiService.QueryProviderMatchId(date);
            var providerMatchIdInfo = providerMatchIdTw.content
                .Select(x => new 
                {   
                    CreatedDate=date,
                    MatchId = x.IbcMatchId,
                    x.ProviderMatchId,
                    x.ProviderId,
                    Provider = ProviderConst.Provider.GetValueOrDefault(x.ProviderId,string.Empty)
                });

           
            await dynamicRepository.ExecuteAsync(new InsertProviderMatchIdCriteria()
            {
                ProviderMatchIdJson = JsonConvert.SerializeObject(providerMatchIdInfo)
            });
        }
    }
}
