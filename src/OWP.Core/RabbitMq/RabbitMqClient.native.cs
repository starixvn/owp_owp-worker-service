using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OWP.Core.RabbitMq.Models;
using OWP.Core.RabbitMq.Settings;
using RabbitMQ.Client;

namespace OWP.Core.RabbitMq;

public interface RabbitMqClient
{
    void Publish(RequestBaseModel message);
}

public class RabbitMqClientImp : RabbitMqClient, IDisposable
{
    private readonly RabbitMqSettings rabbitMqSettings;
    private readonly IModel channel;
    private readonly IConnection connection;

    public RabbitMqClientImp(
        IOptions<RabbitMqSettings> rabbitMqSettings,
        ConnectionFactory connectionFactory)
    {
        this.rabbitMqSettings = rabbitMqSettings.Value;
        
        #if !DEBUG
        if (connection == null || !connection.IsOpen)
        {
            connection = connectionFactory.CreateConnection();
        }

        if (channel == null || !channel.IsOpen)
        {
            channel = connection.CreateModel();
            channel.ExchangeDeclare(
                exchange: this.rabbitMqSettings.Exchange,
                type: "direct",
                durable: true,
                autoDelete: false);

            channel.QueueDeclare(
                queue: this.rabbitMqSettings.Queue,
                durable: true,
                exclusive: false,
                autoDelete: false);
        }
        #endif
    }

    public void Publish(RequestBaseModel message)
    {
        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
        var properties = channel.CreateBasicProperties();
        properties.ContentType = "application/json";
        properties.DeliveryMode = 1;
        properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        channel.BasicPublish(
            exchange: this.rabbitMqSettings.Exchange,
            routingKey: message.RoutingKey,
            body: body,
            basicProperties: properties);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool isDisposing)
    {
        if (channel != null && channel.IsOpen)
        {
            channel?.Close();
            channel?.Dispose();
        }

        if (connection != null && connection.IsOpen)
        {
            connection?.Close();
            connection?.Dispose();
        }
    }
}