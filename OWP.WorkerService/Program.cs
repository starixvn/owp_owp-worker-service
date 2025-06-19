using Fanex.Data.SqlServer;
using Fanex.Logging.Sentry.RavenClient;
using OWP.WorkerService.Extensions;
using OWP.WorkerService.Jobs;
using OWP.WorkerService.Models;
using OWP.WorkerService.Scheduler;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

var jobSettings = configuration.GetSection("JobSettings").Get<List<JobSettings>>();
services.AddSingleton(jobSettings);

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

var logger = app.Services.GetRequiredService<Fanex.Logging.ILogger>();
await logger.InfoAsync("Environment: " + configuration["Environment"]);

app.Run(async context =>
{
    var request = context.Request.Path.HasValue
        ? context.Request.Path.Value
        : string.Empty;
    await context.Response.WriteAsync($"OWP Worker Services. Request Path = {request}");
});

await app.RunAsync();