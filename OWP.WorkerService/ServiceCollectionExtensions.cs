using Fanex.Caching;
using Fanex.Caching.Redis;
using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.Data.SqlServer;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using ONEbook.UM.Configurations;
using ONEbook.UM.Helpers;
using ONEbook.UM.Services;
using ONEbook.UM.Transport;
using OWP.Infosite;
using OWP.InfositeIdSync;
using OWP.LinkLeagueAndTeam;
using OWP.MatchReadyNotification;
using OWP.OtherSport;
using OWP.SumReportData;
using OWP.SyncLeagueSettings;
using OWP.WorkerService.Jobs;
using OWP.WorkerService.Settings;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace OWP.WorkerService;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<IJobFactory, JobFactory>();
        services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
        services.AddSingleton<IDynamicRepository, DynamicRepository>();
        services.AddSingleton<Fanex.Logging.ILogger, Logger>();

        return services;
    }

    public static IServiceCollection AddFanexServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connections = configuration.GetSection("ConnectionStrings")
            .GetChildren()
            .AsEnumerable()
            .ToDictionary(
                x => x.GetValue<string>("Name"),
                y => new ConnectionConfiguration(y.GetValue<string>("Name"), y.GetValue<string>("ConnectionString"), y.GetValue<string>("ProviderName"))
            );

        DbSettingProviderManager
            .StartNewSession()
            .Use(connections)
            .WithSqlServer()
            .UseDefaultDbSettingProvider(resourcePath: "~/App_Data")
            .Run();

        LogManager.SetDefaultLogCategory(configuration.GetSection("FanexLogCategory").Value)
            .Use(new List<ILogEngine>
            {
                new SentryLogging( new SentryEngineOptions
                {
                    Dsn = new SharpRaven.Dsn(configuration.GetSection("SentryToken").Value)
                })
            });

        return services;
    }

    public static IServiceCollection AddRedisServices(this IServiceCollection services, IConfiguration configuration)
    {
        var redisSetting = configuration.GetSection(nameof(RedisSettings)).Get<RedisSettings>();
        services.AddSingleton<ICache>(new RedisCache(new RedisCacheOptions
        {
            ConnectionString = redisSetting.ConnectionString,
            ApplicationName = redisSetting.AppName
        }));

        return services;
    }

    public static IServiceCollection AddOneBookUmServices(this IServiceCollection services, IConfiguration configuration)
    {
        var umConfig = configuration.GetSection(nameof(UMClientConfiguration)).Get<UMClientConfiguration>();
        services.AddSingleton(umConfig);
        services.AddSingleton<IPHelper, IPHelperImpl>();
        services.AddSingleton<IMaintenanceService, MaintenanceService>();
        services.AddSingleton<IUMCacheService, UMCacheService>();
        services.AddSingleton<IUMRestClient, UMRestClient>();

        return services;
    }

    public static IServiceCollection AddInternalServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfositeServices(configuration);
        services.AddInfositeIdSyncServices();
        services.AddLinkLeagueAndTeamServices();
        services.AddMatchReadyNotificationServices();
        services.AddSumReportDataServices();
        services.AddSyncLeagueSettingsServices();
        services.AddOtherSportServices();

        return services;
    }

    public static IServiceCollection AddJobsServices(this IServiceCollection services)
    {
        services.AddTransient<SyncInfositeIdJob>();
        services.AddTransient<SyncFutureInfoSiteMatchJob>();
        services.AddTransient<SyncTodayInfoSiteMatchJob>();
        services.AddTransient<SyncDataNotificationSettingJob>();
        services.AddTransient<SyncPreMatchLeagueJob>();
        services.AddTransient<LinkLeagueAndTeamJob>();
        services.AddTransient<SumOvertimeReportJob>();
        services.AddTransient<SyncLeagueSettingsJob>();
        services.AddTransient<SyncOtherSportAdminSiteMatchJob>();

        return services;
    }
}