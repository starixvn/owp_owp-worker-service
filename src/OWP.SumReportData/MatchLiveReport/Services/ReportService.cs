using System.Collections.Immutable;
using OWP.SumReportData.MatchLiveReport.Constants;
using OWP.SumReportData.MatchLiveReport.Models;
using OWP.SumReportData.MatchLiveReport.Repository;

namespace OWP.SumReportData.MatchLiveReport.Services;

public interface ReportService
{
    Task Calculate(DateTime eventDate);
}

public class ReportServiceImp : ReportService
{
    public static readonly IImmutableDictionary<BetType, BetTypeName> BetTypeMap = new Dictionary<BetType, BetTypeName>
    {
        {BetType.Corners, BetTypeName.Corners},
        {BetType.NextCorners, BetTypeName.NextCorners},
        {BetType.LastCorners, BetTypeName.LastCorners},
        {BetType.Specific15MinsNumberOfCorners, BetTypeName.Specific15MinsNumberOfCorners},
        {BetType.Specific15Mins, BetTypeName.Specific15Mins},
        {BetType.Specific10Mins, BetTypeName.Specific10Mins},
        {BetType.Bookings, BetTypeName.Bookings},
        {BetType.NextBookings, BetTypeName.NextBookings},
        {BetType.Specific15MinsTotalBookings, BetTypeName.Specific15MinsTotalBookings},
        {BetType.LastBookings, BetTypeName.LastBookings},
        {BetType.Winner, BetTypeName.Winner},
        {BetType.SpecialGoal, BetTypeName.SpecialGoal},
        {BetType.HomeSingleTeamOU, BetTypeName.SingleTeamOU},
        {BetType.AwaySingleTeamOU, BetTypeName.SingleTeamOU},
        {BetType.FirstPenaltyToScore, BetTypeName.FirstPenaltyToScore},
        {BetType.FirstPenaltyAwarded, BetTypeName.FirstPenaltyAwarded},
        {BetType.FreeKick, BetTypeName.FreeKick},
        {BetType.LastFreeKick, BetTypeName.LastFreeKick},
        {BetType.ThrowIn, BetTypeName.ThrowIn},
        {BetType.LastThrowIn, BetTypeName.LastThrowIn},
        {BetType.Offside, BetTypeName.Offside},
        {BetType.GoalKick, BetTypeName.GoalKick},
        {BetType.LastGoalKick, BetTypeName.LastGoalKick},
        {BetType.NextGoalKick, BetTypeName.NextGoalKick},
        {BetType.HomePenaltyShootOut, BetTypeName.PenaltyShootOut},
        {BetType.AwayPenaltyShootOut, BetTypeName.PenaltyShootOut},
        {BetType.WhichTeamToTakeTheFirstPenalty, BetTypeName.WhichTeamToTakeTheFirstPenalty},
    }.ToImmutableDictionary();

    private readonly ReportRepository reportRepository;

    public ReportServiceImp(ReportRepository reportRepository)
    {
        this.reportRepository = reportRepository;
    }

    public async Task Calculate(DateTime eventDate)
    {
        var matches = await reportRepository.QueryMatchesByDate(eventDate);
        var minorMatches = CategorizeMinorMatches(matches);
        var mainMatches = CategorizeMinorMatches(matches);
        var sabaMatches = CategorizeSabaMatches(matches);

        
    }

    private static IEnumerable<OWPMatch> CategorizeMinorMatches(IEnumerable<OWPMatch> matches)
    {
        var minorMatches = matches.Where(x => x.MinorBettypeId.HasValue);

        foreach (var match in minorMatches)
        {
            var isBetTypeExisted = BetTypeMap.TryGetValue((BetType)match.MinorBettypeId.Value, out var streamName);
            match.BetTypeName = isBetTypeExisted ? streamName : BetTypeName.Main;
        }

        return minorMatches;
    }

    private static IEnumerable<OWPMatch> CategorizeMainMatches(IEnumerable<OWPMatch> matches)
    {
        var mainMatches = matches.Where(x => !x.MinorBettypeId.HasValue && !x.IsSaba);

        foreach (var match in mainMatches)
        {
            match.BetTypeName = GetMainCategory(match.OWPLeagueName);
        }

        return mainMatches;
    }

    static BetTypeName GetMainCategory(string owpLeagueName)
    {
        BetTypeName result;

        if (owpLeagueName.Contains("FANTASY", StringComparison.InvariantCultureIgnoreCase))
        {
            result = BetTypeName.Fantasy;
        }
        else if (owpLeagueName.Contains("WHICH TEAM TO KICK OFF", StringComparison.InvariantCultureIgnoreCase))
        {
            result = BetTypeName.WhichTeamToKickOff;
        }
        else
        {
            result = BetTypeName.Main;
        }

        return result;
    }

    private static IEnumerable<OWPMatch> CategorizeSabaMatches(IEnumerable<OWPMatch> matches)
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
        BetTypeName result;

        if (owpLeagueName.Contains("15 Mins ", StringComparison.InvariantCultureIgnoreCase))
        {
            result = BetTypeName.SabaClubFriendlyVirtualPes2115MinsPlay;
        }
        else if (owpLeagueName.Contains("20 Mins ", StringComparison.InvariantCultureIgnoreCase))
        {
            result = BetTypeName.SabaInternationalFriendlyVirtualPes2120MinsPlay;
        }
        else
        {
            result = BetTypeName.SabaEliteFriendlyVirtualPes215MinsPlay;
        }

        return result;
    }

    public static BetTypeName GetFifaBetType(string owpLeagueName)
    {
        BetTypeName result;

        if (owpLeagueName.Contains("M88 SABA ", StringComparison.InvariantCultureIgnoreCase) &&
            owpLeagueName.Contains("16 Mins ", StringComparison.InvariantCultureIgnoreCase))
        {
            result = BetTypeName.M88SabaSpanishLeagueVirtualFifa21Season1016MinsPlay;
        }
        else if (owpLeagueName.Contains("16 Mins ", StringComparison.InvariantCultureIgnoreCase))
        {
            result = BetTypeName.SabaClubFriendlyVirtualFifa2116MinsPlay;
        }
        else
        {
            result = BetTypeName.SabaClubFriendlyVirtualFifa20Futsal6MinsPlay;
        }

        return result;
    }
}