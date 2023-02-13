using OWP.Core.RabbitMq.Consts;

namespace OWP.Core.RabbitMq.Models;

public class PrematchNewLeagueNotifyRequest: RequestBaseModel
{
    public string SessionId { get; set; }

    public DateTime FromDate { get; set; }

    public PrematchNewLeagueNotifyRequest()
    {
        RoutingKey = RoutingKeys.PrematchNewLeague;
    }
}