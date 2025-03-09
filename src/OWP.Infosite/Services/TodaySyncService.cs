namespace OWP.Infosite.Services;

using Fanex.Data.Repository;

public interface TodaySyncService : SyncService;

public class TodaySyncServiceImp(
    DateTimeService dateTimeService,
    ApiService apiService,
    IDynamicRepository repository,
    UpdateInfositeIdService updateInfositeIdService,
    EventPickingLogsService eventPickingLogsService,
    ProviderService providerService)
    : SyncServiceImp(dateTimeService, apiService, repository,eventPickingLogsService, providerService), TodaySyncService
{
    public async Task Sync()
    {
        var today = DateTime.Today;

        await Sync(today);
        await Sync(today.AddDays(1));
        await updateInfositeIdService.UpdateInfositeId(today);
        await updateInfositeIdService.UpdateInfositeId(today.AddDays(1));
    }
}