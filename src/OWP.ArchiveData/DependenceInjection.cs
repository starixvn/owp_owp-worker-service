using Microsoft.Extensions.DependencyInjection;
using OWP.ArchiveData.Services;

namespace OWP.ArchiveData;

public static class DependenceInjection
{
    public static IServiceCollection AddArchiveDataServices(this IServiceCollection services)
    {
        services.AddTransient<Repository, RepositoryImp>();
        services.AddTransient<ArchiveDataService, ArchiveDataServiceImp>();
        return services;
    }
}