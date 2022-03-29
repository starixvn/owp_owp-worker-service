namespace OWP.Core.RabbitMq.Models;

public class RequestBaseModel
{
    public string RoutingKey { get; protected set; }
}