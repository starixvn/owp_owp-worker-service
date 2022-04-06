using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using OWP.Core.Extensions;
using OWP.SumReportData.MatchLiveReport.Constants;
using OWP.SumReportData.MatchLiveReport.Models;

namespace OWP.SumReportData.MatchLiveReport.Helpers;

public static class MatchHelper
{
    private static readonly IImmutableDictionary<MinorBetType, BetTypeName> BetTypeMap = new Dictionary<MinorBetType, BetTypeName>
    {
        {MinorBetType.Corners, BetTypeName.Corners},
        {MinorBetType.NextCorners, BetTypeName.NextCorners},
        {MinorBetType.LastCorners, BetTypeName.LastCorners},
        {MinorBetType.Specific15MinsNumberOfCorners, BetTypeName.Specific15MinsNumberOfCorners},
        {MinorBetType.Specific15Mins, BetTypeName.Specific15Mins},
        {MinorBetType.Specific10Mins, BetTypeName.Specific10Mins},
        {MinorBetType.Bookings, BetTypeName.Bookings},
        {MinorBetType.NextBookings, BetTypeName.NextBookings},
        {MinorBetType.Specific15MinsTotalBookings, BetTypeName.Specific15MinsTotalBookings},
        {MinorBetType.LastBookings, BetTypeName.LastBookings},
        {MinorBetType.Winner, BetTypeName.Winner},
        {MinorBetType.SpecialGoal, BetTypeName.SpecialGoal},
        {MinorBetType.HomeSingleTeamOU, BetTypeName.SingleTeamOU},
        {MinorBetType.AwaySingleTeamOU, BetTypeName.SingleTeamOU},
        {MinorBetType.FirstPenaltyToScore, BetTypeName.FirstPenaltyToScore},
        {MinorBetType.FirstPenaltyAwarded, BetTypeName.FirstPenaltyAwarded},
        {MinorBetType.FreeKick, BetTypeName.FreeKick},
        {MinorBetType.LastFreeKick, BetTypeName.LastFreeKick},
        {MinorBetType.ThrowIn, BetTypeName.ThrowIn},
        {MinorBetType.LastThrowIn, BetTypeName.LastThrowIn},
        {MinorBetType.Offside, BetTypeName.Offside},
        {MinorBetType.GoalKick, BetTypeName.GoalKick},
        {MinorBetType.LastGoalKick, BetTypeName.LastGoalKick},
        {MinorBetType.NextGoalKick, BetTypeName.NextGoalKick},
        {MinorBetType.HomePenaltyShootOut, BetTypeName.PenaltyShootOut},
        {MinorBetType.AwayPenaltyShootOut, BetTypeName.PenaltyShootOut},
        {MinorBetType.WhichTeamToTakeTheFirstPenalty, BetTypeName.WhichTeamToTakeTheFirstPenalty},
    }.ToImmutableDictionary();

    public static IEnumerable<ReportModel> SumMatches(DateTime eventDate, IEnumerable<OWPMatch> matches)
    {
        var isSaba = matches.Any(x => x.IsSaba);
        var matchTypeDict = matches.GroupBy(x => x.BetTypeName).ToDictionary(x => x.Key, x => x.AsQueryable());

        foreach (var type in matchTypeDict.Keys)
        {
            var typeGroupedMatches = matchTypeDict[type];
            var leagueCategoryDict = typeGroupedMatches.GroupBy(x => x.LeagueCategory).ToDictionary(x => x.Key, x => x.ToList());

            foreach (var category in leagueCategoryDict.Keys)
            {
                var categorizedMatches = leagueCategoryDict[category];

                yield return new ReportModel
                {
                    BetType = type.GetAttribute<DisplayAttribute>().Name,
                    EventDate = eventDate,
                    IsSaba = isSaba,
                    MatchCount = categorizedMatches.Count(),
                    LeagueCategory = category
                };
            }
        }
    }

    public static IEnumerable<OWPMatch> CategorizeMinorMatches(IEnumerable<OWPMatch> matches)
    {
        var minorMatches = matches.Where(x => x.MinorBettypeId.HasValue);

        foreach (var match in minorMatches)
        {
            var isBetTypeExisted = BetTypeMap.TryGetValue((MinorBetType)match.MinorBettypeId.Value, out var streamName);
            match.BetTypeName = isBetTypeExisted ? streamName : BetTypeName.Other;
        }

        return minorMatches.Where(x => x.BetTypeName != BetTypeName.Other);
    }

    public static IEnumerable<OWPMatch> CategorizeMainMatches(IEnumerable<OWPMatch> matches)
    {
        var mainMatches = matches.Where(x => !x.MinorBettypeId.HasValue && !x.IsSaba);

        foreach (var match in mainMatches)
        {
            match.BetTypeName = GetMainCategory(match.OWPLeagueName);
        }

        return mainMatches;
    }

    public static BetTypeName GetMainCategory(string owpLeagueName)
    {
        if (owpLeagueName.Contains("FANTASY ", StringComparison.InvariantCultureIgnoreCase))
        {
            return BetTypeName.Fantasy;
        }
        else if (owpLeagueName.Contains("WHICH TEAM TO KICK OFF", StringComparison.InvariantCultureIgnoreCase))
        {
            return BetTypeName.WhichTeamToKickOff;
        }

        return BetTypeName.Main;
    }

    public static IEnumerable<OWPMatch> CategorizeSabaMatches(IEnumerable<OWPMatch> matches)
    {
        var sabaMatches = matches.Where(x => !x.MinorBettypeId.HasValue && x.IsSaba);
        var fifaMatches = sabaMatches.Where(x => IsFifa(x.OWPLeagueName));
        var pesMatches = sabaMatches.Where(x => IsPes(x.OWPLeagueName));

        foreach (var match in fifaMatches)
        {
            match.BetTypeName = GetFifaBetType(match.OWPLeagueName);
        }

        foreach (var match in pesMatches)
        {
            match.BetTypeName = GetPesBetType(match.OWPLeagueName);
        }

        return fifaMatches.Concat(pesMatches);
    }

    public static bool IsPes(string owpLeagueName) => owpLeagueName.Contains("PES ", StringComparison.InvariantCultureIgnoreCase);
    
    public static bool IsFifa(string owpLeagueName) => owpLeagueName.Contains("FIFA ", StringComparison.InvariantCultureIgnoreCase);

    public static BetTypeName GetPesBetType(string owpLeagueName)
    {
        if (owpLeagueName.Contains("15 Mins ", StringComparison.InvariantCultureIgnoreCase))
        {
            return BetTypeName.SabaClubFriendlyVirtualPes2115MinsPlay;
        }
        else if (owpLeagueName.Contains("20 Mins ", StringComparison.InvariantCultureIgnoreCase))
        {
            return BetTypeName.SabaInternationalFriendlyVirtualPes2120MinsPlay;
        }

        return BetTypeName.SabaEliteFriendlyVirtualPes215MinsPlay;
    }

    public static BetTypeName GetFifaBetType(string owpLeagueName)
    {
        if (owpLeagueName.Contains("M88 SABA ", StringComparison.InvariantCultureIgnoreCase) &&
            owpLeagueName.Contains("16 Mins ", StringComparison.InvariantCultureIgnoreCase))
        {
            return BetTypeName.M88SabaSpanishLeagueVirtualFifa21Season1016MinsPlay;
        }
        else if (owpLeagueName.Contains("16 Mins ", StringComparison.InvariantCultureIgnoreCase))
        {
            return BetTypeName.SabaClubFriendlyVirtualFifa2116MinsPlay;
        }

        return BetTypeName.SabaClubFriendlyVirtualFifa20Futsal6MinsPlay;
    }
}