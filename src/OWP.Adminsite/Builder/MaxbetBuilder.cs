using OWP.Adminsite.Constants;
using OWP.Adminsite.Enums;
using OWP.Adminsite.Models;

namespace OWP.Adminsite.Builder;

public static class MaxbetBuilder
{
    public static void BuildMaxbet(MatchInfoApiModel matchInfoApi, IEnumerable<MaxbetSetting> defaultMaxbetSettings)
    {
        var maxbets = matchInfoApi.MatchSettings.Where(x => x.Maxbet.HasValue).ToList();

        const byte NUMBER_FULL_BETTYPE = 4;
        if (maxbets.Count == 0 || maxbets.Count(x => x.BettypeId == (int)MajorBetType.Handicap
            || x.BettypeId == (int)MajorBetType.Handicap1St
            || x.BettypeId == (int)MajorBetType.OverUnder
            || x.BettypeId == (int)MajorBetType.OverUnder1St) == NUMBER_FULL_BETTYPE)
        {
            return;
        }

        if (!matchInfoApi.MatchSettings.Any(s => s.BettypeId == (int)MajorBetType.OverUnder))
        {
            matchInfoApi.MatchSettings.Add(new MatchSettingApiModel
            {
                BettypeId = (int)MajorBetType.OverUnder
            });
        }
        BuildMaxbetOverUnder(matchInfoApi.MatchSettings, defaultMaxbetSettings);

        if (!matchInfoApi.MatchSettings.Any(s => s.BettypeId == (int)MajorBetType.Handicap1St))
        {
            matchInfoApi.MatchSettings.Add(new MatchSettingApiModel
            {
                BettypeId = (int)MajorBetType.Handicap1St
            });
        }
        BuildMaxbetHandicap1st(matchInfoApi.MatchSettings, defaultMaxbetSettings);

        if (!matchInfoApi.MatchSettings.Any(s => s.BettypeId == (int)MajorBetType.OverUnder1St))
        {
            matchInfoApi.MatchSettings.Add(new MatchSettingApiModel
            {
                BettypeId = (int)MajorBetType.OverUnder1St
            });
        }
        BuildMaxbetOverUnder1st(matchInfoApi.MatchSettings, defaultMaxbetSettings);
    }

    public static void BuildMaxbetSpecialProduct(
        MatchInfo matchInfo,
        MatchInfoApiModel matchInfoApi,
        IEnumerable<MatchSetting> matchSettings,
        IEnumerable<MaxbetSetting> defaultMaxbetSettings)
    {
        if (SpecialBettype.BettypeIdSpecificXMins.Any(id => id == matchInfo.MinorBettypeId))
        {
            var maxbetHandicap = matchSettings.FirstOrDefault(x => x.OWPMatchId == matchInfo.OWPMatchId);
            if (maxbetHandicap != null)
            {
                matchInfoApi.MatchSettings =
                [
                    new MatchSettingApiModel { BettypeId = (int)MajorBetType.Handicap, Maxbet = maxbetHandicap.Maxbet },
                    new MatchSettingApiModel { BettypeId = (int)MajorBetType.OverUnder, Maxbet = maxbetHandicap.Maxbet }
                ];
            }
        }
        else if (SpecialBettype.BettypeIdSpecialProducts.Any(id => id == matchInfo.MinorBettypeId))
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
                    matchInfoApi.MatchSettings = maxbets.Select(x => new MatchSettingApiModel
                    {
                        BettypeId = x.BettypeId,
                        Maxbet = x.Maxbet
                    }).ToList();
                    break;
            }
        }
        else
        {
            BuildMaxbet(matchInfoApi, defaultMaxbetSettings);
        }
    }

    private static void BuildMaxbetOverUnder(IList<MatchSettingApiModel> maxbets, IEnumerable<MaxbetSetting> defaultMaxbetSettings)
    {
        var ou = maxbets.First(x => x.BettypeId == (int)MajorBetType.OverUnder);
        if (!ou.Maxbet.HasValue)
        {
            var handicap = maxbets.FirstOrDefault(m => m.BettypeId == (int)MajorBetType.Handicap) ?? new MatchSettingApiModel();
            var maxbetFtOu = defaultMaxbetSettings.FirstOrDefault(x => x.FT_HDP == handicap.Maxbet)?.FT_OU;

            ou.Maxbet = GetMaxbet(maxbetFtOu, handicap.Maxbet);
        }
    }

    private static void BuildMaxbetHandicap1st(IList<MatchSettingApiModel> maxbets, IEnumerable<MaxbetSetting> defaultMaxbetSettings)
    {
        var handicap1st = maxbets.First(x => x.BettypeId == (int)MajorBetType.Handicap1St);
        if (!handicap1st.Maxbet.HasValue)
        {
            var handicap = maxbets.FirstOrDefault(m => m.BettypeId == (int)MajorBetType.Handicap) ?? new MatchSettingApiModel();
            var valueFirstHalfHandicap = defaultMaxbetSettings.FirstOrDefault(x => x.FT_HDP == handicap.Maxbet)?.FirstHalf_HDP;
            handicap1st.Maxbet = GetMaxbet(valueFirstHalfHandicap, handicap.Maxbet);
        }
    }

    private static void BuildMaxbetOverUnder1st(IList<MatchSettingApiModel> maxbets, IEnumerable<MaxbetSetting> defaultMaxbetSettings)
    {
        var overUnder1st = maxbets.First(x => x.BettypeId == (int)MajorBetType.OverUnder1St);
        if(!overUnder1st.Maxbet.HasValue)
        {
            var handicap = maxbets.FirstOrDefault(m => m.BettypeId == (int)MajorBetType.Handicap) ?? new MatchSettingApiModel();
            var valueFirstHalfOverUnder = defaultMaxbetSettings.FirstOrDefault(x => x.FT_HDP == handicap.Maxbet)?.FirstHalf_OU;
            overUnder1st.Maxbet = GetMaxbet(valueFirstHalfOverUnder, handicap.Maxbet);
        }
    }

    private static void BuildMaxbetNoOfCorner(
        MatchInfoApiModel matchInfoApi,
        IList<MatchSetting> maxbets,
        IList<MatchSetting> maxbetsMainMatch,
        IEnumerable<MaxbetSetting> defaultMaxbetSettings)
    {
        var handicapMainMatch = GetMaxbetHandicapMainMatch(maxbetsMainMatch);
        var valueBettype = defaultMaxbetSettings.FirstOrDefault(x => x.FT_HDP == handicapMainMatch.Maxbet);
        if (!maxbets.Any(s => s.BettypeId == (int)MajorBetType.Handicap))
        {
            maxbets.Add(new MatchSetting { BettypeId = (int)MajorBetType.Handicap, Maxbet = GetMaxbet(valueBettype?.NoOfCorner_FT_HDP, handicapMainMatch.Maxbet) });
        }

        if (!maxbets.Any(s => s.BettypeId == (int)MajorBetType.OverUnder))
        {
            maxbets.Add(new MatchSetting { BettypeId = (int)MajorBetType.OverUnder, Maxbet = GetMaxbet(valueBettype?.NoOfCorner_FT_OU, handicapMainMatch.Maxbet) });
        }

        if (!maxbets.Any(s => s.BettypeId == (int)MajorBetType.Handicap1St))
        {
            maxbets.Add(new MatchSetting { BettypeId = (int)MajorBetType.Handicap1St, Maxbet = GetMaxbet(valueBettype?.NoOfCorner_1H_HDP, handicapMainMatch.Maxbet) });
        }

        if (!maxbets.Any(s => s.BettypeId == (int)MajorBetType.OverUnder1St))
        {
            maxbets.Add(new MatchSetting { BettypeId = (int)MajorBetType.OverUnder1St, Maxbet = GetMaxbet(valueBettype?.NoOfCorner_1H_OU, handicapMainMatch.Maxbet) });
        }

        matchInfoApi.MatchSettings = maxbets.Select(x => new MatchSettingApiModel
        {
            BettypeId = x.BettypeId,
            Maxbet = x.Maxbet
        }).ToList();
    }

    private static void BuildMaxbetNextCorner(
        MatchInfoApiModel matchInfoApi,
        IList<MatchSetting> maxbets,
        IList<MatchSetting> maxbetsMainMatch,
        IEnumerable<MaxbetSetting> defaultMaxbetSettings)
    {
        if (!maxbets.Any(s => s.BettypeId == (int)MajorBetType.Handicap))
        {
            var handicapMainMatch = GetMaxbetHandicapMainMatch(maxbetsMainMatch);
            var valueBettype = defaultMaxbetSettings.FirstOrDefault(x => x.FT_HDP == handicapMainMatch.Maxbet);
            maxbets.Add(new MatchSetting { BettypeId = (int)MajorBetType.Handicap, Maxbet = GetMaxbet(valueBettype?.NextCorner_FT_HDP, handicapMainMatch.Maxbet) });
        }

        matchInfoApi.MatchSettings = maxbets.Select(x => new MatchSettingApiModel
        {
            BettypeId = x.BettypeId,
            Maxbet = x.Maxbet
        }).ToList();
    }

    private static void BuildMaxbetNextBooking(
        MatchInfoApiModel matchInfoApi,
        IList<MatchSetting> maxbets,
        IList<MatchSetting> maxbetsMainMatch,
        IEnumerable<MaxbetSetting> defaultMaxbetSettings)
    {
        if (!maxbets.Any(s => s.BettypeId == (int)MajorBetType.Handicap))
        {
            var handicapMainMatch = GetMaxbetHandicapMainMatch(maxbetsMainMatch);
            var valueBettype = defaultMaxbetSettings.FirstOrDefault(x => x.FT_HDP == handicapMainMatch.Maxbet);
            maxbets.Add(new MatchSetting { BettypeId = (int)MajorBetType.Handicap, Maxbet = GetMaxbet(valueBettype?.NextBooking_FT_HDP, handicapMainMatch.Maxbet) });
        }

        matchInfoApi.MatchSettings = maxbets.Select(x => new MatchSettingApiModel
        {
            BettypeId = x.BettypeId,
            Maxbet = x.Maxbet
        }).ToList();
    }

    private static void BuildMaxbetTotalBooking(
        MatchInfoApiModel matchInfoApi,
        IList<MatchSetting> maxbets,
        IList<MatchSetting> maxbetsMainMatch,
        IEnumerable<MaxbetSetting> defaultMaxbetSettings)
    {
        var handicapMainMatch = GetMaxbetHandicapMainMatch(maxbetsMainMatch);
        var valueBettype = defaultMaxbetSettings.FirstOrDefault(x => x.FT_HDP == handicapMainMatch.Maxbet);
        if (!maxbets.Any(s => s.BettypeId == (int)MajorBetType.Handicap))
        {
            maxbets.Add(new MatchSetting { BettypeId = (int)MajorBetType.Handicap, Maxbet = GetMaxbet(valueBettype?.TotalBooking_FT_HDP, handicapMainMatch.Maxbet) });
        }

        if (!maxbets.Any(s => s.BettypeId == (int)MajorBetType.OverUnder))
        {
            maxbets.Add(new MatchSetting { BettypeId = (int)MajorBetType.OverUnder, Maxbet = GetMaxbet(valueBettype?.TotalBooking_FT_OU, handicapMainMatch.Maxbet) });
        }

        if (!maxbets.Any(s => s.BettypeId == (int)MajorBetType.Handicap1St))
        {
            maxbets.Add(new MatchSetting { BettypeId = (int)MajorBetType.Handicap1St, Maxbet = GetMaxbet(valueBettype?.TotalBooking_1H_HDP, handicapMainMatch.Maxbet) });
        }

        if (!maxbets.Any(s => s.BettypeId == (int)MajorBetType.OverUnder1St))
        {
            maxbets.Add(new MatchSetting { BettypeId = (int)MajorBetType.OverUnder1St, Maxbet = GetMaxbet(valueBettype?.TotalBooking_1H_OU, handicapMainMatch.Maxbet) });
        }

        matchInfoApi.MatchSettings = maxbets.Select(x => new MatchSettingApiModel
        {
            BettypeId = x.BettypeId,
            Maxbet = x.Maxbet
        }).ToList();
    }

    private static MatchSetting GetMaxbetHandicapMainMatch(IEnumerable<MatchSetting> maxbetsMainMatch)
        => maxbetsMainMatch.FirstOrDefault(m => m.BettypeId == (int)MajorBetType.Handicap) ?? new MatchSetting();

    private static decimal? GetMaxbet(decimal? valueBettype, decimal? defaultValue)
        => valueBettype.HasValue && valueBettype.Value != MaxBetSetting.FOLLOW_MAIN_MATCH ? valueBettype : defaultValue;
}