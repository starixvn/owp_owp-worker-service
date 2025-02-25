using OWP.Adminsite.Constants;
using OWP.Adminsite.Enums;
using OWP.Adminsite.Models;

namespace OWP.Adminsite.Builder;

public static class SpreadBuilder
{
    private const decimal SPREAD_BASE_COMPARER = (decimal)0.1;
    private const int LEAGUE_GROUP_MJ = 1;

    public static void BuildSpread(MatchInfo matchInfo, MatchInfoApiModel matchInfoApi, IEnumerable<MatchSetting> matchSettings)
    {
        var spreads = matchSettings.Where(x => x.OWPMatchId == matchInfo.OWPMatchId && x.Spread.HasValue).ToList();
        if (matchInfo.LeagueGroupId == LEAGUE_GROUP_MJ)
        {
            BuildSpreadForLeagueGroupMJ(matchInfoApi, spreads);
        }
        else
        {
            BuildSpreadForLeagueGroupNotMJ(matchInfoApi, spreads);
        }
    }

    public static void BuildSpreadSpecialProduct(MatchInfo matchInfo, MatchInfoApiModel matchInfoApi, IEnumerable<MatchSetting> matchSettings)
    {
        if (Array.Exists(SpecialBettype.BettypeIdSpecificXMins, id => id == matchInfo.MinorBettypeId))
        {
            var spreadHandicap = matchSettings.FirstOrDefault(x => x.OWPMatchId == matchInfo.OWPMatchId && x.Spread.HasValue);
            if (spreadHandicap != null)
            {
                var spreads = new List<MatchSetting>
                {
                    new() { BettypeId = (int)MajorBetType.Handicap, Spread = spreadHandicap.Spread },
                    new() { BettypeId = (int)MajorBetType.OverUnder, Spread = spreadHandicap.Spread }
                };
                BuilSpread(matchInfoApi, spreads);
            }
        }
        else
        {
            BuildSpread(matchInfo, matchInfoApi, matchSettings);
        }
    }

    private static void BuildSpreadForLeagueGroupMJ(MatchInfoApiModel matchInfoApi, List<MatchSetting> spreads)
    {
        var firstHalfHdp = spreads.Find(s => s.BettypeId == (int)MajorBetType.Handicap1St);
        var firstHalfOu = spreads.Find(s => s.BettypeId == (int)MajorBetType.OverUnder1St);

        var handicap = spreads.Find(x => x.BettypeId == (int)MajorBetType.Handicap) ?? new MatchSetting();
        var overUnder = spreads.Find(x => x.BettypeId == (int)MajorBetType.OverUnder) ?? new MatchSetting();

        if (firstHalfHdp == null)
        {
            spreads.Add(new MatchSetting { BettypeId = (int)MajorBetType.Handicap1St, Spread = handicap.Spread });
        }

        if (firstHalfOu == null)
        {
            spreads.Add(new MatchSetting { BettypeId = (int)MajorBetType.OverUnder1St, Spread = overUnder.Spread });
        }

        BuilSpread(matchInfoApi, spreads);
    }

    private static void BuildSpreadForLeagueGroupNotMJ(MatchInfoApiModel matchInfoApi, List<MatchSetting> spreads)
    {
        var firstHalfHdp = spreads.Find(s => s.BettypeId == (int)MajorBetType.Handicap1St);
        var firstHalfOu = spreads.Find(s => s.BettypeId == (int)MajorBetType.OverUnder1St);

        var handicap = spreads.Find(x => x.BettypeId == (int)MajorBetType.Handicap) ?? new MatchSetting();
        var overUnder = spreads.Find(x => x.BettypeId == (int)MajorBetType.OverUnder) ?? new MatchSetting();

        if (firstHalfHdp == null)
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

            spreads.Add(new MatchSetting { BettypeId = (int)MajorBetType.Handicap1St, Spread = spread1H });
        }

        if (firstHalfOu == null)
        {
            spreads.Add(new MatchSetting
            {
                BettypeId = (int)MajorBetType.OverUnder1St,
                Spread = overUnder.Spread < SPREAD_BASE_COMPARER ? SPREAD_BASE_COMPARER : overUnder.Spread
            });
        }

        BuilSpread(matchInfoApi, spreads);
    }

    private static void BuilSpread(MatchInfoApiModel matchInfoApi, IList<MatchSetting> spreads)
    {
        foreach (var spread in spreads)
        {
            var setting = matchInfoApi.MatchSettings.FirstOrDefault(x => x.Bettype == spread.BettypeId);
            if (setting != null)
            {
                setting.Spread = spread.Spread;
            }
            else
            {
                matchInfoApi.MatchSettings.Add(new MatchSettingApiModel
                {
                    Bettype = spread.BettypeId,
                    Spread = spread.Spread
                });
            }
        }
    }
}