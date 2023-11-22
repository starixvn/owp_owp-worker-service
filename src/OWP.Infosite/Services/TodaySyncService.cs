namespace OWP.Infosite.Services;

using Fanex.Data.Repository;
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
        UpdateInfositeIdService updateInfositeIdService)
        : base(dateTimeService, apiService, rabbitMqClient, repository)
    {
        this.updateInfositeIdService = updateInfositeIdService;
    }

    public async Task Sync()
    {
        var today = DateTime.Today;

        await Sync(today, true);
        await Sync(today.AddDays(1), true);
        await updateInfositeIdService.UpdateInfositeId(today);
        await updateInfositeIdService.UpdateInfositeId(today.AddDays(1));
    }
}