namespace OWP.MatchReadyNotification;

using Microsoft.Extensions.DependencyInjection;
using Services;

public static class DependenceInjection
{
    public static IServiceCollection AddMatchReadyNotificationServices(this IServiceCollection services)
    {
        services.AddTransient<CacheManipulator, CacheManipulatorImp>();
        services.AddTransient<MatchReadyRepository, MatchReadyRepositoryImp>();
        services.AddTransient<ReceivedUserServices, ReceivedUserServicesImp>();
        services.AddTransient<NotReadyMessageFactory, NotReadyMessageFactoryImp>();
        services.AddTransient<Handler, HandlerImp>();

        return services;
    }
}