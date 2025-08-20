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
        var maxbet = matchSettings.Where(x => x.OWPMatchId == matchInfo.OWPMatchId && x.Maxbet.HasValue).ToList();

        const byte NUMBER_FULL_BETTYPE = 4;
        if (maxbet.Count == 0 ||
            maxbet.Count(x => x.BettypeId is (int)MajorBetType.Handicap
                or (int)MajorBetType.Handicap1St
                or (int)MajorBetType.OverUnder
                or (int)MajorBetType.OverUnder1St) == NUMBER_FULL_BETTYPE)
        {
            BuildMaxbet(matchInfoApi, maxbet);
            return;
        }

        if (!maxbet.Exists(s => s.BettypeId == (int)MajorBetType.OverUnder))
        {
            BuildMaxbetOverUnder(maxbet, defaultMaxbetSettings);
        }

        if (!maxbet.Exists(s => s.BettypeId == (int)MajorBetType.Handicap1St))
        {
            BuildMaxbetHandicap1st(maxbet, defaultMaxbetSettings);
        }

        if (!maxbet.Exists(s => s.BettypeId == (int)MajorBetType.OverUnder1St))
        {
            BuildMaxbetOverUnder1st(maxbet, defaultMaxbetSettings);
        }

        BuildMaxbet(matchInfoApi, maxbet);
    }

    public static void BuildMaxbetSpecialProduct(
        MatchInfo matchInfo,
        MatchInfoApiModel matchInfoApi,
        IEnumerable<MatchSetting> matchSettings,
        IEnumerable<MaxbetSetting> defaultMaxbetSettings)
    {
        if (Array.Exists(SpecialBettype.BettypeIdSpecificXMins, id => id == matchInfo.MinorBettypeId))
        {
            BuildMaxbetXMins(matchInfo, matchInfoApi, matchSettings);
        }
        else if (Array.Exists(SpecialBettype.BettypeIdSpecialProducts, id => id == matchInfo.MinorBettypeId))
        {
            BuildMaxbetSpecialProducts(matchInfo, matchInfoApi, matchSettings, defaultMaxbetSettings);
        }
        else
        {
            var maxbetSetting = matchSettings.Where(x => x.OWPMatchId == matchInfo.OWPMatchId && x.Maxbet.HasValue).ToList();
            if (maxbetSetting.Count > 0)
            {
                BuildMaxbet(matchInfoApi, maxbetSetting);
            }
        }
    }

    private static void BuildMaxbetXMins(
        MatchInfo matchInfo,
        MatchInfoApiModel matchInfoApi,
        IEnumerable<MatchSetting> matchSettings)
    {
        var maxbetHandicap = matchSettings.FirstOrDefault(x => x.OWPMatchId == matchInfo.OWPMatchId && x.Maxbet.HasValue) ?? new MatchSetting();
        var maxbets = new List<MatchSetting>
        {
            new() { BettypeId = (int)MajorBetType.Handicap, Maxbet = maxbetHandicap.Maxbet },
            new() { BettypeId = (int)MajorBetType.OverUnder, Maxbet = maxbetHandicap.Maxbet }
        };
        BuildMaxbet(matchInfoApi, maxbets);
    }

    private static void BuildMaxbetSpecialProducts(
        MatchInfo matchInfo,
        MatchInfoApiModel matchInfoApi,
        IEnumerable<MatchSetting> matchSettings,
        IEnumerable<MaxbetSetting> defaultMaxbetSettings)
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

    private static void BuildMaxbet(MatchInfoApiModel matchInfoApi, List<MatchSetting> maxbetSetting)
    {
        foreach (var maxbet in maxbetSetting)
        {
            var setting = matchInfoApi.MatchSettings.FirstOrDefault(x => x.Bettype == maxbet.BettypeId);
            if (setting != null)
            {
                setting.Maxbet = GetMaxbet(maxbet.Maxbet);
            }
            else
            {
                matchInfoApi.MatchSettings.Add(new MatchSettingApiModel
                {
                    Bettype = maxbet.BettypeId,
                    Maxbet = GetMaxbet(maxbet.Maxbet)
                });
            }
        }
    }

    private static MatchSetting GetMaxbetHandicapMainMatch(IEnumerable<MatchSetting> maxbetsMainMatch)
        => maxbetsMainMatch.FirstOrDefault(x => x.BettypeId == (int)MajorBetType.Handicap && x.Maxbet.HasValue) ?? new MatchSetting();

    private static decimal? GetMaxbet(decimal? valueBettype, decimal? defaultValue)
        => valueBettype.HasValue && valueBettype.Value != MaxBetSetting.FOLLOW_MAIN_MATCH ? valueBettype : defaultValue;

    private static int? GetMaxbet(decimal? maxbet) => maxbet.HasValue ? (int)Math.Round(maxbet.Value, 0) : null;
}