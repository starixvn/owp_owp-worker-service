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

services.AddInfositeServices(configuration);
services.AddInfositeIdSyncServices();
services.AddLinkLeagueAndTeamServices();
services.AddMatchReadyNotificationServices();
services.AddSumReportDataServices();
services.AddSyncLeagueSettingsServices();
services.AddOtherSportServices();

var apiService = services.BuildServiceProvider().GetService<OWP.SumReportData.OvertimeReport.Services.ReportCalculationService>();
// Console.WriteLine(DateTime.Now.AddDays(6));
// await apiService.InsertNewLeagueSettings(DateTime.Now.AddDays(6));
for (var date = DateTime.Today.AddDays(-2); date < DateTime.Today; date = date.AddDays(1))
        {
            await apiService.SumOvertimeReport(date).ConfigureAwait(false);
        }
// await apiService?.Execute();