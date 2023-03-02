namespace OWP.SumReport.MatchLiveReport.Helpers;

using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using Constants;
using Extensions;
using Models;

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
        {MinorBetType.HomeSingleTeamOu, BetTypeName.SingleTeamOu},
        {MinorBetType.AwaySingleTeamOu, BetTypeName.SingleTeamOu},
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
        {MinorBetType.WhichTeamToTakeTheFirstPenalty, BetTypeName.WhichTeamToTakeTheFirstPenalty}
    }.ToImmutableDictionary();

    public static IEnumerable<ReportModel> SumMatches(DateTime eventDate, IList<OwpMatch> matches)
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
                    MatchCount = categorizedMatches.Count,
                    LeagueCategory = category
                };
            }
        }
    }

    public static IEnumerable<OwpMatch> CategorizeMinorMatches(IList<OwpMatch> matches, IList<OwpMatch> mainMatches)
    {
        var minorMatches = matches.Where(x => x.MinorBettypeId.HasValue && !x.IsSaba).ToList();

        foreach (var match in minorMatches)
        {
            var isBetTypeExisted = BetTypeMap.TryGetValue((MinorBetType)(match.MinorBettypeId ?? 0), out var streamName);
            match.BetTypeName = isBetTypeExisted ? streamName : BetTypeName.Other;
            match.LeagueCategory = GetMainLeagueCategory(match, mainMatches);
        }

        return minorMatches.Where(x => x.BetTypeName != BetTypeName.Other);
    }

    public static string GetMainLeagueCategory(OwpMatch minorMatch, IEnumerable<OwpMatch> mainMatches)
        => mainMatches.FirstOrDefault(x => x.OwpMatchId == minorMatch.ParentMatchId)?.LeagueCategory ?? minorMatch.LeagueCategory;

    public static IEnumerable<OwpMatch> CategorizeMainMatches(IEnumerable<OwpMatch> matches)
    {
        var mainMatches = matches.Where(x => !x.MinorBettypeId.HasValue && !x.IsSaba).ToList();

        foreach (var match in mainMatches)
        {
            match.BetTypeName = GetMainCategory(match.OwpLeagueName);
        }

        return mainMatches;
    }

    public static BetTypeName GetMainCategory(string owpLeagueName)
    {
        if (owpLeagueName.Contains("FANTASY ", StringComparison.InvariantCultureIgnoreCase))
        {
            return BetTypeName.Fantasy;
        }

        return owpLeagueName.Contains("WHICH TEAM TO KICK OFF", StringComparison.InvariantCultureIgnoreCase) 
            ? BetTypeName.WhichTeamToKickOff 
            : BetTypeName.Main;
    }

    public static IEnumerable<OwpMatch> CategorizeSabaMatches(IEnumerable<OwpMatch> matches)
    {
        var sabaMatches = matches.Where(x => !x.MinorBettypeId.HasValue && x.IsSaba).ToList();
        var fifaMatches = sabaMatches.Where(x => IsFifa(x.OwpLeagueName)).ToList();
        var pesMatches = sabaMatches.Where(x => IsPes(x.OwpLeagueName)).ToList();

        foreach (var match in fifaMatches)
        {
            match.BetTypeName = GetFifaBetType(match.OwpLeagueName);
        }

        foreach (var match in pesMatches)
        {
            match.BetTypeName = GetPesBetType(match.OwpLeagueName);
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

        return owpLeagueName.Contains("20 Mins ", StringComparison.InvariantCultureIgnoreCase) 
            ? BetTypeName.SabaInternationalFriendlyVirtualPes2120MinsPlay
            : BetTypeName.SabaEliteFriendlyVirtualPes215MinsPlay;
    }

    public static BetTypeName GetFifaBetType(string owpLeagueName)
    {
        if (owpLeagueName.Contains("M88 SABA ", StringComparison.InvariantCultureIgnoreCase) &&
            owpLeagueName.Contains("16 Mins ", StringComparison.InvariantCultureIgnoreCase))
        {
            return BetTypeName.M88SabaSpanishLeagueVirtualFifa21Season1016MinsPlay;
        }

        return owpLeagueName.Contains("16 Mins ", StringComparison.InvariantCultureIgnoreCase)
            ? BetTypeName.SabaClubFriendlyVirtualFifa2116MinsPlay 
            : BetTypeName.SabaClubFriendlyVirtualFifa20Futsal6MinsPlay;
    }
}