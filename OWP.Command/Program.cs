using Fanex.Caching;
using Fanex.Caching.Redis;
using Fanex.Data.Repository;
using Fanex.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ONEbook.UM.Configurations;
using ONEbook.UM.Helpers;
using ONEbook.UM.Services;
using ONEbook.UM.Transport;
using OWP.Command;
using OWP.Command.Settings;
using OWP.Infosite;
using OWP.Infosite.Services;
using OWP.InfositeIdSync;
using OWP.LinkLeagueAndTeam;
using OWP.LinkLeagueAndTeam.Services;
using OWP.MatchReadyNotification;
using OWP.OtherSport;
using OWP.SumReportData;
using OWP.SumReportData.OvertimeReport.Services;
using OWP.SyncLeagueSettings;
using OWP.SyncLeagueSettings.Services;
using OWP.PrematchReport;
using OWP.ArchiveData;
using OWP.Core.RabbitMq;
using RabbitMQ.Client;
using OWP.Core.RabbitMq.Settings;
using OWP.Core.RedisPubSub;
using StackExchange.Redis;

#region Configuration

var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
var configuration = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.json", true, true)
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
var rabbitMqSettings = configuration.GetSection(nameof(RabbitMqSettings)).Get<RabbitMqSettings>();
services.AddSingleton(rabbitMqSettings);
services.AddSingleton(umConfig);
services.AddSingleton<IPHelper, IPHelperImpl>();
services.AddSingleton<IMaintenanceService, MaintenanceService>();
services.AddSingleton<IUMCacheService, UMCacheService>();
services.AddSingleton<IUMRestClient, UMRestClient>();

#endregion AddOneBookUM

services.AddInfositeServices(configuration);
services.AddOWPCoreServices();
services.AddInfositeIdSyncServices();
services.AddLinkLeagueAndTeamServices();
services.AddMatchReadyNotificationServices();
services.AddSumReportDataServices();
services.AddSyncLeagueSettingsServices();
services.AddOtherSportServices();
services.AddPrematchReportServices();
services.AddArchiveDataServices();

var hosting = configuration.GetSection("RabbitMqSettings:Hosting").Value;
var virtualHosting = configuration.GetSection("RabbitMqSettings:VirtualHost").Value;
var username = configuration.GetSection("RabbitMqSettings:UserName").Value;
var password = configuration.GetSection("RabbitMqSettings:Password").Value;
var port = int.Parse(configuration.GetSection("RabbitMqSettings:Port").Value);

services.AddSingleton(serviceProvider => new ConnectionFactory
{
    UserName = username,
    Password = password,
    HostName = hosting,
    Port = port,
    VirtualHost = virtualHosting,
    DispatchConsumersAsync = true
});

services.AddSingleton<RabbitMqClient>(new RabbitMqClientImp(configuration).Initialize());

var multiplexer = ConnectionMultiplexer.Connect(redisSetting.ConnectionString);
services.AddSingleton<IConnectionMultiplexer>(multiplexer);
services.AddSingleton<PubSubEvent, PubSubEventImp>();

var apiService = services.BuildServiceProvider().GetService<OWP.PrematchReport.Services.PrematchReportService>();
await apiService.CalculateDataReport().ConfigureAwait(false);