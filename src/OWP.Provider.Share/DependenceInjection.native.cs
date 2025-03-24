namespace OWP.Provider.Share;

using AgentConfirm;
using Microsoft.Extensions.DependencyInjection;
using OWP.Provider.Share.AgentConfirm.Services;

public static class DependenceInjection
{
    public static IServiceCollection AddProviderServices(this IServiceCollection services)
    {
        services.AddTransient<SyncAgentConfirmHandler, SyncAgentConfirmHandlerImp>();
        services.AddTransient<Repository, RepositoryImp>();
        return services;
    }
}