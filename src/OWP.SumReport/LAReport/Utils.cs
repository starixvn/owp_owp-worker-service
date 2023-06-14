using System.Collections.Immutable;
using OWP.SumReport.LAReport.Constants;
using OWP.SumReport.LAReport.Enum;
using OWP.SumReport.LAReport.Models;

namespace OWP.SumReport.LAReport
{
    public static class Utils
    {
        public static ImmutableDictionary<int, int> CountOwpMatchIdMap(IList<Assignment> assignments)
            => assignments.GroupBy(x => x.OwpMatchId)
                .ToImmutableDictionary(x => x.Key, x => x.ToList().Count);

        public static (double totalAssignmentGroup, double totalAssignment) CountMatchInEachGroup(
            List<GroupMatchInfo> groupMatchInfo)
        {
            var count = 0.0;
            var total = 0.0;
            var assignments= groupMatchInfo
                .Select(g=>g.Assignments);

            foreach (var assignment in assignments)
            {
                count += DecidedIncreaseFullOrHalfInGroup(assignment);
                total += assignment.Sum(a => a.Count);
            }

            return (count, total);
        }

        public static double DecidedIncreaseFullOrHalfInGroup(IList<Assignment> assignments)
            => assignments.All(a => Math.Abs(a.Count - 0.5) <= .00001)
                ? CountUnit.COUNT_HALF
                : CountUnit.COUNT_FULL;

        public static void UpdateReportType(IList<Assignment> assignments, IList<int>multipleMatchGroup)
        {
            var multipleLinkMapOwpId = assignments
                .Where(x => multipleMatchGroup
                    .Contains(x.LinkGroup))
                .Select(m => m.OwpMatchId)
                .ToDictionary(x => x);

            var assignmentUpdated =
                assignments.Where(assignment => multipleLinkMapOwpId.ContainsKey(assignment.OwpMatchId));
            foreach (var assignment in assignmentUpdated)
            {
                assignment.ReportType = ReportType.MultipleMatches;
            }
        }

    }
}
