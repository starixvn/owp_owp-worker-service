using System.Text.RegularExpressions;
using OWP.Adminsite.Constants;
using OWP.Adminsite.Enums;
using OWP.Adminsite.Models;

namespace OWP.Adminsite.Builder;

public static class SecondAcceptValueBuilder
{
    public static void BuildSecondAcceptValue(
        MatchInfo matchInfo,
        MatchInfoApiModel matchInfoApi,
        IEnumerable<MatchOffer> matchOffers,
        IEnumerable<DefaultSecondAcceptValue> secondAcceptValues)
    {
        var validOfferIds = matchOffers.Where(x => x.OWPMatchId == matchInfo.OWPMatchId && IsValidSecondAcceptValue(x.Value)).ToList();
        var maxbetHdp = matchInfoApi.MatchSettings.FirstOrDefault(x => x.Bettype == (int)MajorBetType.Handicap)?.Maxbet;
        matchInfoApi.SecondAcceptValue = validOfferIds.Count switch
        {
            0 => GetSecondAcceptValueDefault(matchInfo, maxbetHdp, secondAcceptValues),
            1 => GetSecondAcceptValue(validOfferIds.FirstOrDefault()?.Value),
            _ => null
        };
    }

    public static void BuildSecondAcceptValueSpecialProduct(
        MatchInfo matchInfo,
        MatchInfoApiModel matchInfoApi,
        IEnumerable<MatchSetting> matchSettings,
        IEnumerable<MatchOffer> matchOffers,
        IEnumerable<DefaultSecondAcceptValue> secondAcceptValues)
    {
        var maxbetMainMatch = matchSettings.FirstOrDefault(m => m.OWPMatchId == matchInfo.ParentMatchId && m.BettypeId == (int)MajorBetType.Handicap);
        if (!Array.Exists(SpecialBettype.BettypeIdSpecificXMins, id => id == matchInfo.MinorBettypeId))
        {
            var validOfferIds = matchOffers.Where(x => x.OWPMatchId == matchInfo.OWPMatchId && IsValidSecondAcceptValue(x.Value)).ToList();
            matchInfoApi.SecondAcceptValue = validOfferIds.Count switch
            {
                0 => GetSecondAcceptValueDefault(matchInfo, maxbetMainMatch.Maxbet, secondAcceptValues),
                1 => GetSecondAcceptValue(validOfferIds.FirstOrDefault()?.Value),
                _ => null
            };
        }
    }

    private static bool IsValidSecondAcceptValue(string matchOffer)
    {
        const string pattern = @"(?i)\d+(s)\s(Accept)";
        var regex = new Regex(pattern, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5));
        return regex.IsMatch(matchOffer);
    }

    private static int? GetSecondAcceptValue(string matchOffer)
    {
        if (matchOffer == null || !IsValidSecondAcceptValue(matchOffer))
        {
            return null;
        }

        var regex = new Regex(@"\d+", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5));
        var resultString = regex.Match(matchOffer).Value;
        return int.Parse(resultString);
    }

    private static int? GetSecondAcceptValueDefault(MatchInfo matchInfo, decimal? maxbetHdp, IEnumerable<DefaultSecondAcceptValue> secondAcceptValues)
    {
        const short DefaultMaxbetHDP = -1000;
        var secondAcceptValue = secondAcceptValues.FirstOrDefault(x => x.FT_HDP == maxbetHdp);
        secondAcceptValue ??= secondAcceptValues.FirstOrDefault(x => x.FT_HDP == DefaultMaxbetHDP);

        return matchInfo.League.Contains("FANTASY", StringComparison.OrdinalIgnoreCase)
            ? GetSecondAcceptValue(secondAcceptValue.Fantasy)
            : BuildSecondAcceptValueSpecialDefault(matchInfo.MinorBettypeId, secondAcceptValue);
    }

    private static int? BuildSecondAcceptValueSpecialDefault(int? bettype, DefaultSecondAcceptValue secondAcceptValue)
    {
        return bettype switch
        {
            SpecialBettype.BETTYPE_ID_NEXT_CORNERS or SpecialBettype.BETTYPE_ID_NO_OF_CORNERS => GetSecondAcceptValue(secondAcceptValue.Corner),
            SpecialBettype.BETTYPE_ID_TOTAL_BOOKINGS => GetSecondAcceptValue(secondAcceptValue.TotalBooking),
            SpecialBettype.BETTYPE_ID_MAIN_MATCH => GetSecondAcceptValue(secondAcceptValue.Main),
            _ => null
        };
    }
}