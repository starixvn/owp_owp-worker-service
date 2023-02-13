namespace OWP.RabbitMq.Models;

using Consts;

public class LinkedMatchNotifyRequest : RequestBaseModel
{
    public string SessionId { get; set; }

    public DateTime EventDate { get; set; }

    public LinkedMatchNotifyRequest()
    {
        RoutingKey = RoutingKeys.LinkedLgaMatch;
    }
}