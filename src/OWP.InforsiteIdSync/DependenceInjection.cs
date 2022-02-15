using Microsoft.Extensions.DependencyInjection;
using Olympus.InfoSiteClient;
using OWP.InfositeIdSync.Services;

namespace OWP.InfositeIdSync;

public static class DependenceInjection
{
    public static IServiceCollection AddInfositeIdSyncServices(this IServiceCollection services)
    {
        services.AddTransient<InfositeIdMappingRepository, InfositeIdMappingRepositoryImp>();
        services.AddTransient<LogServiceWrapper, LogServiceWrapperImp>();
        services.AddTransient<SyncIsLiveService, SyncIsLiveServiceImp>();
        services.AddTransient<SyncInfositeIdService, SynInfositeIdServiceImp>();
        services.AddTransient<IsLiveMatchApi, IsLiveMatchApiImp>();
        services.AddTransient<CacheManipulator, CacheManipulatorImp>();

        return services;
    }
}