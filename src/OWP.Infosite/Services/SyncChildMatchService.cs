namespace OWP.Infosite.Services;

using Builder;
using Criteria;
using Extensions;
using Fanex.Data.Repository;
using Helper;
using Microsoft.Extensions.Options;
using Models;
using Newtonsoft.Json;
using Olympus.InfoSiteClient;
using Olympus.InfoSiteClient.Models;
using Settings;

public interface SyncChildMatchService
{
    Task Sync(DateTime date);
}

public class SyncChildMatchServiceImp : SyncChildMatchService
{
    private readonly InfoSiteSettings infoSiteSettings;
    private readonly PullingChildMatch pullingChildMatch;
    private readonly IDynamicRepository repository;

    public SyncChildMatchServiceImp(IOptions<InfoSiteSettings> infoSiteSettings, IDynamicRepository repository)
    {
        this.infoSiteSettings = infoSiteSettings.Value;
        pullingChildMatch = new PullingChildMatch();
        this.repository = repository;
    }

    public async Task Sync(DateTime date)
    {
        var childMatches = pullingChildMatch.Query(infoSiteSettings.InfositeAPI, date, date.AddDays(1));
        var mappedChildMatches = MapChildMatches(childMatches);
        mappedChildMatches = new SpecialMatchBuilder().ReduceSpecialMatch(mappedChildMatches);

        await SaveInfoSiteChildMatchCommand(mappedChildMatches);
    }

    public IEnumerable<MappedChildMatch> MapChildMatches(IEnumerable<ChildMatch> childMatches)
    {
        childMatches = FilterMatch(childMatches);
        foreach (var childMatch in childMatches)
        {
            var (representativeSubtype, minorBetTypeId) = ChildMatchHelper.GetBetTypeIdAndRepresentativeSubtype(childMatch.sub_type);
            if (minorBetTypeId < 0)
            {
                yield return new MappedChildMatch
                {
                    ParentInfoSiteMatchId = (int)childMatch.match_id,
                    AdminSiteMatchId = (int?)childMatch.create_match_id,
                    LeagueName = childMatch.league_name,
                    HomeName = childMatch.home_name,
                    AwayName = childMatch.away_name,
                    EventDateTime = (childMatch.game_date ?? new DateTime(2000, 1, 1)).ToUtc8(),
                    MinorBettypeId = minorBetTypeId,
                    SubType = childMatch.sub_type,
                    RepresentativeSubtype = representativeSubtype,
                    UpdatedTime = childMatch.stamp.ToUtc8()
                };
            }
        }
    }

    private static IEnumerable<ChildMatch> FilterMatch(IEnumerable<ChildMatch> childMatches)
        => childMatches.Where(x =>
            x.match_id > 0 &&
            !IsETPKMatch(x.home_name) &&
            IsValidSubType(x.sub_type) &&
            x.main_type.HasValue);

    private static bool IsETPKMatch(string homeName)
        => homeName.IsContainsIgnoreCase("(ET)") || homeName.IsContainsIgnoreCase("(PEN)");

    private static bool IsValidSubType(int? subType) => subType is > 0;

    public async Task SaveInfoSiteChildMatchCommand(IEnumerable<MappedChildMatch> childMatches)
    {
        var criteria = new PushInfoSiteChildMatchCommand
        {
            ChildMatchesJson = JsonConvert.SerializeObject(childMatches)
        };

        await repository.ExecuteAsync(criteria);
    }
}