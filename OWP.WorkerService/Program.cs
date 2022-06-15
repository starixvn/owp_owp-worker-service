using OWP.WorkerService;
using OWP.WorkerService.Scheduler;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCoreServices();
builder.Services.AddFanexServices(builder.Configuration);
builder.Services.AddRedisServices(builder.Configuration);
builder.Services.AddOneBookUmServices(builder.Configuration);
builder.Services.AddInternalServices(builder.Configuration);
builder.Services.InitializeRabbitMq(builder.Configuration);
builder.Services.AddRedisPubSub();
builder.Services.AddJobsServices();
builder.Services.AddHostedService<OWPScheduler>();

var app = builder.Build();
app.Run(async context =>
{
  await context.Response.WriteAsync("OWP Worker Services");
});
await app.RunAsync("http://0.0.0.0:5000");