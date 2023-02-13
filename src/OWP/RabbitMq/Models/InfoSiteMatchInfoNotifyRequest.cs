using OWP.Core.RabbitMq.Consts;

namespace OWP.Core.RabbitMq.Models;

public class InfoSiteMatchInfoNotifyRequest : RequestBaseModel
{
    public string SessionId { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public InfoSiteMatchInfoNotifyRequest() {
        RoutingKey = RoutingKeys.InfoSiteMatchInfo;
    }
}