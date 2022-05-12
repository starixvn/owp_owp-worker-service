using Fanex.Caching;
using Fanex.Logging;
using Newtonsoft.Json;
using OWP.Core.RabbitMq.Models;
using StackExchange.Redis;

namespace OWP.Core.RedisPubSub;

public interface PubSubEvent
{
  void Publish(PrematchNewLeagueNotifyRequest request);
}
public class PubSubEventImp : PubSubEvent
{
  private readonly ICache cache;

  private readonly ILogger logger;

  private readonly ISubscriber subscriber;

  public PubSubEventImp(ICache cache, ILogger logger, IConnectionMultiplexer redis)
  {
    this.cache = cache;
    this.logger = logger;
    this.subscriber = redis.GetSubscriber();
  }

  public void Publish(PrematchNewLeagueNotifyRequest request)
  {
    var cachePolicy = new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1));
    var mesage = JsonConvert.SerializeObject(request);
    var numberClient = subscriber.Publish("messages", mesage);
    cache.Set($"Redis public-{request.SessionId}-{DateTime.Now}", mesage, cachePolicy);
    logger.Info($"Redis public-{request.SessionId}-{DateTime.Now}-{mesage}");
    logger.Info($"Redis public Connect numberClient: {numberClient}");
  }
}