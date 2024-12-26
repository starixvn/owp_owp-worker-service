using OWP.Adminsite.Constants;
using OWP.Adminsite.Enums;
using OWP.Adminsite.Models;

namespace OWP.Adminsite.Builder;

public static class MaxbetBuilder
{
    public static void BuildMaxbet(MatchInfoApiModel matchInfoApi, IEnumerable<MaxbetSetting> defaultMaxbetSettings)
    {
        var maxbets = matchInfoApi.MatchSettings.Where(x => x.Maxbet.HasValue).ToList();

        const byte NUMBER_FULL_BETTYPE = 6;
        if (maxbets.Count == 0 || maxbets.Count == NUMBER_FULL_BETTYPE)
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

    public static void BuildMaxbetSpecial(
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

        }
        else
        {
            
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

    private static decimal? GetMaxbet(decimal? valueBettype, decimal? defaultValue)
        => valueBettype.HasValue && valueBettype.Value != MaxBetSetting.FOLLOW_MAIN_MATCH ? valueBettype : defaultValue;
}