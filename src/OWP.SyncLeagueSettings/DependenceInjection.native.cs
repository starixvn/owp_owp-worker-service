namespace OWP.SyncLeagueSettings;

using Microsoft.Extensions.DependencyInjection;
using Services;

public static class DependenceInjection
{
    public static IServiceCollection AddSyncLeagueSettingsServices(this IServiceCollection services)
    {
        services.AddTransient<LeagueSettingsRepository, LeagueSettingsRepositoryImp>();
        services.AddTransient<LeagueSettingsService, LeagueSettingsServiceImp>();

        return services;
    }
}