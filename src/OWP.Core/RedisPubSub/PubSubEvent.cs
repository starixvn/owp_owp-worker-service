using Fanex.Logging;
using Newtonsoft.Json;
using OWP.Core.RabbitMq.Models;
using OWP.Core.RedisPubSub.Constants;
using StackExchange.Redis;

namespace OWP.Core.RedisPubSub;

public interface PubSubEvent
{
  void Publish(PrematchNewLeagueNotifyRequest request);
}
public class PubSubEventImp : PubSubEvent
{
  private readonly ILogger logger;

  private readonly ISubscriber subscriber;

  public PubSubEventImp(ILogger logger, IConnectionMultiplexer redis)
  {
    this.logger = logger;
    this.subscriber = redis.GetSubscriber();
  }

  public void Publish(PrematchNewLeagueNotifyRequest request)
  {
    var mesage = JsonConvert.SerializeObject(request);
    var numberClient = subscriber.Publish(PubSubConst.NEW_LEAGUE_CHANEL, mesage);
    logger.Info($"Redis public-{request.SessionId}-{DateTime.Now}-{mesage}");
    logger.Info($"Redis public Connect numberClient: {numberClient}");
  }
}