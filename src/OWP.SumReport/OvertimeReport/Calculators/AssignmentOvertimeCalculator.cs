using OWP.Core.Extensions;
using OWP.SumReport.OvertimeReport.Constants;
using OWP.SumReport.OvertimeReport.Helpers;
using OWP.SumReport.OvertimeReport.Models;

namespace OWP.SumReport.OvertimeReport.Calculators;

public static class AssignmentOvertimeCalculator
{
    private static readonly Dictionary<string, Func<Assignment, int>> sabaOperations =
        new Dictionary<string, Func<Assignment, int>>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "PES ", MatchHelper.CalculateSABAPESMatchDuration },
            { "FIFA ", MatchHelper.CalculateSABAFIFAMatchDuration },
        };

    public static decimal CalculateOvertimeData(
        IEnumerable<Assignment> userAssignments,
        EmployeeWorkingShift workingShift,
        IEnumerable<MainMatch> mainMatchRemarks)
    {
        var finalMatchEndTime = GetLatestMatchEndTime(userAssignments, workingShift, mainMatchRemarks);

        if (finalMatchEndTime <= workingShift.ShiftEndTime)
        {
            return 0;
        }

        return (decimal)(finalMatchEndTime - workingShift.ShiftEndTime).TotalHours;
    }

    public static DateTime GetLatestMatchEndTime(
        IEnumerable<Assignment> userAssignments,
        EmployeeWorkingShift workingShift,
        IEnumerable<MainMatch> mainMatchRemarks)
    {
        var latestAssignments =
            GetLatestAssignmentsInShift(userAssignments, workingShift.ShiftStartTime, workingShift.ShiftEndTime);

        if (!latestAssignments.Any())
        {
            return workingShift.ShiftEndTime;
        }

        var matchesDuration = new List<DateTime>();

        foreach (var assignment in latestAssignments)
        {
            if (assignment.ParentMatchId.HasValue &&
                (BetType.FullTimeMinorBetTypes.Contains(assignment.MinorBettypeId.Value) ||
                 BetType.MinorMatchBetTypes.Contains(assignment.MinorBettypeId.Value))
               )
            {
                var mainMatch = mainMatchRemarks.First(x => x.OWPMatchId == assignment.ParentMatchId.Value);

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
        if (BetType.MinorMatchBetTypes.Contains(assignment.MinorBettypeId.Value) &&
            MatchHelper.IsHavingETPK(mainMatch.Remark))
        {
            return (" ET", mainMatch.EventDateTime);
        }

        if (BetType.FullTimeMinorBetTypes.Contains(assignment.MinorBettypeId.Value))
        {
            return (mainMatch.Remark, mainMatch.EventDateTime);
        }

        return (assignment.Remark, mainMatch.EventDateTime);
    }

    public static DateTime CalculateMatchEndTime(Assignment assignment)
    {
        if (MatchHelper.IsSabaMatch(assignment.OWPLeagueName) &&
            BetType.FullTimeBetTypes.Contains(assignment.Bettype))
        {
            var method = sabaOperations.LookupByKey<Func<Assignment, int>>(assignment.OWPLeagueName, null);
            var duration = method != null ? method(assignment) : 0;

            return assignment.EventDateTime.AddMinutes(duration);
        }

        return assignment.EventDateTime.AddMinutes(MatchHelper.CalculateNormalMatchDuration(assignment));
    }

    public static IEnumerable<Assignment> GetLatestAssignmentsInShift(
        IEnumerable<Assignment> userAssignments,
        DateTime shiftStartTime,
        DateTime shiftEndTime)
    {
        var latestAssignments = userAssignments
            .Where(x => x.EventDateTime >= shiftStartTime && x.EventDateTime <= shiftEndTime);

        if (latestAssignments.Any())
        {
            var latestEventDateTime = latestAssignments.Max(x => x.EventDateTime);

            return userAssignments.Where(x => x.EventDateTime.Equals(latestEventDateTime));
        }

        return new List<Assignment>();
    }
}