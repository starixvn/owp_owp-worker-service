using Fanex.Data.SqlServer;
using Fanex.Logging.Sentry.RavenClient;
using OWP.WorkerService.Extensions;
using OWP.WorkerService.Jobs;
using OWP.WorkerService.Scheduler;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddCoreServices();
services.AddNapClient(configuration);
services.AddFanexSql(configuration);
services.AddFanexLogSentry(configuration);
services.AddRedisServices(configuration);
services.AddInternalServices(configuration);
services.AddRabbitMq(configuration);
services.AddJobsServices();

services.AddSingleton<GarbageCollectorJob>();
services.AddHostedService<OwpScheduler>();
services.AddHostedService<GarbageCollectorJob>();

var app = builder.Build();

app.UseRabbitMq();

app.Run(async context =>
{
    var request = context.Request.Path.HasValue
        ? context.Request.Path.Value
        : string.Empty;
    await context.Response.WriteAsync($"OWP Worker Services. Request Path = {request}");
});

await app.RunAsync();