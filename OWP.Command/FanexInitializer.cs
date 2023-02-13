namespace OWP.Command;

using Fanex.Data;
using Fanex.Data.SqlServer;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Microsoft.Extensions.Configuration;
using SharpRaven;

internal static class FanexInitializer
{
    public static void Start(IConfiguration configuration)
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
    }
}