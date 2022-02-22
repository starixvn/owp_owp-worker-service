using Microsoft.Extensions.Options;
using OWP.InfositeIdSync.Helpers;
using OWP.Infosite.Services;
using OWP.Infosite.Settings;
using System.Globalization;

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
    public SynInfositeIdServiceImp(
        IOptions<InfoSiteSettings> infoSiteSettings,
        InfositeIdMappingRepository repository,
        DateTimeService dateTimeService,
        CacheManipulator cacheManipulator,
        HttpHandler httpHandler)
    {
        this.infoSiteSettings = infoSiteSettings.Value;
        this.repository = repository;
        this.dateTimeService = dateTimeService;
        this.cacheManipulator = cacheManipulator;
        this.httpHandler = httpHandler;
    }

    public async Task Sync()
    {
        var sessionId = Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture);
        await SaveOldSnapshot(sessionId);

        var owpMatches = await repository.QueryUnmappingOWPMatch();
        var infositeMatches = await repository.QueryInfositeMatch();
        var mappingMatches = InfoSiteHelper.Map(owpMatches, infositeMatches);

        await repository.UpdateInfositeIdForMainMatches(mappingMatches);

        //Trigger net core
        await TriggerNetCoreLinkedMatchNotification(sessionId);
    }

    public async Task SaveOldSnapshot(string sessionId)
    {
        var oldSnapshot = await repository.QueryMatchesByDate(dateTimeService.Today);
        await cacheManipulator.SaveOldAssignmentSnapshot(sessionId, oldSnapshot);
    }

    public async Task TriggerNetCoreLinkedMatchNotification(string sessionId)
    {
        var requestLink = $"{infoSiteSettings.OWPNetCoreUri}/PreMatchNotification/notify-linked-lga-match?sessionId={sessionId}&eventDate={dateTimeService.Today}";
        await httpHandler.GetAsync(requestLink);
    }
}