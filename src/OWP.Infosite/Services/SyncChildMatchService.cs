using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Olympus.InfoSiteClient;
using Olympus.InfoSiteClient.Models;
using Olympus.OWP.Constants;
using OWP.Core.Extensions;
using OWP.Core.Mics;
using OWP.Infosite.Builder;
using OWP.Infosite.Criteria;
using OWP.Infosite.Models;
using OWP.Infosite.Settings;

namespace OWP.Infosite.Services;

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
        mappedChildMatches = SpecialMatchBuilder.ReduceSpecialMatch(mappedChildMatches);

        await SaveInfoSiteChildMatchCommand(mappedChildMatches);
    }

    public static (int?, int?) GetOWPMinorMatchIdFromSubtype(int? subType)
    {
        var firstType = new Specific10MinsDetector();
        var secondType = new Specific15MinsDetector();
        var thirdType = new NextTypesDetector();
        var fourthType = new SingleTeamPenaltyShootOut();
        var fifthType = new WinnerAndNextRound();

        firstType.NextType = secondType;
        secondType.NextType = thirdType;
        thirdType.NextType = fourthType;
        fourthType.NextType = fifthType;
        var representativeSubtype = firstType.GetRepresentativeSubtype(subType.Value);
        var minorBettypeId = MinorBettype.GetMinorBettypeIdBySubtype(representativeSubtype);

        return (representativeSubtype, minorBettypeId < 0 ? minorBettypeId : null);
    }

    public IEnumerable<MappedChildMatch> MapChildMatches(IEnumerable<ChildMatch> childMatches)
    {
        childMatches = FilterMatch(childMatches);
        foreach (var childMatch in childMatches)
        {
            var (subType, minorBettypeId) = GetOWPMinorMatchIdFromSubtype(childMatch.sub_type);
            if (minorBettypeId.HasValue && minorBettypeId < 0)
            {
                yield return new MappedChildMatch
                {
                    ParentInfoSiteMatchId = (int)childMatch.match_id,
                    AdminSiteMatchId = (int?)childMatch.create_match_id,
                    LeagueName = childMatch.league_name,
                    HomeName = childMatch.home_name,
                    AwayName = childMatch.away_name,
                    EventDateTime = ((DateTime)childMatch.game_date).ToUtc8(),
                    MinorBettypeId = minorBettypeId,
                    SubType = subType,
                    UpdatedTime = childMatch.stamp
                };
            }
        }
    }

    private static IEnumerable<ChildMatch> FilterMatch(IEnumerable<ChildMatch> childMatches)
        => childMatches.Where(x => !IsETPKMatch(x.home_name) && IsValidSubType(x.sub_type) && x.main_type.HasValue);

    private static bool IsETPKMatch(string homeName)
        => homeName.IsContainsIgnoreCase("(ET)") || homeName.IsContainsIgnoreCase("(PEN)");

    private static bool IsValidSubType(int? subType) => subType.HasValue && subType > 0;

    public async Task SaveInfoSiteChildMatchCommand(IEnumerable<MappedChildMatch> childMatches)
    {
        var criteria = new PushInfoSiteChildMatchCommand
        {
            ChildMatchesJson = JsonConvert.SerializeObject(childMatches)
        };

        await repository.ExecuteAsync(criteria);
    }
}