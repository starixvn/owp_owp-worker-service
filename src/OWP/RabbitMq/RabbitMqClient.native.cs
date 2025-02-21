namespace OWP.RabbitMq;

using Fanex.Logging;
using Microsoft.Extensions.Options;
using Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

public interface RabbitMqClient
{
    ValueTask<bool> PublishRequestMessage(RequestBaseModel request);

    Task<bool> ConnectRabbitMq();
}

public class RabbitMqClientImp(IOptions<RabbitMqSetting> options, ILogger logger) : RabbitMqClient
{
    private IChannel channel;
    private readonly RabbitMqSetting setting = options.Value;

    public async Task<bool> ConnectRabbitMq()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                UserName = setting.UserName,
                Password = setting.Password,
                HostName = setting.Hosting,
                Port = setting.Port,
                VirtualHost = setting.VirtualHost
            };

            var connection = await factory.CreateConnectionAsync();
            channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: setting.RequestQueue, durable: true, exclusive: false, autoDelete: false);
        }
        catch (Exception ex)
        {
            await logger.ErrorAsync($"Connect RabbitMQ failed, exception: {ex}");
            return false;
        }

        return true;
    }

    public async ValueTask<bool> PublishRequestMessage(RequestBaseModel request)
    {
        const int EXPIRATION_TIME = 1000 * 60 * 30;
        var json = JsonConvert.SerializeObject(request);
        var messageBytes = Encoding.UTF8.GetBytes(json);

        var props = new BasicProperties
        {
            Type = request.RoutingKey,
            Expiration = EXPIRATION_TIME.ToString()
        };

        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: setting.RequestQueue,
            basicProperties: props,
            body: messageBytes,
            mandatory: true);

        return true;
    }
}
