using Olympus.OWP.Constants;
using OWP.Adminsite.Builder;
using OWP.Adminsite.Constants;
using OWP.Adminsite.Models;
using OWP.Adminsite.Repository;

namespace OWP.Adminsite.Services;

public interface ISyncMatchInfoToAdminSiteService
{
    Task Sync();
}

public class SyncMatchInfoToAdminSiteService(IMatchInfoRepository repository) : ISyncMatchInfoToAdminSiteService
{
    public async Task Sync()
    {
        var nowGmt8 = DateTime.Now.AddHours(12);
        var fromDate = nowGmt8.Date.AddHours(12);
        var toDate = fromDate.AddDays(1).AddSeconds(-1);

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

            var matchInfoApis = new List<MatchInfoApiModel>();

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

                    TraderAssignmentBuilder.BuildTraderAssignmentSpecial(matchInfo, item, assignments);
                    SpreadBuilder.BuildSpreadSpecial(matchInfo, item, matchSettings);
                }
                else
                {
                    matchInfo.MinorBettypeId = SpecialBettype.BETTYPE_ID_MAIN_MATCH;
                    item.MatchSettings = matchSettings.Where(x => x.OWPMatchId == matchInfo.OWPMatchId)
                        .Select(x => new MatchSettingApiModel
                        {
                            BettypeId = x.BettypeId,
                            Maxbet = x.Maxbet,
                            Spread = x.Spread
                        }).ToList();

                    TraderAssignmentBuilder.BuildTraderAssignment(matchInfo, item, assignments);
                    MaxbetBuilder.BuildMaxbet(item, defaultMaxbetSettings);
                    SpreadBuilder.BuildSpread(matchInfo, item);
                    SecondAcceptValueBuilder.BuildSecondAcceptValue(matchInfo, item, matchOffers, secondAcceptValues);
                    OddBuilder.BuildOdd(matchInfo, item, recommendOdds, recommendOddsDetails);
                    NumberOfLinesBuilder.BuildNumberOfLines(matchInfo, item);
                }

                matchInfoApis.Add(item);
            }
        }
    }

    private static int? GetRepresentativeSubType(int? minorBettypeId)
        => !minorBettypeId.HasValue ? null : MinorBettype.GetRepresentativeSubtype(minorBettypeId.Value);
}
