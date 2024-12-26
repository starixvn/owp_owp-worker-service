using OWP.Adminsite.Enums;
using OWP.Adminsite.Models;

namespace OWP.Adminsite.Builder;

public static class OddBuilder
{
    private const byte HomeChoice = 1;
    private const byte AwayChoice = 2;

    public static void BuildOdd(
        MatchInfo matchInfo,
        MatchInfoApiModel matchInfoApi,
        IEnumerable<MatchRecommendOdd> recommendOdds,
        IEnumerable<MatchRecommendOddDetail> recommendOddsDetails)
    {
        if (!matchInfoApi.MatchSettings.Any(x => x.BettypeId == (int)MajorBetType.Handicap))
        {
            matchInfoApi.MatchSettings.Add(new MatchSettingApiModel
            {
                BettypeId = (int)MajorBetType.Handicap
            });
        }

        if (!matchInfoApi.MatchSettings.Any(x => x.BettypeId == (int)MajorBetType.OverUnder))
        {
            matchInfoApi.MatchSettings.Add(new MatchSettingApiModel
            {
                BettypeId = (int)MajorBetType.OverUnder
            });
        }

        GetOdds12(matchInfo, matchInfoApi, recommendOddsDetails);
        GetHdp12(matchInfo, matchInfoApi, recommendOdds);
    }

    private static void GetOdds12(
        MatchInfo matchInfo,
        MatchInfoApiModel matchInfoApi,
        IEnumerable<MatchRecommendOddDetail> recommendOddsDetails)
    {
        var handicap = matchInfoApi.MatchSettings.First(x => x.BettypeId == (int)MajorBetType.Handicap);
        var overUnder = matchInfoApi.MatchSettings.First(x => x.BettypeId == (int)MajorBetType.OverUnder);
        var details = recommendOddsDetails.Where(x => x.OWPMatchId == matchInfo.OWPMatchId).ToList();

        foreach (var detail in details)
        {
            switch (detail.Choice)
            {
                case HomeChoice:
                    if (detail.BettypeId == (int)MajorBetType.Handicap)
                    {
                        handicap.Odds1 = detail.Odds;
                    }

                    if (detail.BettypeId == (int)MajorBetType.OverUnder)
                    {
                        overUnder.Odds1 = detail.Odds;
                    }
                    break;

                case AwayChoice:
                    if (detail.BettypeId == (int)MajorBetType.Handicap)
                    {
                        handicap.Odds2 = detail.Odds;
                    }

                    if (detail.BettypeId == (int)MajorBetType.OverUnder)
                    {
                        overUnder.Odds2 = detail.Odds;
                    }
                    break;

                default:
                    break;
            }
        }
    }

    private static void GetHdp12(
        MatchInfo matchInfo,
        MatchInfoApiModel matchInfoApi,
        IEnumerable<MatchRecommendOdd> recommendOdds)
    {
        var handicap = matchInfoApi.MatchSettings.First(x => x.BettypeId == (int)MajorBetType.Handicap);
        var overUnder = matchInfoApi.MatchSettings.First(x => x.BettypeId == (int)MajorBetType.OverUnder);
        var odds = recommendOdds.Where(x => x.OWPMatchId == matchInfo.OWPMatchId).ToList();

        var hdpOdd = odds.FirstOrDefault(x => x.BettypeId == (int)MajorBetType.Handicap)?.HDP;
        var ouOdd = odds.FirstOrDefault(x => x.BettypeId == (int)MajorBetType.OverUnder)?.HDP;

        decimal? odd_hdp_hdp1 = 0;
        decimal? odd_hdp_hdp2 = 0;
        if (odds.Any(x => x.TeamChoice == AwayChoice))
        {
            if (hdpOdd.HasValue && hdpOdd.Value < 0)
            {
                odd_hdp_hdp2 = Math.Abs(hdpOdd.Value);
            }

            if (hdpOdd.HasValue && hdpOdd.Value > 0)
            {
                odd_hdp_hdp1 = hdpOdd.Value;
            }
        }
        else
        {
            if (hdpOdd.HasValue && hdpOdd.Value < 0)
            {
                odd_hdp_hdp1 = Math.Abs(hdpOdd.Value);
            }

            if (hdpOdd.HasValue && hdpOdd.Value > 0)
            {
                odd_hdp_hdp2 = hdpOdd.Value;
            }
        }

        handicap.Hdp1 = odd_hdp_hdp1;
        handicap.Hdp2 = odd_hdp_hdp2;
        overUnder.Hdp1 = ouOdd;
        overUnder.Hdp2 = ouOdd.HasValue ? 0 : ouOdd;
    }
}