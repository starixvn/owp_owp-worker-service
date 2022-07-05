using Fanex.Caching;
using Fanex.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OWP.Core.Extensions;
using OWP.Core.RabbitMq;
using OWP.Core.RabbitMq.Models;
using OWP.Core.RedisPubSub;
using OWP.Core.Services;
using OWP.Infosite.Constants;
using OWP.Infosite.Models;
using OWP.Infosite.Settings;

namespace OWP.Infosite.Services;

public interface ApiService
{
  Task<List<CommonMatchModel>> QueryAsync(ParamsQueryModel param);

  void TriggerNetCoreMatchInfoNotification(string sessionId, DateTime fromDate, DateTime toDate);

  Task<List<MatchRelation>> QueryMatchesRelation(ParamsQueryModel param);

  void TriggerNetCoreNewLeagueNotification(string sessionId, DateTime fromDate);

  Task<List<string>> GetInvalidEventKeysByDateAsync(DateTime date);

  Task<List<MatchModelQuery180Day>> QueryInfoSiteMatchFor180Day();
}

public class ApiServiceImp : ApiService
{
  private readonly InfoSiteSettings infoSiteSettings;

  private readonly HttpHandler httpHandler;

  private readonly ILogger logger;

  private readonly RabbitMqClient rabbitMqClient;

  private readonly ICache cacheService;

  private readonly PubSubEvent pubSubEvent;

  public ApiServiceImp(HttpHandler httpHandler,
      ILogger logger,
      IOptions<InfoSiteSettings> infoSiteSettings,
      RabbitMqClient rabbitMqClient,
      ICache cacheService,
       PubSubEvent pubSubEvent)
  {
    this.httpHandler = httpHandler;
    this.logger = logger;
    this.infoSiteSettings = infoSiteSettings.Value;
    this.rabbitMqClient = rabbitMqClient;
    this.cacheService = cacheService;
    this.pubSubEvent = pubSubEvent;
  }

  public async Task<List<CommonMatchModel>> QueryAsync(ParamsQueryModel param)
  {
    var date_from = param.FromDate.ToString(Constant.DateTimeFormat);
    var date_to = param.ToDate.ToString(Constant.DateTimeFormat);
    var stamp = param.LastUpdatedTime.ToString(Constant.DateTimeWithHourFormat);

    var requestLink = $"{infoSiteSettings.InfositeAPI}api/EventSchedule/getEventsByDate?rd_checked=true&date_from={date_from}&date_to={date_to}&stamp={stamp}";

    var response = await httpHandler.GetAsync(requestLink);
    if (response.IsSuccessStatusCode)
    {
      var jsonData = await response.Content.ReadAsStringAsync();
      return JsonConvert.DeserializeObject<List<CommonMatchModel>>(jsonData, new TrimmingConverter());
    }
    else
    {
      var logInfo = new
      {
        DateTime = DateTime.UtcNow,
        RequestUri = requestLink,
        StatusCode = (int)response.StatusCode,
        response.RequestMessage
      };
      var logInfoString = JsonConvert.SerializeObject(logInfo);
      logger.Error(logInfoString);

      return new List<CommonMatchModel>();
    }
  }

  public async Task<List<MatchRelation>> QueryMatchesRelation(ParamsQueryModel param)
  {
    var fromDate = param.FromDate.ToString(Constant.DateTimeFormat);
    var toDate = param.ToDate.ToString(Constant.DateTimeFormat);

    var requestLink = $"{infoSiteSettings.InfositeAPI}api/EventSchedule/GetEventScheduleRelationForOWP?dateFrom={fromDate}&dateTo={toDate}";

    var response = await httpHandler.GetAsync(requestLink);

    if (response.IsSuccessStatusCode)
    {
      var jsonData = await response.Content.ReadAsStringAsync();
      return JsonConvert.DeserializeObject<List<MatchRelation>>(jsonData, new TrimmingConverter());
    }
    else
    {
      var logInfo = new
      {
        DateTime = DateTime.UtcNow,
        RequestUri = requestLink,
        StatusCode = (int)response.StatusCode,
        response.RequestMessage
      };

      var logInfoString = JsonConvert.SerializeObject(logInfo);
      logger.Error(logInfoString);

      return new List<MatchRelation>();
    }
  }

#pragma warning disable S1481 // Remove the unused local variable 'request', disabled by Tyler
  public void TriggerNetCoreMatchInfoNotification(string sessionId, DateTime fromDate, DateTime toDate)
  {
    var request = new InfoSiteMatchInfoNotifyRequest
    {
      SessionId = sessionId,
      FromDate = fromDate,
      ToDate = toDate
    };
#if !DEBUG
        rabbitMqClient.Publish(request);
#endif
  }

  public void TriggerNetCoreNewLeagueNotification(string sessionId, DateTime fromDate)
  {
    var request = new PrematchNewLeagueNotifyRequest
    {
      SessionId = sessionId,
      FromDate = fromDate
    };
#if !DEBUG
        pubSubEvent.Publish(request);
#endif
  }
#pragma warning restore S1481

  public async Task<List<string>> GetInvalidEventKeysByDateAsync(DateTime date)
  {
    var dateInString = date.ToString(Constant.DateTimeFormat);

    var requestLink = $"{infoSiteSettings.InfositeAPI}api/EventSchedule/getInvalidEventKeysByDate?rd_checked=true&date_from={dateInString}&date_to={dateInString}";

    var response = await httpHandler.GetAsync(requestLink);
    if (response.IsSuccessStatusCode)
    {
      var jsonData = await response.Content.ReadAsStringAsync();
      return JsonConvert.DeserializeObject<List<string>>(jsonData, new TrimmingConverter());
    }
    else
    {
      var logInfo = new
      {
        DateTime = DateTime.UtcNow,
        RequestPath = requestLink,
        StatusCode = (int)response.StatusCode,
        response.RequestMessage
      };
      var logInfoString = JsonConvert.SerializeObject(logInfo);
      logger.Error(logInfoString);

      return new List<string>();
    }
  }

  public async Task<List<MatchModelQuery180Day>> QueryInfoSiteMatchFor180Day()
  {
    var requestLink = $"{infoSiteSettings.Infosite180DayAPI}api/EventSchedule/GetEventScheduleForOWP?sport_id=1";
    var response = await httpHandler.GetAsync(requestLink);

    if (response.IsSuccessStatusCode)
    {
      var jsonData = await response.Content.ReadAsStringAsync();
      return JsonConvert.DeserializeObject<List<MatchModelQuery180Day>>(jsonData, new TrimmingConverter());
    }
    else
    {
      var logInfo = new
      {
        DateTime = DateTime.UtcNow,
        RequestUri = requestLink,
        StatusCode = (int)response.StatusCode,
        response.RequestMessage
      };
      var logInfoString = JsonConvert.SerializeObject(logInfo);
      logger.Error(logInfoString);

      return new List<MatchModelQuery180Day>();
    }
  }
}