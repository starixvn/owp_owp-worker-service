namespace OWP.Infosite.Services;

using Fanex.Data.Repository;

public interface FutureSyncService : SyncService;

public class FutureSyncServiceImp(
    DateTimeService dateTimeService,
    ApiService apiService,
    IDynamicRepository repository,
    UpdateInfositeIdService updateInfositeIdService,
    EventPickingLogsService eventPickingLogsService,
    ProviderService providerService)
    : SyncServiceImp(dateTimeService, apiService, repository,eventPickingLogsService,
        providerService), FutureSyncService
{
    public async Task Sync()
    {
        var dateFrom = DateTimeService.Today.AddDays(1);
        const int syncDays = 10;

        for (var i = 0; i < syncDays; i++)
        {
            await Sync(dateFrom.AddDays(i));
            await updateInfositeIdService.UpdateInfositeId(dateFrom.AddDays(i));
        }
    }
}