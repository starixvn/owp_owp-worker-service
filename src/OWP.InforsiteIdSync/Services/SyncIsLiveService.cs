namespace OWP.InfositeIdSync.Services;

using Fanex.Logging;
using Helpers;
using Infosite.Settings;
using Microsoft.Extensions.Options;
using Models;
using Newtonsoft.Json;
using Olympus.InfoSiteClient;

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
        var mainDbMatches = await repository.GetOfferLiveStatusFromMainDb(string.Join(',', adminSiteMatchIds));

        var unsynchronizedMatches = MatchHelper.GetUnsynchronizedMatches(owpMatches, mainDbMatches);

        var deletedMatches = await cacheManipulator.GetDeletedMatches();
        var unsynchronizedDeletedMatches = deletedMatches
            .Select(x => new SyncIsLiveModel { InfoSiteMatchId = x.MatchId, MinorBetTypeId = x.MinorBettypeId })
            .Except(unsynchronizedMatches, new SyncIsLiveModel()).ToList();

        var liveMatches = unsynchronizedMatches.Where(m => m.IsLive).Distinct().ToList();
        var nonLiveMatches = unsynchronizedMatches.Where(m => !m.IsLive)
                                        .Union(unsynchronizedDeletedMatches)
                                        .Distinct()
                                        .ToList();

        var distinctLiveMatches = MatchHelper.GetDistinctLiveMatches(liveMatches, nonLiveMatches).ToList();
        var distinctNonLiveMatches = MatchHelper.GetDistinctNonLiveMatches(liveMatches, nonLiveMatches).ToList();

        await LogService.InfoAsync($"begin sync live to infosite {JsonConvert.SerializeObject(distinctLiveMatches)}");
        var synchronizedLiveMatchResult = await liveMatchApi.SetLiveMatches(infoSiteSettings.InfositeApi, distinctLiveMatches);
        await LogService.InfoAsync("end sync live to infosite");
        var synchronizedNonLiveMatchResult = await liveMatchApi.SetNonLiveMatches(infoSiteSettings.InfositeApi, distinctNonLiveMatches);
        var failedMatchesWithMinorBettypeId = synchronizedNonLiveMatchResult.FailedMatches
            .Select(x => (x.Item1, MatchHelper.GetOwpMinorMatchIdFromSubtype(x.Item2)));

        var successfulNonLiveMatches = deletedMatches.Select(x => (x.MatchId, x.MinorBettypeId)).Except(failedMatchesWithMinorBettypeId).ToList();
        await cacheManipulator.SaveDeletedMatches(successfulNonLiveMatches);

        await logService.LogSynIsLive(
            unsynchronizedMatches,
            deletedMatches.Select(x => (x.MatchId, x.MinorBettypeId)).ToList(),
            liveMatches.Select(x => (x.InfoSiteMatchId, MinorBettypeId: x.MinorBetTypeId)).ToList(),
            nonLiveMatches.Select(x => (x.InfoSiteMatchId, MinorBettypeId: x.MinorBetTypeId)).ToList(),
            synchronizedLiveMatchResult,
            synchronizedNonLiveMatchResult);
    }
}