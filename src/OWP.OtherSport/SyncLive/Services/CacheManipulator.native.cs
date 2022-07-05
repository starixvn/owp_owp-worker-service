using Fanex.Caching;
using Newtonsoft.Json;
using OWP.OtherSport.SyncAdminSiteId.Models;
using OWP.OtherSport.SyncLive.Model;

namespace OWP.OtherSport.SyncLive.Services
{
    public interface CacheManipulator
    {
        Task<List<Match>> GetDeletedMatches();

        Task RemoveDeletedMatches();
    }

    internal class CacheManipulatorImp : CacheManipulator
    {
        private readonly ICache cache;

        private const string DELETED_MATCH_KEY = "OS-DELETED-MATCH";

        public CacheManipulatorImp(ICache cache)
        {
            this.cache = cache;
        }

        public async Task<List<Match>> GetDeletedMatches()
        {
            var cacheResult = await cache.TryGetAsync<string>(DELETED_MATCH_KEY);
            var matches = !string.IsNullOrEmpty(cacheResult.Item2)
                ? JsonConvert.DeserializeObject<List<Match>>(cacheResult.Item2)
                : new List<Match>();

            return matches.Where(m => m.OSMatchId > 0).ToList();
        }

        public async Task RemoveDeletedMatches()
        {
            await cache.RemoveAsync(DELETED_MATCH_KEY);
        }
    }
}
