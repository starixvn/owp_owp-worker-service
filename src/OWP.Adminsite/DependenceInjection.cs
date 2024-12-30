using Microsoft.Extensions.DependencyInjection;
using OWP.Adminsite.Repository;
using OWP.Adminsite.Services;

namespace OWP.Adminsite;

public static class DependenceInjection
{
    public static IServiceCollection AddAdminSiteSyncServices(this IServiceCollection services)
    {
        services.AddTransient<IMatchInfoRepository, MatchInfoRepository>();
        services.AddTransient<IApiService, ApiService>();
        services.AddTransient<IMatchInfoService, MatchInfoService>();
        services.AddTransient<ISyncDataService, SyncDataService>();

        return services;
    }
}