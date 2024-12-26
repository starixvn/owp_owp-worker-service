using Microsoft.Extensions.DependencyInjection;
using OWP.Adminsite.Repository;
using OWP.Adminsite.Services;

namespace OWP.Adminsite;

public static class DependenceInjection
{
    public static IServiceCollection AddSyncLeagueSettingsServices(this IServiceCollection services)
    {
        services.AddTransient<IMatchInfoRepository, MatchInfoRepository>();
        services.AddTransient<ISyncMatchInfoToAdminSiteService, SyncMatchInfoToAdminSiteService>();

        return services;
    }
}