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
            var (recommendOdds, recommendOddsDetails) = await repository.GetMatchOddAsync(matchIds);
            var matchOffers = await repository.GetMatchOfferAsync(matchIds);
            var secondAcceptValues = await repository.GetDefaultSecondAcceptValueAsync();
            var defaultMaxbetSettings = await repository.GetDefaultMaxbetSettingAsync();

            foreach (var matchInfo in matchInfos)
            {
                var item = new MatchInfoApiModel
                {
                    MatchId = matchInfo.AdminSiteMatchId,
                    Provider = matchInfo.Provider,
                    OtherProvider = matchInfo.OtherProvider,
                    WatchBet = matchInfo.WatchBet
                };

                if (matchInfo.ParentMatchId.HasValue)
                {
                    item.SubType = GetRepresentativeSubType(matchInfo.MinorBettypeId);

                    if (item.SubType.HasValue && item.SubType.Value > 0)
                    {
                        TraderAssignmentBuilder.BuildTraderAssignmentSpecialProduct(matchInfo, item, assignments);
                        MaxbetBuilder.BuildMaxbetSpecialProduct(matchInfo, item, matchSettings, defaultMaxbetSettings);
                        SpreadBuilder.BuildSpreadSpecialProduct(matchInfo, item, matchSettings);
                        SecondAcceptValueBuilder.BuildSecondAcceptValueSpecialProduct(matchInfo, item, matchSettings, matchOffers, secondAcceptValues);
                        NumberOfLinesBuilder.BuildNumberOfLinesSpecialProduct(matchInfo, item);

                        matchInfoApis.Add(item);
                    }
                }
                else
                {
                    matchInfo.MinorBettypeId = SpecialBettype.BETTYPE_ID_MAIN_MATCH;

                    TraderAssignmentBuilder.BuildTraderAssignment(matchInfo, item, assignments);
                    MaxbetBuilder.BuildMaxbet(matchInfo, item, matchSettings, defaultMaxbetSettings);
                    SpreadBuilder.BuildSpread(matchInfo, item, matchSettings);
                    SecondAcceptValueBuilder.BuildSecondAcceptValue(matchInfo, item, matchOffers, secondAcceptValues);
                    OddBuilder.BuildOdd(matchInfo, item, recommendOdds, recommendOddsDetails);
                    NumberOfLinesBuilder.BuildNumberOfLines(matchInfo, item);

                    matchInfoApis.Add(item);
                }
            }
        }

        return matchInfoApis;
    }

    private static int? GetRepresentativeSubType(int? minorBettypeId)
        => !minorBettypeId.HasValue ? null : MinorBettype.GetRepresentativeSubtype(minorBettypeId.Value);
}
