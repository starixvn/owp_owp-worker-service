namespace OWP.SumReport.OvertimeReport.Calculators;

using Constants;
using Extensions;
using Helpers;
using Models;

public static class AssignmentOvertimeCalculator
{
    private static readonly Dictionary<string, Func<Assignment, int>> SabaOperations =
        new(StringComparer.InvariantCultureIgnoreCase)
        {
            { "PES ", MatchHelper.CalculateSabaPesMatchDuration },
            { "FIFA ", MatchHelper.CalculateSabaFifaMatchDuration }
        };

    public static decimal CalculateOvertimeData(
        EmployeeWorkingShift workingShift,
        IList<Assignment> userAssignments,
        IList<MainMatch> mainMatchRemarks)
    {
        var finalMatchEndTime = GetLatestMatchEndTime(workingShift, userAssignments, mainMatchRemarks);

        if (finalMatchEndTime <= workingShift.ShiftEndTime)
        {
            return 0;
        }

        return (decimal)(finalMatchEndTime - workingShift.ShiftEndTime).TotalHours;
    }

    public static DateTime GetLatestMatchEndTime(
        EmployeeWorkingShift workingShift,
        IList<Assignment> userAssignments,
        IList<MainMatch> mainMatchRemarks)
    {
        var latestAssignments = GetLatestAssignmentsInShift(userAssignments, workingShift.ShiftStartTime, workingShift.ShiftEndTime).ToList();

        if (!latestAssignments.Any())
        {
            return workingShift.ShiftEndTime;
        }

        var matchesDuration = new List<DateTime>();

        foreach (var assignment in latestAssignments)
        {
            if (assignment.ParentMatchId.HasValue &&
                (BetType.FullTimeMinorBetTypes.Contains(assignment.MinorBettypeId ?? int.MinValue) ||
                 BetType.MinorMatchBetTypes.Contains(assignment.MinorBettypeId ?? int.MinValue))
               )
            {
                var mainMatch = mainMatchRemarks.FirstOrDefault(x => x.OwpMatchId == assignment.ParentMatchId.Value);

                if (mainMatch != null)
                {
                    var (mainMatchRemark, eventDateTime) = GetMainMatchInfo(mainMatch, assignment);
                    assignment.Remark = mainMatchRemark;
                    assignment.EventDateTime = eventDateTime;
                }
            }

            matchesDuration.Add(CalculateMatchEndTime(assignment));
        }

        return matchesDuration.Max();
    }

    public static (string, DateTime) GetMainMatchInfo(MainMatch mainMatch, Assignment assignment)
    {
        if (BetType.MinorMatchBetTypes.Contains(assignment.MinorBettypeId ?? 0) &&
            MatchHelper.IsHavingEtpk(mainMatch.Remark))
        {
            return (" ET", mainMatch.EventDateTime);
        }

        return BetType.FullTimeMinorBetTypes.Contains(assignment.MinorBettypeId ?? int.MinValue)
            ? (mainMatch.Remark, mainMatch.EventDateTime)
            : (assignment.Remark, mainMatch.EventDateTime);
    }

    public static DateTime CalculateMatchEndTime(Assignment assignment)
    {
        if (!MatchHelper.IsSabaMatch(assignment.OwpLeagueName) ||
            !BetType.FullTimeBetTypes.Contains(assignment.Bettype))
        {
            return assignment.EventDateTime.AddMinutes(MatchHelper.CalculateNormalMatchDuration(assignment));
        }

        var method = SabaOperations.LookupByKey(assignment.OwpLeagueName, null);
        var duration = method?.Invoke(assignment) ?? 0;

        return assignment.EventDateTime.AddMinutes(duration);
    }

    public static IEnumerable<Assignment> GetLatestAssignmentsInShift(
        IList<Assignment> userAssignments,
        DateTime shiftStartTime,
        DateTime shiftEndTime)
    {
        var latestAssignments = userAssignments
            .Where(x => x.EventDateTime >= shiftStartTime && x.EventDateTime <= shiftEndTime)
            .ToList();

        if (!latestAssignments.Any())
        {
            return new List<Assignment>();
        }

        var latestEventDateTime = latestAssignments.Max(x => x.EventDateTime);

        return userAssignments.Where(x => x.EventDateTime.Equals(latestEventDateTime));
    }
}