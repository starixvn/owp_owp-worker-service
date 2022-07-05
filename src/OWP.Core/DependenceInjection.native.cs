using Microsoft.Extensions.DependencyInjection;
using OWP.Core.Services;

namespace OWP.InfositeIdSync;

public static class DependenceInjection
{
    public static IServiceCollection AddOWPCoreServices(this IServiceCollection services)
    {
        services.AddTransient<HttpHandler, HttpHandlerImp>();

        return services;
    }
}