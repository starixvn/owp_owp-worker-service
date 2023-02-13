using Fanex.Caching;
using Fanex.Caching.Redis;
using Fanex.Data.Repository;
using Fanex.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ONEbook.UM.Configurations;
using ONEbook.UM.Helpers;
using ONEbook.UM.Services;
using ONEbook.UM.Transport;
using OWP;
using OWP.ArchiveData;
using OWP.Command;
using OWP.Command.Settings;
using OWP.Infosite;
using OWP.Infosite.Services;
using OWP.InfositeIdSync;
using OWP.LinkLeagueAndTeam;
using OWP.MatchReadyNotification;
using OWP.OtherSport;
using OWP.RabbitMq;
using OWP.RabbitMq.Models;
using OWP.SumReport;
using OWP.SyncLeagueSettings;

#region Configuration

var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{environment}.json", true, true)
    .Build();

#endregion Configuration

#region ServiceCollection

var services = new ServiceCollection();

#endregion ServiceCollection

#region AddFanexData

FanexInitializer.Start(configuration);
services.AddScoped<IDynamicRepository, DynamicRepository>();
services.AddTransient<ILogger, Logger>();
services.AddSingleton<IConfiguration>(configuration);
#endregion AddFanexData

#region AddRedis

var redisSetting = configuration.GetSection(nameof(RedisSettings)).Get<RedisSettings>();

services.AddSingleton<ICache>(new RedisCache(new RedisCacheOptions
{
    ConnectionString = redisSetting.ConnectionString,
    ApplicationName = redisSetting.AppName
}));

#endregion AddRedis

#region AddOneBookUM

var umConfig = configuration.GetSection(nameof(UMClientConfiguration)).Get<UMClientConfiguration>();
services.AddSingleton(umConfig);
services.AddSingleton<IPHelper, IPHelperImpl>();
services.AddSingleton<IMaintenanceService, MaintenanceService>();
services.AddSingleton<IUMCacheService, UMCacheService>();
services.AddSingleton<IUMRestClient, UMRestClient>();

#endregion AddOneBookUM

#region AddRabbitMQ

var settings = new RabbitMQSetting
{
    Hosting = "localhost",
    VirtualHost = "/",
    UserName = "guest",
    Password = "guest",
    Port = 5672,
    RequestQueue = "OWP.Request.Queue",
};
IOptions<RabbitMQSetting> appSettingsOptions = Options.Create(settings);
var rabbitService = new RabbitMqClientImp(appSettingsOptions, new Logger());
rabbitService.ConnectRabbitMq();

services.AddSingleton<RabbitMqClient>(rabbitService);

#endregion

#region AddDependenceInjection
services.AddInfositeServices(configuration);
services.AddOWPCoreServices();
services.AddInfositeIdSyncServices();
services.AddLinkLeagueAndTeamServices();
services.AddMatchReadyNotificationServices();
services.AddSumReportServices();
services.AddSyncLeagueSettingsServices();
services.AddOtherSportServices();
services.AddArchiveDataServices();
#endregion

var apiService = services.BuildServiceProvider().GetService<SyncChildMatchService>();
await apiService.Sync(new DateTime(2022, 12, 05)).ConfigureAwait(false);
