using Fanex.Data.Repository;

namespace OWP.Infosite.Services;

public interface FutureSyncService : SyncService
{
}

public class FutureSyncServiceImp : SyncServiceImp, FutureSyncService
{
    private readonly UpdateInfositeIdService updateInfositeIdService;

    public FutureSyncServiceImp(
        DateTimeService dateTimeService,
        ApiService apiService,
        IDynamicRepository repository,
        UpdateInfositeIdService updateInfositeIdService)
        : base(dateTimeService, apiService, repository)
    {
        this.updateInfositeIdService = updateInfositeIdService;
    }

    public async Task Sync()
    {
        var dateFrom = dateTimeService.Today.AddDays(1);
        const int syncDays = 10;
        
        for (var i = 0; i < syncDays; i++)
        {
            await Sync(dateFrom.AddDays(i));
            await updateInfositeIdService.UpdateInfositeId(dateFrom.AddDays(i));
        }
    }
}