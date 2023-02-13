namespace OWP;

using Microsoft.Extensions.DependencyInjection;
using Services;

public static class DependenceInjection
{
    public static IServiceCollection AddOWPCoreServices(this IServiceCollection services)
    {
        services.AddTransient<HttpHandler, HttpHandlerImp>();

        return services;
    }
}