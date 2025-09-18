namespace OWP.PushInfoToSettlement;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OWP.PushInfoToSettlement.Model;
using Services;

public static class DependenceInjection
{
    public static IServiceCollection AddPushInfoToSettlementServices(this IServiceCollection services, IConfiguration configuration)
    {
        var apiSettings = configuration.GetSection("SettlementApiSettings").Get<SettlementApiSetting>();
        services.AddSingleton(apiSettings);

        services.AddTransient<PushService, PushServiceImp>();
        services.AddTransient<CacheManipulator, CacheManipulatorImp>();
        services.AddTransient<EventService, EventServiceImp>();
        services.AddTransient<ISettlementApiService, SettlementApiService>();

        return services;
    }
}