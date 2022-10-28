using OWP.Core.Extensions;
using OWP.Infosite.Constants;
using OWP.Infosite.Models;
using System.Text.RegularExpressions;

namespace OWP.Infosite.Builder;

public static class SpecialMatchBuilder
{
    public static List<MappedChildMatch> ReduceSpecialMatch(IEnumerable<MappedChildMatch> childMatches)
    {
        var childMatchDict = childMatches
            .GroupBy(x => new { type = GetSpecialMatchType(x), x.ParentInfoSiteMatchId, x.EventDateTime })
            .ToDictionary(x => x.Key, x => x.ToList());
        var match = new List<MappedChildMatch>();

        foreach (var key in childMatchDict.Keys)
        {
            var firstMatch = childMatchDict[key].FirstOrDefault();
            if (key.type == 0)
            {
                match.AddRange(childMatchDict[key]);
            }
            else
            {
                match.Add(GetPresentationMatch(firstMatch, key.type));
            }
        }

        return match;
    }

    private static MappedChildMatch GetPresentationMatch(MappedChildMatch childMatch, int specialType)
    {
        Constant.SpecialMatchDict.TryGetValue(specialType, out var nameExtension);

        return new MappedChildMatch
        {
            ParentInfoSiteMatchId = childMatch.ParentInfoSiteMatchId,
            AdminSiteMatchId = childMatch.AdminSiteMatchId,
            LeagueName = childMatch.LeagueName,
            HomeName = GetOwpMatchName(childMatch.HomeName, nameExtension),
            AwayName = GetOwpMatchName(childMatch.AwayName, nameExtension),
            EventDateTime = childMatch.EventDateTime,
            SubType = childMatch.SubType,
            MinorBettypeId = childMatch.MinorBettypeId,
            UpdatedTime = childMatch.UpdatedTime
        };
    }

    private static string GetOwpMatchName(string matchName, string nameExtension)
    {
        var pattern = @"(((\d+\w{2})\s(\w+))|(\d{2}:\d{2}-\d{2}:\d{2}))$";

        return Regex.Replace(matchName, pattern, nameExtension);
    }

    private static int GetSpecialMatchType(MappedChildMatch childMatch)
    {
        if (IsNextCorner(childMatch))
            return Constant.CornerType;

        if (IsConditionOfBooking(childMatch))
            return Constant.BookingType;

        if (IsConditionOf15Min(childMatch))
            return Constant.Special15MinsType;

        return 0;
    }

    private static bool IsNextCorner(MappedChildMatch match)
    {
        return match.MinorBettypeId == Constant.NextCornerBettypeId &&
               char.IsNumber(char.Parse(match.AwayName.Substring(match.AwayName.Length - 10, 1)));
    }

    private static bool IsConditionOfBooking(MappedChildMatch match)
    {
        return match.MinorBettypeId == Constant.NextBookingBettypeId &&
               char.IsNumber(char.Parse(match.AwayName.Substring(match.AwayName.Length - 11, 1)));
    }

    private static bool IsConditionOf15Min(MappedChildMatch match)
    {
        return match.MinorBettypeId == Constant.Specific15MinBettypeId &&
               Constant.Special15Min.Any(x => CheckContainHomeAway(match, x));
    }

    private static bool CheckContainHomeAway(MappedChildMatch match, string typeSpecial)
    {
        return match.AwayName.IsContainsIgnoreCase(typeSpecial) &&
               match.HomeName.IsContainsIgnoreCase(typeSpecial);
    }
}
