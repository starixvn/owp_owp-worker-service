using OWP.Adminsite.Constants;
using OWP.Adminsite.Enums;
using OWP.Adminsite.Models;

namespace OWP.Adminsite.Builder;

public static class MaxbetBuilder
{
    public static void BuildMaxbet(
        MatchInfo matchInfo,
        MatchInfoApiModel matchInfoApi,
        IEnumerable<MatchSetting> matchSettings,
        IEnumerable<MaxbetSetting> defaultMaxbetSettings)
    {
        var maxbets = matchSettings.Where(x => x.OWPMatchId == matchInfo.OWPMatchId && x.Maxbet.HasValue).ToList();

        const byte NUMBER_FULL_BETTYPE = 4;
        if (maxbets.Count == 0 || maxbets.Count(x => x.BettypeId == (int)MajorBetType.Handicap
            || x.BettypeId == (int)MajorBetType.Handicap1St
            || x.BettypeId == (int)MajorBetType.OverUnder
            || x.BettypeId == (int)MajorBetType.OverUnder1St) == NUMBER_FULL_BETTYPE)
        {
            BuildMaxbet(matchInfoApi, maxbets);
            return;
        }

        if (!maxbets.Exists(s => s.BettypeId == (int)MajorBetType.OverUnder))
        {
            BuildMaxbetOverUnder(maxbets, defaultMaxbetSettings);
        }

        if (!maxbets.Exists(s => s.BettypeId == (int)MajorBetType.Handicap1St))
        {
            BuildMaxbetHandicap1st(maxbets, defaultMaxbetSettings);
        }

        if (!maxbets.Exists(s => s.BettypeId == (int)MajorBetType.OverUnder1St))
        {
            BuildMaxbetOverUnder1st(maxbets, defaultMaxbetSettings);
        }

        BuildMaxbet(matchInfoApi, maxbets);
    }

    public static void BuildMaxbetSpecialProduct(
        MatchInfo matchInfo,
        MatchInfoApiModel matchInfoApi,
        IEnumerable<MatchSetting> matchSettings,
        IEnumerable<MaxbetSetting> defaultMaxbetSettings)
    {
        if (Array.Exists(SpecialBettype.BettypeIdSpecificXMins, id => id == matchInfo.MinorBettypeId))
        {
            var maxbetHandicap = matchSettings.FirstOrDefault(x => x.OWPMatchId == matchInfo.OWPMatchId && x.Maxbet.HasValue) ?? new MatchSetting();
            var maxbets = new List<MatchSetting>
            {
                new() { BettypeId = (int)MajorBetType.Handicap, Maxbet = maxbetHandicap.Maxbet },
                new() { BettypeId = (int)MajorBetType.OverUnder, Maxbet = maxbetHandicap.Maxbet }
            };
            BuildMaxbet(matchInfoApi, maxbets);
        }
        else if (Array.Exists(SpecialBettype.BettypeIdSpecialProducts, id => id == matchInfo.MinorBettypeId))
        {
            var maxbets = matchSettings.Where(x => x.OWPMatchId == matchInfo.OWPMatchId && x.Maxbet.HasValue).ToList();
            var maxbetsMainMatch = matchSettings.Where(x => x.OWPMatchId == matchInfo.ParentMatchId).ToList();
            switch (matchInfo.MinorBettypeId)
            {
                case SpecialBettype.BETTYPE_ID_NO_OF_CORNERS:
                    BuildMaxbetNoOfCorner(matchInfoApi, maxbets, maxbetsMainMatch, defaultMaxbetSettings);
                    break;
                case SpecialBettype.BETTYPE_ID_NEXT_CORNERS:
                    BuildMaxbetNextCorner(matchInfoApi, maxbets, maxbetsMainMatch, defaultMaxbetSettings);
                    break;
                case SpecialBettype.BETTYPE_ID_NEXT_BOOKINGS:
                    BuildMaxbetNextBooking(matchInfoApi, maxbets, maxbetsMainMatch, defaultMaxbetSettings);
                    break;
                case SpecialBettype.BETTYPE_ID_TOTAL_BOOKINGS:
                    BuildMaxbetTotalBooking(matchInfoApi, maxbets, maxbetsMainMatch, defaultMaxbetSettings);
                    break;
                default:
                    BuildMaxbet(matchInfoApi, maxbets);
                    break;
            }
        }
        else
        {
            BuildMaxbet(matchInfo, matchInfoApi, matchSettings, defaultMaxbetSettings);
        }
    }

    private static void BuildMaxbetOverUnder(List<MatchSetting> maxbets, IEnumerable<MaxbetSetting> defaultMaxbetSettings)
    {
        var handicap = maxbets.Find(m => m.BettypeId == (int)MajorBetType.Handicap) ?? new MatchSetting();
        var maxbetFtOu = defaultMaxbetSettings.FirstOrDefault(x => x.FT_HDP == handicap.Maxbet)?.FT_OU;

        maxbets.Add(new MatchSetting { BettypeId = (int)MajorBetType.OverUnder, Maxbet = GetMaxbet(maxbetFtOu, handicap.Maxbet) });
    }

    private static void BuildMaxbetHandicap1st(List<MatchSetting> maxbets, IEnumerable<MaxbetSetting> defaultMaxbetSettings)
    {
        var handicap = maxbets.Find(m => m.BettypeId == (int)MajorBetType.Handicap) ?? new MatchSetting();
        var maxbetHDP1st = defaultMaxbetSettings.FirstOrDefault(x => x.FT_HDP == handicap.Maxbet)?.FirstHalf_HDP;

        maxbets.Add(new MatchSetting { BettypeId = (int)MajorBetType.Handicap1St, Maxbet = GetMaxbet(maxbetHDP1st, handicap.Maxbet) });
    }

    private static void BuildMaxbetOverUnder1st(List<MatchSetting> maxbets, IEnumerable<MaxbetSetting> defaultMaxbetSettings)
    {
        var handicap = maxbets.Find(m => m.BettypeId == (int)MajorBetType.Handicap) ?? new MatchSetting();
        var maxbetOU1st = defaultMaxbetSettings.FirstOrDefault(x => x.FT_HDP == handicap.Maxbet)?.FirstHalf_OU;

        maxbets.Add(new MatchSetting { BettypeId = (int)MajorBetType.OverUnder1St, Maxbet = GetMaxbet(maxbetOU1st, handicap.Maxbet) });
    }

    private static void BuildMaxbetNoOfCorner(
        MatchInfoApiModel matchInfoApi,
        List<MatchSetting> maxbets,
        IList<MatchSetting> maxbetsMainMatch,
        IEnumerable<MaxbetSetting> defaultMaxbetSettings)
    {
        var handicapMainMatch = GetMaxbetHandicapMainMatch(maxbetsMainMatch);
        var valueBettype = defaultMaxbetSettings.FirstOrDefault(x => x.FT_HDP == handicapMainMatch.Maxbet);
        if (!maxbets.Exists(s => s.BettypeId == (int)MajorBetType.Handicap))
        {
            maxbets.Add(new MatchSetting { BettypeId = (int)MajorBetType.Handicap, Maxbet = GetMaxbet(valueBettype?.NoOfCorner_FT_HDP, handicapMainMatch.Maxbet) });
        }

        if (!maxbets.Exists(s => s.BettypeId == (int)MajorBetType.OverUnder))
        {
            maxbets.Add(new MatchSetting { BettypeId = (int)MajorBetType.OverUnder, Maxbet = GetMaxbet(valueBettype?.NoOfCorner_FT_OU, handicapMainMatch.Maxbet) });
        }

        if (!maxbets.Exists(s => s.BettypeId == (int)MajorBetType.Handicap1St))
        {
            maxbets.Add(new MatchSetting { BettypeId = (int)MajorBetType.Handicap1St, Maxbet = GetMaxbet(valueBettype?.NoOfCorner_1H_HDP, handicapMainMatch.Maxbet) });
        }

        if (!maxbets.Exists(s => s.BettypeId == (int)MajorBetType.OverUnder1St))
        {
            maxbets.Add(new MatchSetting { BettypeId = (int)MajorBetType.OverUnder1St, Maxbet = GetMaxbet(valueBettype?.NoOfCorner_1H_OU, handicapMainMatch.Maxbet) });
        }

        BuildMaxbet(matchInfoApi, maxbets);
    }

    private static void BuildMaxbetNextCorner(
        MatchInfoApiModel matchInfoApi,
        List<MatchSetting> maxbets,
        IList<MatchSetting> maxbetsMainMatch,
        IEnumerable<MaxbetSetting> defaultMaxbetSettings)
    {
        if (!maxbets.Exists(s => s.BettypeId == (int)MajorBetType.Handicap))
        {
            var handicapMainMatch = GetMaxbetHandicapMainMatch(maxbetsMainMatch);
            var valueBettype = defaultMaxbetSettings.FirstOrDefault(x => x.FT_HDP == handicapMainMatch.Maxbet);
            maxbets.Add(new MatchSetting { BettypeId = (int)MajorBetType.Handicap, Maxbet = GetMaxbet(valueBettype?.NextCorner_FT_HDP, handicapMainMatch.Maxbet) });
        }

        BuildMaxbet(matchInfoApi, maxbets);
    }

    private static void BuildMaxbetNextBooking(
        MatchInfoApiModel matchInfoApi,
        List<MatchSetting> maxbets,
        IList<MatchSetting> maxbetsMainMatch,
        IEnumerable<MaxbetSetting> defaultMaxbetSettings)
    {
        if (!maxbets.Exists(s => s.BettypeId == (int)MajorBetType.Handicap))
        {
            var handicapMainMatch = GetMaxbetHandicapMainMatch(maxbetsMainMatch);
            var valueBettype = defaultMaxbetSettings.FirstOrDefault(x => x.FT_HDP == handicapMainMatch.Maxbet);
            maxbets.Add(new MatchSetting { BettypeId = (int)MajorBetType.Handicap, Maxbet = GetMaxbet(valueBettype?.NextBooking_FT_HDP, handicapMainMatch.Maxbet) });
        }

        BuildMaxbet(matchInfoApi, maxbets);
    }

    private static void BuildMaxbetTotalBooking(
        MatchInfoApiModel matchInfoApi,
        List<MatchSetting> maxbets,
        IList<MatchSetting> maxbetsMainMatch,
        IEnumerable<MaxbetSetting> defaultMaxbetSettings)
    {
        var handicapMainMatch = GetMaxbetHandicapMainMatch(maxbetsMainMatch);
        var valueBettype = defaultMaxbetSettings.FirstOrDefault(x => x.FT_HDP == handicapMainMatch.Maxbet);
        if (!maxbets.Exists(s => s.BettypeId == (int)MajorBetType.Handicap))
        {
            maxbets.Add(new MatchSetting { BettypeId = (int)MajorBetType.Handicap, Maxbet = GetMaxbet(valueBettype?.TotalBooking_FT_HDP, handicapMainMatch.Maxbet) });
        }

        if (!maxbets.Exists(s => s.BettypeId == (int)MajorBetType.OverUnder))
        {
            maxbets.Add(new MatchSetting { BettypeId = (int)MajorBetType.OverUnder, Maxbet = GetMaxbet(valueBettype?.TotalBooking_FT_OU, handicapMainMatch.Maxbet) });
        }

        if (!maxbets.Exists(s => s.BettypeId == (int)MajorBetType.Handicap1St))
        {
            maxbets.Add(new MatchSetting { BettypeId = (int)MajorBetType.Handicap1St, Maxbet = GetMaxbet(valueBettype?.TotalBooking_1H_HDP, handicapMainMatch.Maxbet) });
        }

        if (!maxbets.Exists(s => s.BettypeId == (int)MajorBetType.OverUnder1St))
        {
            maxbets.Add(new MatchSetting { BettypeId = (int)MajorBetType.OverUnder1St, Maxbet = GetMaxbet(valueBettype?.TotalBooking_1H_OU, handicapMainMatch.Maxbet) });
        }

        BuildMaxbet(matchInfoApi, maxbets);
    }

    private static void BuildMaxbet(MatchInfoApiModel matchInfoApi, List<MatchSetting> maxbets)
    {
        foreach (var maxbet in maxbets)
        {
            var setting = matchInfoApi.MatchSettings.FirstOrDefault(x => x.BettypeId == maxbet.BettypeId);
            if (setting != null)
            {
                setting.Maxbet = maxbet.Maxbet;
            }
            else
            {
                matchInfoApi.MatchSettings.Add(new MatchSettingApiModel
                {
                    BettypeId = maxbet.BettypeId,
                    Maxbet = maxbet.Maxbet
                });
            }
        }
    }

    private static MatchSetting GetMaxbetHandicapMainMatch(IEnumerable<MatchSetting> maxbetsMainMatch)
        => maxbetsMainMatch.FirstOrDefault(x => x.BettypeId == (int)MajorBetType.Handicap && x.Maxbet.HasValue) ?? new MatchSetting();

    private static decimal? GetMaxbet(decimal? valueBettype, decimal? defaultValue)
        => valueBettype.HasValue && valueBettype.Value != MaxBetSetting.FOLLOW_MAIN_MATCH ? valueBettype : defaultValue;
}