namespace OWP.RabbitMq.Models;

using Consts;

public class PrematchNewLeagueNotifyRequest : RequestBaseModel
{
    public string SessionId { get; set; }

    public DateTime FromDate { get; set; }

    public PrematchNewLeagueNotifyRequest()
    {
        RoutingKey = RoutingKeys.PrematchNewLeague;
    }
}