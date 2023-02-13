namespace OWP.OtherSport;

using Microsoft.Extensions.DependencyInjection;
using SyncAdminSiteId.Handlers;
using SyncLive;
using SyncLive.Services;

public static class DependenceInjection
{
    public static IServiceCollection AddOtherSportServices(this IServiceCollection services)
    {
        services.AddTransient<SyncAdminSiteMatchesHandler, SyncAdminSiteMatchesHandlerImp>();
        services.AddTransient<SyncLiveToAdminSiteHandler, SyncLiveToAdminSiteHandlerImp>();
        services.AddTransient<EditEventGenerator, EditEventGeneratorImp>();
        services.AddTransient<ApiService, ApiServiceImp>();
        services.AddTransient<CacheManipulator, CacheManipulatorImp>();
        return services;
    }
}