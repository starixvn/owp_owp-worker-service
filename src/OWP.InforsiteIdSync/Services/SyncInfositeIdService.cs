using Microsoft.Extensions.Options;
using OWP.InfositeIdSync.Helpers;
using OWP.Infosite.Services;
using OWP.Infosite.Settings;
using System.Globalization;
using OWP.Core.RabbitMq;
using OWP.Core.RabbitMq.Models;
using OWP.Core.Services;

namespace OWP.InfositeIdSync.Services;

public interface SyncInfositeIdService
{
  Task Sync();
}

public class SynInfositeIdServiceImp : SyncInfositeIdService
{
  private readonly InfoSiteSettings infoSiteSettings;
  private readonly InfositeIdMappingRepository repository;
  private readonly DateTimeService dateTimeService;
  private readonly CacheManipulator cacheManipulator;
  private readonly HttpHandler httpHandler;

  private readonly RabbitMqClient rabbitMqClient;

  public SynInfositeIdServiceImp(
      IOptions<InfoSiteSettings> infoSiteSettings,
      InfositeIdMappingRepository repository,
      DateTimeService dateTimeService,
      CacheManipulator cacheManipulator,
      HttpHandler httpHandler,
      RabbitMqClient rabbitMqClient)
  {
    this.infoSiteSettings = infoSiteSettings.Value;
    this.repository = repository;
    this.dateTimeService = dateTimeService;
    this.cacheManipulator = cacheManipulator;
    this.httpHandler = httpHandler;
    this.rabbitMqClient = rabbitMqClient;
  }

  public async Task Sync()
  {
    var sessionId = Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture);
    await SaveOldSnapshot(sessionId);

    var owpMatches = await repository.QueryUnmappingOWPMatch();
    var infositeMatches = await repository.QueryInfositeMatch();
    var mappingMatches = InfoSiteHelper.Map(owpMatches, infositeMatches);

    await repository.UpdateInfositeIdForMainMatches(mappingMatches);

#if !DEBUG
        TriggerNetCoreLinkedMatchNotification(sessionId);
#endif
  }

  public async Task SaveOldSnapshot(string sessionId)
  {
    var oldSnapshot = await repository.QueryMatchesByDate(dateTimeService.Today);
    await cacheManipulator.SaveOldAssignmentSnapshot(sessionId, oldSnapshot);
  }

  public void TriggerNetCoreLinkedMatchNotification(string sessionId)
  {
    var notifyRequest = new LinkedMatchNotifyRequest
    {
      EventDate = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Unspecified),
      SessionId = sessionId
    };

    rabbitMqClient.Publish(notifyRequest);
  }
}