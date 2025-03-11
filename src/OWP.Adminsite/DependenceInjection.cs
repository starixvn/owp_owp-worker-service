using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Olympus.OWP.Log.Data.Repository.Services;
using OWP.Adminsite.Models;
using OWP.Adminsite.Repository;
using OWP.Adminsite.Services;
using OWP.Services;

namespace OWP.Adminsite;

public static class DependenceInjection
{
    public static IServiceCollection AddAdminSiteSyncServices(this IServiceCollection services, IConfiguration configuration)
    {
        var apiSettings = configuration.GetSection("AdminSiteSettings").Get<AdminSiteSettings>();
        services.AddSingleton(apiSettings);

        services.AddTransient<LogWriterService, LogWriterServiceImp>();
        services.AddTransient<IMatchInfoRepository, MatchInfoRepository>();
        services.AddTransient<IApiService, ApiService>();
        services.AddTransient<IMatchInfoService, MatchInfoService>();
        services.AddTransient<ISyncDataService, SyncDataService>();
        services.AddTransient<SyncMatchProviderIdService, SyncMatchProviderIdImp>();
        services.AddTransient<LogWriterService, LogWriterServiceImp>();

        return services;
    }
}