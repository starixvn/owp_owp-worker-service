namespace OWP.SumReport.OvertimeReport.Helpers;

using Constants;
using Extensions;
using Models;

public static class MatchHelper
{
    private static readonly Dictionary<string, Func<Assignment, int>> PesOperations =
        new(StringComparer.InvariantCultureIgnoreCase)
        {
            { "15 Mins ", CalculateSabaPes15And20MinsMatchDuration },
            { "20 Mins ", CalculateSabaPes15And20MinsMatchDuration },
            { "5 Mins ", CalculateSabaPes5MinsMatchDuration }
        };

    public static bool IsSabaMatch(string leagueName)
        => leagueName.IndexOf("SABA ", StringComparison.CurrentCultureIgnoreCase) >= 0;

    public static int CalculateSabaPesMatchDuration(Assignment assignment)
    {
        var method = PesOperations.LookupByKey(assignment.OwpLeagueName, null);
        return method?.Invoke(assignment) ?? 0;
    }

    public static int CalculateSabaPes5MinsMatchDuration(Assignment assignment)
        => 60 - assignment.EventDateTime.Minute;

    public static int CalculateSabaPes15And20MinsMatchDuration(Assignment assignment)
        => IsHavingEtpk(assignment.Remark) || IsHavingPKOnly(assignment.Remark) ? 45 : 30;

    public static int CalculateSabaFifaMatchDuration(Assignment assignment)
        => IsHavingEtpk(assignment.Remark) || IsHavingPKOnly(assignment.Remark) ? 60 : 45;

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
        if (IsHavingEtpk(remark) || IsHavingETOnly(remark))
        {
            return 150;
        }

        return IsHavingPKOnly(remark) ? 135 : 120;
    }

    public static bool IsHavingEtpk(string remark)
        => remark.IndexOf(" ET", StringComparison.CurrentCultureIgnoreCase) >= 0
           && remark.IndexOf("PK", StringComparison.CurrentCultureIgnoreCase) >= 0;

    public static bool IsHavingPKOnly(string remark)
        => remark.IndexOf(" ET", StringComparison.CurrentCultureIgnoreCase) < 0
           && remark.IndexOf("PK", StringComparison.CurrentCultureIgnoreCase) >= 0;

    public static bool IsHavingETOnly(string remark)
        => remark.IndexOf(" ET", StringComparison.CurrentCultureIgnoreCase) >= 0
           && remark.IndexOf("PK", StringComparison.CurrentCultureIgnoreCase) < 0;
}