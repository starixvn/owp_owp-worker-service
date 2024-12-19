namespace OWP.Provider;

using Microsoft.Extensions.DependencyInjection;
using AgentConfirm;
using OWP.Provider.AgentConfirm.Services;

public static class DependenceInjection
{
    public static IServiceCollection AddProviderServices(this IServiceCollection services)
    {
        services.AddTransient<SyncAgentConfirmHandler, SyncAgentConfirmHandlerImp>();
        services.AddTransient<Repository, RepositoryImp>();
        return services;
    }
}