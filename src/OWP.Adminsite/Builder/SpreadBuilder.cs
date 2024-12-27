using OWP.Adminsite.Constants;
using OWP.Adminsite.Enums;
using OWP.Adminsite.Models;

namespace OWP.Adminsite.Builder;

public static class SpreadBuilder
{
    private const decimal SPREAD_BASE_COMPARER = (decimal)0.1;
    private const int LEAGUE_GROUP_MJ = 1;

    public static void BuildSpread(MatchInfo matchInfo, MatchInfoApiModel matchInfoApi)
    {
        if (!matchInfoApi.MatchSettings.Any(x => x.BettypeId == (int)MajorBetType.Handicap1St))
        {
            matchInfoApi.MatchSettings.Add(new MatchSettingApiModel
            {
                BettypeId = (int)MajorBetType.Handicap1St
            });
        }

        if (!matchInfoApi.MatchSettings.Any(x => x.BettypeId == (int)MajorBetType.OverUnder1St))
        {
            matchInfoApi.MatchSettings.Add(new MatchSettingApiModel
            {
                BettypeId = (int)MajorBetType.OverUnder1St
            });
        }

        if (matchInfo.LeagueGroupId == LEAGUE_GROUP_MJ)
        {
            BuildSpreadForLeagueGroupMJ(matchInfoApi.MatchSettings);
        }
        else
        {
            BuildSpreadForLeagueGroupNotMJ(matchInfoApi.MatchSettings);
        }
    }

    public static void BuildSpreadSpecialProduct(MatchInfo matchInfo, MatchInfoApiModel matchInfoApi, IEnumerable<MatchSetting> matchSettings)
    {
        if (SpecialBettype.BettypeIdSpecificXMins.Any(id => id == matchInfo.MinorBettypeId))
        {
            var spreadHandicap = matchSettings.FirstOrDefault(x => x.OWPMatchId == matchInfo.OWPMatchId);
            if (spreadHandicap != null)
            {
                matchInfoApi.MatchSettings =
                [
                    new MatchSettingApiModel { BettypeId = (int)MajorBetType.Handicap, Spread = spreadHandicap.Spread },
                    new MatchSettingApiModel { BettypeId = (int)MajorBetType.OverUnder, Spread = spreadHandicap.Spread }
                ];
            }
        }
        else
        {
            matchInfoApi.MatchSettings = matchSettings.Where(x => x.OWPMatchId == matchInfo.OWPMatchId)
                .Select(x => new MatchSettingApiModel
                {
                    BettypeId = x.BettypeId,
                    Maxbet = x.Maxbet,
                    Spread = x.Spread
                }).ToList();
            BuildSpread(matchInfo, matchInfoApi);
        }
    }

    private static void BuildSpreadForLeagueGroupMJ(IList<MatchSettingApiModel> spreads)
    {
        var firstHalfHdp = spreads.First(s => s.BettypeId == (int)MajorBetType.Handicap1St);
        var firstHalfOu = spreads.First(s => s.BettypeId == (int)MajorBetType.OverUnder1St);

        var handicap = spreads.FirstOrDefault(x => x.BettypeId == (int)MajorBetType.Handicap && x.Spread.HasValue) ?? new MatchSettingApiModel();
        var overUnder = spreads.FirstOrDefault(x => x.BettypeId == (int)MajorBetType.OverUnder && x.Spread.HasValue) ?? new MatchSettingApiModel();

        if (!firstHalfHdp.Spread.HasValue)
        {
            firstHalfHdp.Spread = handicap.Spread;
        }

        if (!firstHalfOu.Spread.HasValue)
        {
            firstHalfOu.Spread = overUnder.Spread;
        }
    }

    private static void BuildSpreadForLeagueGroupNotMJ(IList<MatchSettingApiModel> spreads)
    {
        var firstHalfHdp = spreads.First(s => s.BettypeId == (int)MajorBetType.Handicap1St);
        var firstHalfOu = spreads.First(s => s.BettypeId == (int)MajorBetType.OverUnder1St);

        var handicap = spreads.FirstOrDefault(x => x.BettypeId == (int)MajorBetType.Handicap && x.Spread.HasValue) ?? new MatchSettingApiModel();
        var overUnder = spreads.FirstOrDefault(x => x.BettypeId == (int)MajorBetType.OverUnder && x.Spread.HasValue) ?? new MatchSettingApiModel();

        if (!firstHalfHdp.Spread.HasValue)
        {
            decimal? spread1H = null;
            decimal? pivot = (decimal)0.09;

            if (handicap.Spread > pivot)
            {
                spread1H = handicap.Spread;
            }

            if (handicap.Spread < pivot)
            {
                spread1H = (decimal)0.1;
            }

            if (handicap.Spread.Equals(pivot))
            {
                spread1H = (decimal)0.11;
            }

            firstHalfHdp.Spread = spread1H;
        }

        if (!firstHalfOu.Spread.HasValue)
        {
            firstHalfOu.Spread = overUnder.Spread < SPREAD_BASE_COMPARER ? SPREAD_BASE_COMPARER : overUnder.Spread;
        }
    }
}