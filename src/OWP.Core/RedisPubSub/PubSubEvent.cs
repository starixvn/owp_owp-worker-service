using Fanex.Logging;
using Microsoft.Extensions.Configuration;
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

  private readonly IConfiguration config;

  public PubSubEventImp(ILogger logger, IConnectionMultiplexer redis, IConfiguration config)
  {
    this.logger = logger;
    this.subscriber = redis.GetSubscriber();
    this.config=config;
  }

  public void Publish(PrematchNewLeagueNotifyRequest request)
  {
    var channel= config.GetSection("RedisChannel:NewLeague").Value;
    var mesage = JsonConvert.SerializeObject(request);
    var numberClient = subscriber.Publish(channel, mesage);
    logger.Info("Redis channel: "+ channel);
    logger.Info($"Redis public-{request.SessionId}-{DateTime.Now}-{mesage}");
    logger.Info($"Redis public Connect numberClient: {numberClient}");
  }
}