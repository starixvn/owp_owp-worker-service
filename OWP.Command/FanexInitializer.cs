namespace OWP.Command;

using Fanex.Data;
using Fanex.Data.SqlServer;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharpRaven;

internal static class FanexInitializer
{
    public static void Start(IConfiguration configuration)
    {
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