using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OWP.Core.RabbitMq.Models;
using RabbitMQ.Client;

namespace OWP.Core.RabbitMq;

public interface RabbitMqClient
{
  void Publish(RequestBaseModel message);
  void ConnectRabbitMq();
}
public class RabbitMqClientImp : RabbitMqClient, IDisposable
{
#pragma warning disable CS0649 // Field 'RabbitMqClientImp.channel/connection' is never assigned to
  private IModel channel;
  private IConnection connection;
#pragma warning restore CS0649
  private readonly IConnectionFactory connectionFactory;
  private readonly IConfiguration configuration;

  public RabbitMqClientImp(
      IConfiguration configuration)
  {
    this.configuration = configuration;

    var hosting = configuration.GetSection("RabbitMqSettings:Hosting").Value;
    var virtualHosting = configuration.GetSection("RabbitMqSettings:VirtualHost").Value;
    var username = configuration.GetSection("RabbitMqSettings:UserName").Value;
    var password = configuration.GetSection("RabbitMqSettings:Password").Value;
    var port = int.Parse(configuration.GetSection("RabbitMqSettings:Port").Value);

    connectionFactory = new ConnectionFactory
    {
      UserName = username,
      Password = password,
      HostName = hosting,
      Port = port,
      VirtualHost = virtualHosting,
      DispatchConsumersAsync = true
    };

  }

  public void ConnectRabbitMq()
  {
#if !DEBUG

        var exchange = configuration.GetSection("RabbitMqSettings:Exchange").Value;
        var queue = configuration.GetSection("RabbitMqSettings:Queue").Value;

        connection = connectionFactory.CreateConnection();
        channel = connection.CreateModel();
        channel.ExchangeDeclare(
            exchange: exchange,
            type: "direct",
            durable: true,
            autoDelete: false);
        channel.QueueDeclare(
            queue: queue,
            durable: true,
            exclusive: false,
            autoDelete: false);
#endif
  }

  public void Publish(RequestBaseModel message)
  {
    var exchange = configuration.GetSection("RabbitMqSettings:Exchange").Value;

    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
    var properties = channel.CreateBasicProperties();
    properties.ContentType = "application/json";
    properties.DeliveryMode = 1;
    properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

    channel.BasicPublish(
        exchange: exchange,
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

public static class RabbitMqClientExtension
{
  public static RabbitMqClient Initialize(this RabbitMqClientImp client)
  {
    client.ConnectRabbitMq();
    return client;
  }
}