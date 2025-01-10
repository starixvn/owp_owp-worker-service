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
        IEnumerable<MatchRecommendOddDetail> recommendOddDetails)
    {
        if (!matchInfoApi.MatchSettings.Any(x => x.Bettype == (int)MajorBetType.Handicap))
        {
            matchInfoApi.MatchSettings.Add(new MatchSettingApiModel
            {
                Bettype = (int)MajorBetType.Handicap
            });
        }

        if (!matchInfoApi.MatchSettings.Any(x => x.Bettype == (int)MajorBetType.OverUnder))
        {
            matchInfoApi.MatchSettings.Add(new MatchSettingApiModel
            {
                Bettype = (int)MajorBetType.OverUnder
            });
        }

        GetOdds12(matchInfo, matchInfoApi, recommendOddDetails);
        GetHdp12(matchInfo, matchInfoApi, recommendOdds);
    }

    private static void GetOdds12(
        MatchInfo matchInfo,
        MatchInfoApiModel matchInfoApi,
        IEnumerable<MatchRecommendOddDetail> recommendOddDetails)
    {
        var handicap = matchInfoApi.MatchSettings.First(x => x.Bettype == (int)MajorBetType.Handicap);
        var overUnder = matchInfoApi.MatchSettings.First(x => x.Bettype == (int)MajorBetType.OverUnder);
        var details = recommendOddDetails.Where(x => x.OWPMatchId == matchInfo.OWPMatchId).ToList();

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
                    // nothing
                    break;
            }
        }
    }

    private static void GetHdp12(
        MatchInfo matchInfo,
        MatchInfoApiModel matchInfoApi,
        IEnumerable<MatchRecommendOdd> recommendOdds)
    {
        var handicap = matchInfoApi.MatchSettings.First(x => x.Bettype == (int)MajorBetType.Handicap);
        var overUnder = matchInfoApi.MatchSettings.First(x => x.Bettype == (int)MajorBetType.OverUnder);
        var odds = recommendOdds.Where(x => x.OWPMatchId == matchInfo.OWPMatchId).ToList();

        var hdpOdd = odds.Find(x => x.BettypeId == (int)MajorBetType.Handicap)?.HDP;
        var ouOdd = odds.Find(x => x.BettypeId == (int)MajorBetType.OverUnder)?.HDP;

        var (odd_hdp_hdp1, odd_hdp_hdp2) = GetHdp12ByTeamChoice(odds, hdpOdd);

        handicap.Hdp1 = odd_hdp_hdp1;
        handicap.Hdp2 = odd_hdp_hdp2;
        overUnder.Hdp1 = ouOdd;
        overUnder.Hdp2 = ouOdd.HasValue ? 0 : ouOdd;
    }

    private static (decimal?, decimal?) GetHdp12ByTeamChoice(List<MatchRecommendOdd> odds, decimal? hdpOdd)
    {
        decimal? odd_hdp_hdp1 = null;
        decimal? odd_hdp_hdp2 = null;
        if (odds.Exists(x => x.TeamChoice == AwayChoice))
        {
            if (hdpOdd < 0)
            {
                odd_hdp_hdp1 = 0;
                odd_hdp_hdp2 = Math.Abs(hdpOdd.Value);
            }

            if (hdpOdd == 0)
            {
                odd_hdp_hdp1 = 0;
                odd_hdp_hdp2 = 0;
            }

            if (hdpOdd > 0)
            {
                odd_hdp_hdp1 = hdpOdd;
                odd_hdp_hdp2 = 0;
            }
        }
        else
        {
            if (hdpOdd < 0)
            {
                odd_hdp_hdp1 = Math.Abs(hdpOdd.Value);
                odd_hdp_hdp2 = 0;
            }

            if (hdpOdd == 0)
            {
                odd_hdp_hdp1 = 0;
                odd_hdp_hdp2 = 0;
            }

            if (hdpOdd > 0)
            {
                odd_hdp_hdp1 = 0;
                odd_hdp_hdp2 = hdpOdd;
            }
        }

        return (odd_hdp_hdp1, odd_hdp_hdp2);
    }
}