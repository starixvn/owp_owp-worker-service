namespace OWP.Infosite;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Settings;

public static class DependenceInjection
{
    public static IServiceCollection AddInfositeServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<InfoSiteSettings>(configuration.GetSection(nameof(InfoSiteSettings)));

        services.AddTransient<NAPUserControlService, NAPUserControlServiceImp>();
        services.AddTransient<ApiService, ApiServiceImp>();
        services.AddTransient<DateTimeService, DateTimeServiceImp>();
        services.AddTransient<UpdateInfositeIdService, UpdateInfositeIdServiceImp>();
        services.AddTransient<SyncPreMatchLeagueService, SyncPreMatchLeagueServiceImp>();
        services.AddTransient<TodaySyncService, TodaySyncServiceImp>();
        services.AddTransient<SyncEventDeletedService, SyncEventDeletedServiceImp>();
        services.AddTransient<Sync180DayInfositeMatchService, Sync180DayInfositeMatchServiceImp>();
        services.AddTransient<FutureSyncService, FutureSyncServiceImp>();
        services.AddTransient<SyncChildMatchService, SyncChildMatchServiceImp>();
        services.AddTransient<SyncDeleteChildMatchService, SyncDeleteChildMatchServiceImp>();

        return services;
    }
}