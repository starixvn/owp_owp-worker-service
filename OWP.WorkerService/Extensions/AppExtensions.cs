using OWP.Core.RabbitMq;

namespace OWP.WorkerService.Extensions;

public static class AppExtensions
{
    public static IApplicationBuilder UseRabbitMQ(this IApplicationBuilder app)
    {
        var rabbitService = app.ApplicationServices.GetService<RabbitMqClient>();
        var lifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();
        lifetime!.ApplicationStarted.Register(() => rabbitService!.ConnectRabbitMq());

        return app;
    }
}