namespace OWP.OtherSport;

using Microsoft.Extensions.DependencyInjection;
using OWP.Provider.AgentConfirm;
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