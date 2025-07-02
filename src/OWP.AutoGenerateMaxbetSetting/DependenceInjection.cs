namespace OWP.AutoGenerateMaxbetSetting;

using Microsoft.Extensions.DependencyInjection;
using Services;

public static class DependenceInjection
{
    public static IServiceCollection AddAutoGenerateMaxbetSettingServices(this IServiceCollection services)
    {
        services.AddTransient<Repository, RepositoryImp>();
        services.AddTransient<GeneratorService, GeneratorServiceImp>();
        services.AddTransient<MaxbetGeneratorService, MaxbetGeneratorServiceImp>();
        services.AddTransient<SpreadGeneratorService, SpreadGeneratorServiceImp>();
        return services;
    }
}