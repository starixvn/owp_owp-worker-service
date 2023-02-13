using OWP.InfositeIdSync.Helpers;
using OWP.Infosite.Services;
using System.Globalization;
using OWP.RabbitMq;
using OWP.RabbitMq.Models;

namespace OWP.InfositeIdSync.Services;

public interface SyncInfositeIdService
{
    Task Sync();
}

public class SynInfositeIdServiceImp : SyncInfositeIdService
{
    private readonly InfositeIdMappingRepository repository;
    private readonly DateTimeService dateTimeService;
    private readonly CacheManipulator cacheManipulator;

    private readonly RabbitMqClient rabbitMqClient;

    public SynInfositeIdServiceImp(
        InfositeIdMappingRepository repository,
        DateTimeService dateTimeService,
        CacheManipulator cacheManipulator,
        RabbitMqClient rabbitMqClient)
    {
        this.repository = repository;
        this.dateTimeService = dateTimeService;
        this.cacheManipulator = cacheManipulator;
        this.rabbitMqClient = rabbitMqClient;
    }

    public async Task Sync()
    {
        var owpMatches = await repository.QueryUnmappingOWPMatch();
        var infositeMatches = await repository.QueryInfositeMatch();
        var mappingMatches = InfoSiteHelper.Map(owpMatches, infositeMatches);

        var sessionId = Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture);
        await SaveOldSnapshot(sessionId);

        await repository.UpdateInfositeIdForMainMatches(mappingMatches);

        PublishLinkedMatchNotification(sessionId);
    }

    public async Task SaveOldSnapshot(string sessionId)
    {
        var oldSnapshot = await repository.QueryMatchesByDate(dateTimeService.Today);
        await cacheManipulator.SaveOldAssignmentSnapshot(sessionId, oldSnapshot);
    }

    public void PublishLinkedMatchNotification(string sessionId)
    {
        var notifyRequest = new LinkedMatchNotifyRequest
        {
            EventDate = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Unspecified),
            SessionId = sessionId
        };

        rabbitMqClient.PublishRequestMessage(notifyRequest);
    }
}