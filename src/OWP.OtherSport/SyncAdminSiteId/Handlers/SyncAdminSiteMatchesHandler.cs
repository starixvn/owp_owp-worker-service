namespace OWP.OtherSport.SyncAdminSiteId.Handlers;

using Criteria;
using Fanex.Data.Repository;
using Helpers;
using Models;
using Olympus.Xml;
using SyncLive;

public interface SyncAdminSiteMatchesHandler
{
    Task Handle(DateTime eventDate);
}

public class SyncAdminSiteMatchesHandlerImp : SyncAdminSiteMatchesHandler
{
    private readonly IDynamicRepository dynamicRepository;
    private readonly SyncLiveToAdminSiteHandler syncLiveService;

    public SyncAdminSiteMatchesHandlerImp(IDynamicRepository repository, SyncLiveToAdminSiteHandler syncLiveService)
    {
        dynamicRepository = repository;
        this.syncLiveService = syncLiveService;
    }

    public async Task Handle(DateTime eventDate)
    {
        var owpMatches = await GetOwpMatches(eventDate);
        var adminSiteMatches = await GetAdminSiteMatches(eventDate);
        var unlinkedMatches = owpMatches.Where(x => !x.AdminSiteMatchId.HasValue).ToList();
        var linkedMatches = owpMatches.Where(x => x.AdminSiteMatchId.HasValue).ToList();

        var newLinkedMatches = MatchHelper.GetNewLinkedMatches(unlinkedMatches, adminSiteMatches).ToList();
        var updatedMatches = MatchHelper.GetRemovedMatches(linkedMatches, adminSiteMatches).ToList();

        await UpdateUnlinkedMatches(newLinkedMatches);
        await UpdateLinkedMatches(updatedMatches);

        // Sync live to infosite
        var newOwpMatches = await GetOwpMatches(eventDate);
        await syncLiveService.Handle(adminSiteMatches, newOwpMatches, eventDate);
    }
    
    private async Task<IList<Match>> GetOwpMatches(DateTime eventDate)
        => (await dynamicRepository.FetchAsync<Match>(new GetMatchesFromOwpCriteria { EventDate = eventDate })).ToList();

    private async Task<IList<Match>> GetAdminSiteMatches(DateTime eventDate)
        => (await dynamicRepository.FetchAsync<Match>(new GetMatchesFromAdminSiteCriteria { EventDate = eventDate })).ToList();

    private async Task UpdateUnlinkedMatches(IList<LinkedMatch> matches)
    {
        if (matches.Any())
        {
            var criteria = new UpdateUnlinkedMatchesCriteria
            {
                UnlinkedMatches = XmlConverter.Serialize(matches)
            };
            await dynamicRepository.ExecuteAsync(criteria);
        }
    }

    private async Task UpdateLinkedMatches(IList<LinkedMatch> matches)
    {
        if (matches.Any())
        {
            var criteria = new UpdateLinkedMatchesCriteria
            {
                LinkedMatches = XmlConverter.Serialize(matches)
            };
            await dynamicRepository.ExecuteAsync(criteria);
        }
    }
}