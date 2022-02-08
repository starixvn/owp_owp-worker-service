using Microsoft.Extensions.Options;
using Olympus.InfoSiteClient;
using OWP.Infosite.Settings;
using OWP.InfositeIdSync.Helpers;
using OWP.InfositeIdSync.Models;

namespace OWP.InfositeIdSync.Services;

public interface SyncIsLiveService
{
    Task Sync(DateTime fromTime, DateTime toTime);
}

public class SyncIsLiveServiceImp : SyncIsLiveService
{
    private readonly InfositeIdMappingRepository repository;
    private readonly LogServiceWrapper logService;
    private readonly CacheManipulator cacheManipulator;
    private readonly IsLiveMatchApi liveMatchApi;
    private readonly InfoSiteSettings infoSiteSettings;

    public SyncIsLiveServiceImp(InfositeIdMappingRepository repository,
        LogServiceWrapper logService,
        CacheManipulator cacheManipulator,
        IsLiveMatchApi liveMatchApi,
        IOptions<InfoSiteSettings> infoSiteSettings)
    {
        this.repository = repository;
        this.logService = logService;
        this.cacheManipulator = cacheManipulator;
        this.liveMatchApi = liveMatchApi;
        this.infoSiteSettings = infoSiteSettings.Value;
    }

    public async Task Sync(DateTime fromTime, DateTime toTime)
    {
        var owpMatches = await repository.GetInfositeMatchIsLive(fromTime, toTime);
        var adminSiteMatchIds = owpMatches.Select(x => x.AdminSiteMatchId).Distinct();
        var mainDBMatches = await repository.GetOfferLiveStatusFromMainDB(string.Join(',', adminSiteMatchIds));

        var unsynchorizedMatches = MatchHelper.GetUnsynchorizedMatches(owpMatches, mainDBMatches);

        var deletedMatches = await cacheManipulator.GetDeletedMatches();
        var unsynchronizedDeletedMatches = deletedMatches
            .Select(x => new SyncIsLiveModel { InfoSiteMatchId = x.MatchId, MinorBettypeId = x.MinorBettypeId })
            .Except(unsynchorizedMatches, new SyncIsLiveModel()).ToList();

        var liveMatches = unsynchorizedMatches.Where(m => m.IsLive).Distinct();
        var nonLiveMatches = unsynchorizedMatches.Where(m => !m.IsLive)
                                        .Union(unsynchronizedDeletedMatches)
                                        .Distinct();

        var distinctLiveMatches = MatchHelper.GetDistinctLiveMatches(liveMatches, nonLiveMatches);
        var distinctNonLiveMatches = MatchHelper.GetDistinctNonLiveMatches(liveMatches, nonLiveMatches);

        var synchronizedLiveMatchResult = await liveMatchApi.SetLiveMatches(infoSiteSettings.InfositeAPI, distinctLiveMatches);
        var synchronizedNonLiveMatchResult = await liveMatchApi.SetNonLiveMatches(infoSiteSettings.InfositeAPI, distinctNonLiveMatches);
        var failedMatchesWithMinorBettypeId = synchronizedNonLiveMatchResult.FailedMatches
            .Select(x => (x.Item1, MatchHelper.GetOWPMinorMatchIdFromSubtype(x.Item2)));

        var successfulNonLiveMatches = deletedMatches.Select(x => (x.MatchId, x.MinorBettypeId)).Except(failedMatchesWithMinorBettypeId).ToList();
        await cacheManipulator.SaveDeletedMatches(successfulNonLiveMatches);

        await logService.LogSynIsLive(
            unsynchorizedMatches,
            deletedMatches.Select(x => (x.MatchId, x.MinorBettypeId)),
            liveMatches.Select(x => (x.InfoSiteMatchId, x.MinorBettypeId)),
            nonLiveMatches.Select(x => (x.InfoSiteMatchId, x.MinorBettypeId)),
            synchronizedLiveMatchResult,
            synchronizedNonLiveMatchResult);
    }
}