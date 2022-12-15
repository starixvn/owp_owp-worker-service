using Fanex.Data.Repository;
using Fanex.Logging;
using Olympus.Xml;
using OWP.OtherSport.SyncAdminSiteId.Criteria;
using OWP.OtherSport.SyncAdminSiteId.Helpers;
using OWP.OtherSport.SyncAdminSiteId.Models;
using OWP.OtherSport.SyncLive;
using OWP.OtherSport.SyncLive.Services;

namespace OWP.OtherSport.SyncAdminSiteId.Handlers;

public interface SyncAdminSiteMatchesHandler
{
    Task Handle(DateTime eventDate);
}

public class SyncAdminSiteMatchesHandlerImp : SyncAdminSiteMatchesHandler
{
    private readonly IDynamicRepository dynamicRepository;
    private readonly SyncLiveToAdminSiteHandler syncLiveService;
    private readonly CacheManipulator cacheManipulator;

    public SyncAdminSiteMatchesHandlerImp(IDynamicRepository repository, SyncLiveToAdminSiteHandler syncLiveService, CacheManipulator cacheManipulator)
    {
        dynamicRepository = repository;
        this.syncLiveService = syncLiveService;
        this.cacheManipulator = cacheManipulator;
    }

    public async Task Handle(DateTime eventDate)
    {
        var owpMatches = await GetOWPMatches(eventDate);
        var adminSiteMatches = await GetAdminSiteMatches(eventDate);
        var unlinkedMatches = owpMatches.Where(x => !x.AdminSiteMatchId.HasValue);
        var linkedMatches = owpMatches.Where(x => x.AdminSiteMatchId.HasValue);

        var newLinkedMatches = MatchHelper.GetNewLinkedMatches(unlinkedMatches, adminSiteMatches).ToList();
        var updatedMatches = MatchHelper.GetRemovedMatches(linkedMatches, adminSiteMatches);

        var newLinkedDuplicatedMatches = newLinkedMatches.Where(x => !CheckDuplicateAdminSiteId(newLinkedMatches, x.AdminSiteMatchId));
        var excludeDuplicatedMatches = newLinkedMatches.Except(newLinkedDuplicatedMatches);

        await cacheManipulator.SaveMathIdsDuplicatedSnapshot(newLinkedDuplicatedMatches.Select(x=>x.AdminSiteMatchId));

        await UpdateUnlinkedMatches(excludeDuplicatedMatches);
        await UpdateLinkedMatches(updatedMatches);

        // Sync live to infosite
        var newOwpMatches = await GetOWPMatches(eventDate);
        await syncLiveService.Handle(adminSiteMatches, newOwpMatches, eventDate);
    }
    public bool CheckDuplicateAdminSiteId(IEnumerable<LinkedMatch> match, int? id)
        => match.Count(x => x.AdminSiteMatchId == id) == 1;

    private async Task<IEnumerable<Match>> GetOWPMatches(DateTime eventDate)
        => await dynamicRepository.FetchAsync<Match>(new GetMatchesFromOWPCriteria { EventDate = eventDate });

    private async Task<IEnumerable<Match>> GetAdminSiteMatches(DateTime eventDate)
        => await dynamicRepository.FetchAsync<Match>(new GetMatchesFromAdminSiteCriteria { EventDate = eventDate });

    private async Task UpdateUnlinkedMatches(IEnumerable<LinkedMatch> matches)
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

    private async Task UpdateLinkedMatches(IEnumerable<LinkedMatch> matches)
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