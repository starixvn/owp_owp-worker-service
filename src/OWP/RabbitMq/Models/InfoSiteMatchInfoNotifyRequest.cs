namespace OWP.RabbitMq.Models;

using Consts;

public class InfoSiteMatchInfoNotifyRequest : RequestBaseModel
{
    public string SessionId { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public InfoSiteMatchInfoNotifyRequest() {
        RoutingKey = RoutingKeys.InfoSiteMatchInfo;
    }
}