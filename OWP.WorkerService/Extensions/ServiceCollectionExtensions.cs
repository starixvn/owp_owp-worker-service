namespace OWP.WorkerService.Extensions;

using ArchiveData;
using Fanex.Caching;
using Fanex.Caching.Redis;
using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.Data.SqlServer;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Infosite;
using InfositeIdSync;
using Jobs;
using LinkLeagueAndTeam;
using MatchReadyNotification;
using ONEbook.UM.Configurations;
using ONEbook.UM.Helpers;
using ONEbook.UM.Services;
using ONEbook.UM.Transport;
using OtherSport;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using RabbitMq;
using RabbitMq.Models;
using Settings;
using SharpRaven;
using StackExchange.Redis;
using SumReport;
using SyncLeagueSettings;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<IJobFactory, JobFactory>();
        services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
        services.AddSingleton<IDynamicRepository, DynamicRepository>();
        services.AddSingleton<ILogger, Logger>();

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
                    Dsn = new Dsn(configuration.GetSection("SentryToken").Value)
                })
            });

        return services;
    }

    public static IServiceCollection AddRedisServices(this IServiceCollection services, IConfiguration configuration)
    {
        var redisSetting = configuration.GetSection(nameof(RedisSettings)).Get<RedisSettings>();
        var connectionString = redisSetting?.ConnectionString ?? string.Empty;
        var appName = redisSetting?.AppName ?? string.Empty;
        var redis = ConnectionMultiplexer.Connect(connectionString);
        services.AddSingleton<IConnectionMultiplexer>(redis);
        services.AddSingleton<ICache>(new RedisCache(new RedisCacheOptions
        {
            ConnectionString = connectionString,
            ApplicationName = appName
        }));

        return services;
    }

    public static IServiceCollection AddOneBookUmServices(this IServiceCollection services, IConfiguration configuration)
    {
        var umConfig = configuration.GetSection(nameof(UMClientConfiguration)).Get<UMClientConfiguration>();
        services.AddSingleton(umConfig ?? new UMClientConfiguration());
        services.AddSingleton<IPHelper, IPHelperImpl>();
        services.AddSingleton<IMaintenanceService, MaintenanceService>();
        services.AddSingleton<IUMCacheService, UMCacheService>();
        services.AddSingleton<IUMRestClient, UMRestClient>();

        return services;
    }

    public static IServiceCollection AddInternalServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfositeServices(configuration);
        services.AddOwpCoreServices();
        services.AddInfositeIdSyncServices();
        services.AddLinkLeagueAndTeamServices();
        services.AddMatchReadyNotificationServices();
        services.AddSumReportServices();
        services.AddSyncLeagueSettingsServices();
        services.AddOtherSportServices();
        services.AddArchiveDataServices();

        return services;
    }

    public static IServiceCollection AddJobsServices(this IServiceCollection services)
    {
        services.AddTransient<SyncFutureInfoSiteMatchJob>();
        services.AddTransient<SyncEventDeletedJob>();
        services.AddTransient<SyncDataNotificationSettingJob>();
        services.AddTransient<MatchReadyNotificationJob>();
        services.AddTransient<SumTraderReportJob>();
        services.AddTransient<SumLaReportJob>();
        services.AddTransient<SyncInfositeIdJob>();
        services.AddTransient<SyncPreMatchLeagueJob>();
        services.AddTransient<Sync180DayInfositeMatchJob>();
        services.AddTransient<SyncLeagueSettingsJob>();
        services.AddTransient<LinkLeagueAndTeamJob>();
        services.AddTransient<SumOvertimeReportJob>();
        services.AddTransient<SyncTodayInfoSiteMatchJob>();
        services.AddTransient<SyncTodayInfoSiteChildMatchJob>();
        services.AddTransient<SyncFutureInfoSiteChildMatchJob>();
        services.AddTransient<SyncDeletedChildInfoSiteMatchJob>();
        services.AddTransient<SyncOtherSportAdminSiteMatchJob>();
        services.AddTransient<SumPrematchReportJob>();
        services.AddTransient<SumMatchLiveReportJob>();
        services.AddTransient<DeleteOldMessageJob>();
        services.AddTransient<ArchiveDataJob>();
        services.AddTransient<HealthCheckLogJob>();

        return services;
    }

    public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqSetting>(configuration.GetSection(RabbitMqSetting.RabbitMq));
        services.AddSingleton<RabbitMqClient, RabbitMqClientImp>();

        return services;
    }
}