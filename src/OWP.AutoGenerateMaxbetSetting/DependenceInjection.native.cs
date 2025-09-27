namespace OWP.AutoGenerateMaxbetSetting;

using Microsoft.Extensions.DependencyInjection;
using Services;

public static class DependenceInjection
{
    public static IServiceCollection AddAutoGenerateNewLeagueSettingServices(this IServiceCollection services)
    {
        services.AddTransient<LeagueSettingsRepository, LeagueSettingsRepositoryImp>();
        services.AddTransient<GeneratorService, GeneratorServiceImp>();
        services.AddTransient<LeagueSettingChangeService, LeagueSettingChangeServiceImp>();

        return services;
    }
}