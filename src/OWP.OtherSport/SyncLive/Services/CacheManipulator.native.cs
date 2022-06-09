using Fanex.Caching;
using OWP.OtherSport.SyncLive.Model;

namespace OWP.OtherSport.SyncLive.Services
{
    public interface CacheManipulator
    {
        Task<List<DeletedMatch>> GetDeletedMatches();

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

        public async Task<List<DeletedMatch>> GetDeletedMatches()
        {
            var cacheResult = await cache.TryGetAsync<List<DeletedMatch>>(DELETED_MATCH_KEY);
            var matches = cacheResult.Item2 ?? new List<DeletedMatch>();

            return matches.Where(m => m.OSMatchId > 0).ToList();
        }

        public async Task RemoveDeletedMatches()
        {
            await cache.RemoveAsync(DELETED_MATCH_KEY);
        }
    }
}
