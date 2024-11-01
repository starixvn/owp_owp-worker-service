namespace OWP.Infosite.Services;

using Fanex.Data.Repository;
using Fanex.Logging;

public interface TodaySyncService : SyncService
{
}

public class TodaySyncServiceImp : SyncServiceImp, TodaySyncService
{
    private readonly UpdateInfositeIdService updateInfositeIdService;
    private readonly ILogger logger;

    public TodaySyncServiceImp(
        DateTimeService dateTimeService,
        ApiService apiService,
        IDynamicRepository repository,
        UpdateInfositeIdService updateInfositeIdService,
        ILogger logger)
        : base(dateTimeService, apiService, repository)
    {
        this.updateInfositeIdService = updateInfositeIdService;
        this.logger = logger;
    }

    public async Task Sync()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        var today = DateTime.Today;
        await logger.InfoAsync($"Begin Sync Infosite today match on {DateTime.Now}");

        await Sync(today);
        await Sync(today.AddDays(1));
        await updateInfositeIdService.UpdateInfositeId(today);
        await updateInfositeIdService.UpdateInfositeId(today.AddDays(1));
        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        await logger.InfoAsync($"End Sync Infosite today match on {DateTime.Now}. Total time {elapsedMs}ms");
    }
}