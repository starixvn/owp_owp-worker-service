namespace OWP.Core.RabbitMq.Models;

public class PrematchNewLeagueNotifyRequest
{
  public string SessionId { get; set; }

  public DateTime FromDate { get; set; }
}