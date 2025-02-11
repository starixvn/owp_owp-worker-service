namespace OWP;

using Microsoft.Extensions.DependencyInjection;
using Services;

public static class DependenceInjection
{
    public static IServiceCollection AddOwpCoreServices(this IServiceCollection services)
    {
        services.AddTransient<HttpHandler, HttpHandlerImp>();
        services.AddTransient<LogMonitoringService, LogMonitoringServiceImp>();

        return services;
    }
}