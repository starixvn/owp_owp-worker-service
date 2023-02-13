namespace OWP.InfositeIdSync.Services;

using Criteria;
using Fanex.Data.Repository;
using Infosite.Services;
using Models;
using Olympus.Xml;

public interface InfositeIdMappingRepository
{
    Task<List<OwpMatch>> QueryUnmappingOwpMatch();

    Task<List<InfositeMatch>> QueryInfositeMatch();

    Task UpdateInfositeIdForMainMatches(List<MatchMappingWithInfosite> matches);

    Task<List<SyncIsLiveModel>> GetInfositeMatchIsLive(DateTime fromTime, DateTime toTime);

    Task<List<MainDbMatch>> GetOfferLiveStatusFromMainDb(string matchIds);

    Task<AssignmentSnapshot> QueryMatchesByDate(DateTime eventDate);
}

public class InfositeIdMappingRepositoryImp : InfositeIdMappingRepository
{
    private readonly IDynamicRepository repository;
    private readonly DateTimeService dateTimeService;
    private readonly LogServiceWrapper logService;

    public InfositeIdMappingRepositoryImp(IDynamicRepository repository, DateTimeService dateTimeService, LogServiceWrapper logService)
    {
        this.repository = repository;
        this.dateTimeService = dateTimeService;
        this.logService = logService;
    }

    public async Task<List<SyncIsLiveModel>> GetInfositeMatchIsLive(DateTime fromTime, DateTime toTime)
    {
        var criteria = new GetInfoSiteMatchIsLiveCriteria { FromTime = fromTime, ToTime = toTime };
        return (await repository.FetchAsync<SyncIsLiveModel>(criteria)).ToList();
    }

    public async Task<List<MainDbMatch>> GetOfferLiveStatusFromMainDb(string matchIds)
    {
        var criteria = new GetMatchesFromMainDbCriteria { AdminSiteIds = matchIds };
        return (await repository.FetchAsync<MainDbMatch>(criteria)).ToList();
    }

    public async Task<List<InfositeMatch>> QueryInfositeMatch()
    {
        var criteria = new QueryInfositeMatchCriteria
        {
            FromDate = dateTimeService.Today
        };

        return (await repository.FetchAsync<InfositeMatch>(criteria)).ToList();
    }

    public async Task<List<OwpMatch>> QueryUnmappingOwpMatch()
    {
        var criteria = new QueryUnmappingOwpMatchCriteria
        {
            FromDate = dateTimeService.Today
        };

        return (await repository.FetchAsync<OwpMatch>(criteria)).ToList();
    }

    public async Task UpdateInfositeIdForMainMatches(List<MatchMappingWithInfosite> matches)
    {
        if (matches.Any())
        {
            await logService.Log("UpdateInfositeIdForMainMatches", matches);
        }

        var xml = XmlConverter.Serialize(matches);
        var command = new UpdateInfositeIdCommand
        {
            Xml = xml
        };
        await repository.ExecuteAsync(command).ConfigureAwait(false);
    }

    public async Task<AssignmentSnapshot> QueryMatchesByDate(DateTime eventDate)
    {
        var matchesTask = repository.FetchAsync<GeneralMatch>(new GetAllMatchesByTimeRangeCriteria { EventDate = eventDate });

        return new AssignmentSnapshot
        {
            Matches = (await matchesTask).ToDictionary(m => m.MatchId)
        };
    }
}