namespace OWP.PushInfoToSettlement;

using Microsoft.Extensions.DependencyInjection;
using Services;

public static class DependenceInjection
{
    public static IServiceCollection AddPúhInfoToSettlementServices(this IServiceCollection services)
    {
        services.AddTransient<PushService, PushServiceImp>();

        return services;
    }
}