using Fanex.Caching;
using Fanex.Logging;
using StackExchange.Redis;

namespace OWP.Core.RedisPubSub;

public interface PubSubEvent
{
  void Publish(string sessionId);
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

  public void Publish(string sessionId)
  {
    var cachePolicy = new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1));
    var numberClient = subscriber.Publish("messages", sessionId);
    logger.Info($"Redis public Connect numberClient: {numberClient}");
    cache.Set($"Redis public-{sessionId}", sessionId, cachePolicy);
  }
}