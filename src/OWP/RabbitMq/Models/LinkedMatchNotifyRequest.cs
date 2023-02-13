using OWP.Core.RabbitMq.Consts;

namespace OWP.Core.RabbitMq.Models;

public class LinkedMatchNotifyRequest : RequestBaseModel
{
    public string SessionId { get; set; }

    public DateTime EventDate { get; set; }

    public LinkedMatchNotifyRequest()
    {
        RoutingKey = RoutingKeys.LinkedLgaMatch;
    }
}