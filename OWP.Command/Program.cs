using Fanex.Caching;
using Fanex.Caching.Redis;
using Fanex.Data.Repository;
using Fanex.Data.SqlServer;
using Fanex.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OWP;
using OWP.ArchiveData;
using OWP.Command;
using OWP.Command.Settings;
using OWP.Infosite;
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

services.AddFanexSql(configuration);
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

#region AddRabbitMQ

var settings = new RabbitMqSetting
{
    Hosting = "localhost",
    VirtualHost = "/",
    UserName = "guest",
    Password = "guest",
    Port = 5672,
    RequestQueue = "OWP.Request.Queue"
};
var appSettingsOptions = Options.Create(settings);
var rabbitService = new RabbitMqClientImp(appSettingsOptions, new Logger());
rabbitService.ConnectRabbitMq();

services.AddSingleton<RabbitMqClient>(rabbitService);

#endregion

#region AddDependenceInjection
services.AddInfositeServices(configuration);
services.AddOwpCoreServices();
services.AddInfositeIdSyncServices();
services.AddLinkLeagueAndTeamServices();
services.AddMatchReadyNotificationServices();
services.AddSumReportServices();
services.AddSyncLeagueSettingsServices();
services.AddOtherSportServices();
services.AddProviderServices();
services.AddArchiveDataServices();
#endregion

var apiService = services.BuildServiceProvider().GetService<OWP.Infosite.Services.TodaySyncService>();
if (apiService != null)
{
    await apiService.Sync().ConfigureAwait(false);
}
