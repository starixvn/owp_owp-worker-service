namespace OWP.Infosite.Services;

using Fanex.Data.Repository;
using RabbitMq;

public interface FutureSyncService : SyncService
{
}

public class FutureSyncServiceImp : SyncServiceImp, FutureSyncService
{
    private readonly UpdateInfositeIdService updateInfositeIdService;

    public FutureSyncServiceImp(
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
        var dateFrom = DateTimeService.Today.AddDays(1);
        const int syncDays = 10;

        for (var i = 0; i < syncDays; i++)
        {
            await Sync(dateFrom.AddDays(i));
            await updateInfositeIdService.UpdateInfositeId(dateFrom.AddDays(i));
        }
    }
}