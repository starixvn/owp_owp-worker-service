using OWP.Infosite.Helper;
using OWP.Infosite.Models;
using Fanex.Data.Repository;
using OWP.Infosite.Criteria;

namespace OWP.Infosite.Services
{
    public interface ProviderService
    {
        Task UpdateProviders(List<Event> queriedResult, List<InfoSiteDbInfo> infoSiteDbInfos);
    }
    public class ProviderServiceImp(IDynamicRepository repository) : ProviderService
    {
        public async Task UpdateProviders(List<Event> queriedResult, List<InfoSiteDbInfo> infoSiteDbInfos)
        {
            var priorityProviders = await GetProviderArrangement();
            foreach (var x in queriedResult)
            {
                var currentTime = DateTime.UtcNow.AddHours(8);
                if (currentTime < x.EventDateTime.AddDays(-2))
                {
                    x.Provider = ProviderHelper.BuildProviderByOtherProvider(priorityProviders, x.OtherProvider);
                }
                else
                {
                    var matchingEvent = infoSiteDbInfos.Find(e => e.InfoSiteMatchId == x.InfoSiteMatchId);
                    if (matchingEvent != null && !string.IsNullOrEmpty(matchingEvent.Provider))
                    {
                        x.Provider = matchingEvent.Provider;
                    }
                }
            }
        }

        private async Task<List<string>> GetProviderArrangement()
        {
            var criteria = new GetProviderCriteria();
            var provider = (await repository.FetchAsync<ProviderInfo>(criteria))
                .OrderBy(x => x.Order)
                .Select(x => x.Value)
                .ToList();

            return provider;
        }
    }
}
