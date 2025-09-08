namespace OWP.PushInfoToSettlement;

using Microsoft.Extensions.DependencyInjection;
using Services;

public static class DependenceInjection
{
    public static IServiceCollection AddPushInfoToSettlementServices(this IServiceCollection services)
    {
        services.AddTransient<PushService, PushServiceImp>();
        services.AddTransient<CacheManipulator, CacheManipulatorImp>();
        services.AddTransient<EventService, EventServiceImp>();

        return services;
    }
}