using Fanex.Data.Repository;
using Olympus.Xml;
using OWP.OtherSport.SyncAdminSiteId.Criteria;
using OWP.OtherSport.SyncAdminSiteId.Helpers;
using OWP.OtherSport.SyncAdminSiteId.Models;
using OWP.OtherSport.SyncLive;

namespace OWP.OtherSport.SyncAdminSiteId.Handlers;

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
        var owpMatches = await GetOWPMatches(eventDate);
        var adminSiteMatches = await GetAdminSiteMatches(eventDate);
        var unlinkedMatches = owpMatches.Where(x => !x.AdminSiteMatchId.HasValue);
        var linkedMatches = owpMatches.Where(x => x.AdminSiteMatchId.HasValue);

        var newLinkedMatches = MatchHelper.GetNewLinkedMatches(unlinkedMatches, adminSiteMatches);
        var updatedMatches = MatchHelper.GetRemovedMatches(linkedMatches, adminSiteMatches);

        await UpdateUnlinkedMatches(newLinkedMatches);
        await UpdateLinkedMatches(updatedMatches);

        // Sync live to infosite
        var newOwpMatches = await GetOWPMatches(eventDate);
        await syncLiveService.Handle(adminSiteMatches, newOwpMatches, eventDate);
    }

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