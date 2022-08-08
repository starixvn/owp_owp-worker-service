using Fanex.Caching;
using Fanex.Data.Repository;
using Fanex.Logging;
using Newtonsoft.Json;
using OWP.Core.RabbitMq;
using OWP.Core.RabbitMq.Models;
using OWP.Infosite.Constants;
using OWP.Infosite.Criteria;
using OWP.Infosite.Models;
using System.Globalization;

namespace OWP.Infosite.Services;

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
        var today = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Unspecified);

        await Sync(today, true);
        await Sync(today.AddDays(1), true);
        await updateInfositeIdService.UpdateInfositeId(today);
        await updateInfositeIdService.UpdateInfositeId(today.AddDays(1));
    }
}