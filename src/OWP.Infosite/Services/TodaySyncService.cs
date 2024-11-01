namespace OWP.Infosite.Services;

using Fanex.Data.Repository;

public interface TodaySyncService : SyncService
{
}

public class TodaySyncServiceImp : SyncServiceImp, TodaySyncService
{
    private readonly UpdateInfositeIdService updateInfositeIdService;

    public TodaySyncServiceImp(
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
        var today = DateTime.Today;

        await Sync(today);
        await Sync(today.AddDays(1));
        await updateInfositeIdService.UpdateInfositeId(today);
        await updateInfositeIdService.UpdateInfositeId(today.AddDays(1));
    }
}