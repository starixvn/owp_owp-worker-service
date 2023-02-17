namespace OWP.Infosite.Services;

using Fanex.Caching;
using Fanex.Data.Repository;
using Fanex.Logging;
using RabbitMq;

public interface TodaySyncService : SyncService
{
}

public class TodaySyncServiceImp : SyncServiceImp, TodaySyncService
{
    private readonly UpdateInfositeIdService updateInfositeIdService;

    public TodaySyncServiceImp(
        DateTimeService dateTimeService,
        RabbitMqClient rabbitMqClient,
        ApiService apiService,
        IDynamicRepository repository,
        ICache cacheService,
        UpdateInfositeIdService updateInfositeIdService)
        : base(dateTimeService, apiService, rabbitMqClient, cacheService, repository)
    {
        this.updateInfositeIdService = updateInfositeIdService;
    }

    public async Task Sync()
    {
        var today = DateTime.Today;
        var logMessage = $"SyncInfoSite QueryEvents with params: FromDate: {today}. ToDate: {today}";
        await Logger.Log.InfoAsync(logMessage).ConfigureAwait(false);

        await Sync(today, true);
        await Sync(today.AddDays(1), true);
        await updateInfositeIdService.UpdateInfositeId(today);
        await updateInfositeIdService.UpdateInfositeId(today.AddDays(1));
    }
}