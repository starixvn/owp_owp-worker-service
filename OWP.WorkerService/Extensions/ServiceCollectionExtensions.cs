using OWP.AutoGenerateMaxbetSetting;
using OWP.Chat;
using OWP.PushInfoToSettlement;

namespace OWP.WorkerService.Extensions;

using Adminsite;
using ArchiveData;
using Fanex.Caching;
using Fanex.Caching.Redis;
using Fanex.Data.Repository;
using Fanex.Logging;
using Infosite;
using InfositeIdSync;
using Jobs;
using LinkLeagueAndTeam;
using MatchReadyNotification;
using OtherSport;
using OWP.Provider.Share;
using Provider;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using RabbitMq;
using RabbitMq.Models;
using Settings;
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
        services.AddProviderServices();
        services.AddAdminSiteSyncServices(configuration);
        services.AddChatServices();
        services.AddAutoGenerateNewLeagueSettingServices();
        services.AddPushInfoToSettlementServices();

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
        services.AddTransient<SyncAgentConfirmJob>();
        services.AddTransient<SumPrematchReportJob>();
        services.AddTransient<SumMatchLiveReportJob>();
        services.AddTransient<ArchiveOldMessageJob>();
        services.AddTransient<ArchiveDataJob>();
        services.AddTransient<SyncMatchInfoToAdminSiteJob>();
        services.AddTransient<SyncProviderMatchIdJob>();
        services.AddTransient<ChatJob>();
        services.AddTransient<AutoGenerateMaxbetSettingJob>();
        services.AddTransient<PushInfoToSettlementJob>();

        return services;
    }

    public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqSetting>(configuration.GetSection(RabbitMqSetting.RabbitMq));
        services.AddSingleton<RabbitMqClient, RabbitMqClientImp>();

        return services;
    }
}