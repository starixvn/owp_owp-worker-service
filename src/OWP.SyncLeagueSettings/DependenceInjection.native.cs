using Microsoft.Extensions.DependencyInjection;
using OWP.SyncLeagueSettings.Services;

namespace OWP.SyncLeagueSettings;

public static class DependenceInjection
{
    public static IServiceCollection AddSyncLeagueSettingsServices(this IServiceCollection services)
    {
        services.AddTransient<LeagueSettingsRepository, LeagueSettingsRepositoryImp>();
        services.AddTransient<LeagueSettingsService, LeagueSettingsServiceImp>();

        return services;
    }
}