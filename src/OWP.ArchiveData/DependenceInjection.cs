namespace OWP.ArchiveData;

using Microsoft.Extensions.DependencyInjection;
using Services;

public static class DependenceInjection
{
    public static IServiceCollection AddArchiveDataServices(this IServiceCollection services)
    {
        services.AddTransient<Repository, RepositoryImp>();
        services.AddTransient<ArchiveDataService, ArchiveDataServiceImp>();
        return services;
    }
}