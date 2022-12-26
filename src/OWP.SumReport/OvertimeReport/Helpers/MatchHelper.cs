using OWP.Core.Extensions;
using OWP.SumReport.OvertimeReport.Constants;
using OWP.SumReport.OvertimeReport.Models;

namespace OWP.SumReport.OvertimeReport.Helpers;

public static class MatchHelper
{
    private static readonly Dictionary<string, Func<Assignment, int>> pesOperations =
        new Dictionary<string, Func<Assignment, int>>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "15 Mins ", CalculateSABAPES15And20MinsMatchDuration },
            { "20 Mins ", CalculateSABAPES15And20MinsMatchDuration },
            { "5 Mins ", CalculateSABAPES5MinsMatchDuration }
        };

    public static bool IsSabaMatch(string leagueName)
        => leagueName.IndexOf("SABA ", StringComparison.CurrentCultureIgnoreCase) >= 0;

    public static int CalculateSABAPESMatchDuration(Assignment assignment)
    {
        var method = pesOperations.LookupByKey<Func<Assignment, int>>(assignment.OWPLeagueName, null);
        return method != null ? method(assignment) : 0;
    }

    public static int CalculateSABAPES5MinsMatchDuration(Assignment assignment)
        => 60 - assignment.EventDateTime.Minute;

    public static int CalculateSABAPES15And20MinsMatchDuration(Assignment assignment)
        => IsHavingETPK(assignment.Remark) || IsHavingPKOnly(assignment.Remark) ? 45 : 30;

    public static int CalculateSABAFIFAMatchDuration(Assignment assignment)
        => IsHavingETPK(assignment.Remark) || IsHavingPKOnly(assignment.Remark) ? 60 : 45;

    public static int CalculateNormalMatchDuration(Assignment assignment)
    {
        if (BetType.FirstHalfBetTypes.Contains(assignment.Bettype))
        {
            return 60;
        }

        if (!assignment.MinorBettypeId.HasValue ||
            BetType.FullTimeMinorBetTypes.Contains(assignment.MinorBettypeId.Value) ||
            BetType.MinorMatchBetTypes.Contains(assignment.MinorBettypeId.Value))
        {
            return CalculateNormalMatchDuration(assignment.Remark);
        }

        return 0;
    }

    public static int CalculateNormalMatchDuration(string remark)
    {
        if (IsHavingETPK(remark) || IsHavingETOnly(remark))
        {
            return 150;
        }

        return IsHavingPKOnly(remark) ? 135 : 120;
    }

    public static bool IsHavingETPK(string remark)
        => remark.IndexOf(" ET", StringComparison.CurrentCultureIgnoreCase) >= 0
           && remark.IndexOf("PK", StringComparison.CurrentCultureIgnoreCase) >= 0;

    public static bool IsHavingPKOnly(string remark)
        => remark.IndexOf(" ET", StringComparison.CurrentCultureIgnoreCase) < 0
           && remark.IndexOf("PK", StringComparison.CurrentCultureIgnoreCase) >= 0;

    public static bool IsHavingETOnly(string remark)
        => remark.IndexOf(" ET", StringComparison.CurrentCultureIgnoreCase) >= 0
           && remark.IndexOf("PK", StringComparison.CurrentCultureIgnoreCase) < 0;
}