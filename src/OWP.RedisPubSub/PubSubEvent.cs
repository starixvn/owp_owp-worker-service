using Fanex.Caching;
using Fanex.Logging;
using StackExchange.Redis;

namespace OWP.RedisPubSub;
public interface PubSubEvent
{
  void Publish();

  void Subscribe();
}
public class PubSubEventImp : PubSubEvent
{
  private readonly ICache cache;

  private readonly ILogger logger;

  public PubSubEventImp(ICache cache, ILogger logger)
  {
    this.cache = cache;
    this.logger = logger;
    Subscribe();
  }

  public void Publish()
  {
    
  }

  public void Subscribe()
  {
    logger.Info("Redis Subscribe start");
    var cachePolicy = new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1));
    using (var redis = ConnectionMultiplexer.Connect("ha.nexdev.net:16379,password=foo1@bared,defaultDatabase=11"))
    {
      logger.Info("Redis Connect");
      var sub = redis.GetSubscriber();
      sub.Subscribe("messages", (channel, message) =>
      {
        cache.Set($"RedisPubSub-Subscribe-{message}", message, cachePolicy);
      });
    }
  }
}