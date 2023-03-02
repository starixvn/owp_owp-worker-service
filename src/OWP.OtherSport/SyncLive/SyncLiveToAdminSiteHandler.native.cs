namespace OWP.OtherSport.SyncLive;

using Model;
using Services;
using SyncAdminSiteId.Models;

public interface SyncLiveToAdminSiteHandler
{
    Task Handle(IList<Match> adminSiteMatches, IList<Match> owpMatches, DateTime eventDate);
}

public class SyncLiveToAdminSiteHandlerImp : SyncLiveToAdminSiteHandler
{
    private readonly EditEventGenerator editEventGenerator;

    private readonly ApiService apiService;

    private readonly CacheManipulator cacheManipulator;

    public SyncLiveToAdminSiteHandlerImp(
        EditEventGenerator editEventGenerator,
        ApiService apiService,
        CacheManipulator cacheManipulator)
    {
        this.editEventGenerator = editEventGenerator;
        this.apiService = apiService;
        this.cacheManipulator = cacheManipulator;
    }

    public async Task Handle(IList<Match> adminSiteMatches, IList<Match> owpMatches, DateTime eventDate)
    {
        var deletedMatches = await cacheManipulator.GetDeletedMatches();

        var duplicateAdminSiteIdMatches = owpMatches.Where(x => CheckDuplicateAdminSiteId(owpMatches, x.AdminSiteMatchId));
        var syncMatches = owpMatches.Except(duplicateAdminSiteIdMatches);

        var editEventList = editEventGenerator.GetChangedOfferLiveMatches(adminSiteMatches, syncMatches.Union(deletedMatches)).ToList();

        if (!editEventList.Any())
        {
            return;
        }

        var model = new BodyModel { live_configs = editEventList };

        await apiService.PostSyncLiveAsync(model);
        await cacheManipulator.RemoveDeletedMatches();
    }

    private static bool CheckDuplicateAdminSiteId(IEnumerable<Match> match, int? id)
        => match.Count(x => x.AdminSiteMatchId == id) > 1;
}