using Olympus.OWP.Constants;
using OWP.Adminsite.Builder;
using OWP.Adminsite.Constants;
using OWP.Adminsite.Models;
using OWP.Adminsite.Repository;

namespace OWP.Adminsite.Services;

public interface IMatchInfoService
{
    Task<IList<MatchInfoApiModel>> GetMatchInfoAsync(DateTime fromDate, DateTime toDate);
}

public class MatchInfoService(IMatchInfoRepository repository) : IMatchInfoService
{
    public async Task<IList<MatchInfoApiModel>> GetMatchInfoAsync(DateTime fromDate, DateTime toDate)
    {
        var matchInfoApis = new List<MatchInfoApiModel>();
        var matchInfos = await repository.GetMatchInfoAsync(fromDate, toDate);

        if (matchInfos.Any())
        {
            var matchIds = string.Join(",", matchInfos.Select(x => x.OWPMatchId));
            var assignments = await repository.GetAssignmentAsync(matchIds);
            var matchSettings = await repository.GetMatchSettingAsync(matchIds);
            var (recommendOdds, recommendOddDetails) = await repository.GetMatchOddAsync(matchIds);
            var matchOffers = await repository.GetMatchOfferAsync(matchIds);
            var secondAcceptValues = await repository.GetDefaultSecondAcceptValueAsync();
            var defaultMaxbetSettings = await repository.GetDefaultMaxbetSettingAsync();

            foreach (var matchInfo in matchInfos)
            {
                if (matchInfo.ParentMatchId.HasValue)
                {
                    BuildSpecialMatch(matchInfo, matchInfoApis, assignments, matchSettings, matchOffers, secondAcceptValues, defaultMaxbetSettings);
                }
                else
                {
                    matchInfo.MinorBettypeId = SpecialBettype.BETTYPE_ID_MAIN_MATCH;

                    var item = new MatchInfoApiModel
                    {
                        MatchId = matchInfo.AdminSiteMatchId,
                        Provider = matchInfo.Provider,
                        OtherProvider = matchInfo.OtherProvider,
                        WatchBet = matchInfo.WatchBet
                    };

                    TraderAssignmentBuilder.BuildTraderAssignment(matchInfo, item, assignments);
                    MaxbetBuilder.BuildMaxbet(matchInfo, item, matchSettings, defaultMaxbetSettings);
                    SpreadBuilder.BuildSpread(matchInfo, item, matchSettings);
                    SecondAcceptValueBuilder.BuildSecondAcceptValue(matchInfo, item, matchOffers, secondAcceptValues);
                    OddBuilder.BuildOdd(matchInfo, item, recommendOdds, recommendOddDetails);
                    NumberOfLinesBuilder.BuildNumberOfLines(matchInfo, item);

                    matchInfoApis.Add(item);
                }
            }
        }

        return matchInfoApis;
    }

    private static void BuildSpecialMatch(
        MatchInfo matchInfo,
        List<MatchInfoApiModel> matchInfoApis,
        IEnumerable<Assignment> assignments,
        IEnumerable<MatchSetting> matchSettings,
        IEnumerable<MatchOffer> matchOffers,
        IEnumerable<DefaultSecondAcceptValue> defaultSecondAcceptValues,
        IEnumerable<MaxbetSetting> defaultMaxbetSettings)
    {
        var subType = GetRepresentativeSubType(matchInfo.MinorBettypeId);

        if (subType.HasValue && subType.Value > 0)
        {
            var item = new MatchInfoApiModel
            {
                MatchId = matchInfo.AdminSiteMatchId,
                Provider = matchInfo.Provider,
                OtherProvider = matchInfo.OtherProvider,
                WatchBet = matchInfo.WatchBet,
                SubType = subType
            };

            TraderAssignmentBuilder.BuildTraderAssignmentSpecialProduct(matchInfo, item, assignments);
            MaxbetBuilder.BuildMaxbetSpecialProduct(matchInfo, item, matchSettings, defaultMaxbetSettings);
            SpreadBuilder.BuildSpreadSpecialProduct(matchInfo, item, matchSettings);
            SecondAcceptValueBuilder.BuildSecondAcceptValueSpecialProduct(matchInfo, item, matchSettings, matchOffers, defaultSecondAcceptValues);
            NumberOfLinesBuilder.BuildNumberOfLinesSpecialProduct(matchInfo, item);

            matchInfoApis.Add(item);
        }
    }

    private static int? GetRepresentativeSubType(int? minorBettypeId)
        => !minorBettypeId.HasValue ? null : MinorBettype.GetRepresentativeSubtype(minorBettypeId.Value);
}
