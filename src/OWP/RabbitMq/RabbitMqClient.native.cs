namespace OWP.RabbitMq;

using Fanex.Logging;
using Microsoft.Extensions.Options;
using Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

public interface RabbitMqClient
{
    void PublishRequestMessage(RequestBaseModel request);

    void ConnectRabbitMq();
}

public class RabbitMqClientImp : RabbitMqClient
{
    private IModel channel;
    private readonly RabbitMqSetting setting;
    private readonly ILogger logger;

    public RabbitMqClientImp(IOptions<RabbitMqSetting> options, ILogger logger)
    {
        setting = options.Value;
        this.logger = logger;
    }

    public void ConnectRabbitMq()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                UserName = setting.UserName,
                Password = setting.Password,
                HostName = setting.Hosting,
                Port = setting.Port,
                VirtualHost = setting.VirtualHost,
                DispatchConsumersAsync = true
            };

            var connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.QueueDeclare(queue: setting.RequestQueue, durable: true, exclusive: false, autoDelete: false);
        }
        catch (Exception ex)
        {
            logger.Error($"Connect RabbitMQ failed, exception: {ex}");
        }
    }

    public void PublishRequestMessage(RequestBaseModel request)
    {
        const int EXPIRATION_TIME = 1000 * 60 * 30;
        var json = JsonConvert.SerializeObject(request);
        var messageBytes = Encoding.UTF8.GetBytes(json);

        var props = channel.CreateBasicProperties();
        props.Type = request.RoutingKey;
        props.Expiration = EXPIRATION_TIME.ToString();

        channel.BasicPublish(
            exchange: "",
            routingKey: setting.RequestQueue,
            basicProperties: props,
            body: messageBytes);
    }
}
