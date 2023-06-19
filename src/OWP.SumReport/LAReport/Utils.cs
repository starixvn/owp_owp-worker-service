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

        public static double CountSingleMatch(IList<Assignment> assignments, ReportType reportType)
        {
            var assignmentMapOwpId = assignments.Where(x => x.ReportType == reportType);
            return assignmentMapOwpId.Sum(assignment => assignment.SingleAssignment ? CountUnit.COUNT_FULL : CountUnit.COUNT_HALF);
        }

        public static double CountGroupSpecialProduct(IList<Assignment> assignments, ReportType reportType)
        {
            var count = 0.0;

            var assignmentMapParentId=assignments.Where(x => x.ReportType == reportType)
                .GroupBy(x => x.ParentMatchId)
                .ToImmutableDictionary(x => x.Key, x => x.ToList());
            
            foreach (var (_, assignment) in assignmentMapParentId)
            {
                count+=assignment.All(a => a.SingleAssignment) ? CountUnit.COUNT_FULL : CountUnit.COUNT_HALF;
            }
            return count;
        }

        public static double CountGroupMixedProduct(IList<Assignment> assignments, ReportType reportType)
        {
            var count = 0.0;

            var assignmentSpecific = assignments.Where(x => x.ReportType == reportType && x.ParentMatchId.HasValue)
                .GroupBy(x => x.ParentMatchId)
                .ToImmutableDictionary(x => x.Key, x => x.ToList());

            foreach (var (parentMatchId, assignment) in assignmentSpecific)
            {
                var parentGame = assignment.First(a => a.OwpMatchId == parentMatchId);
                count += parentGame.SingleAssignment && assignment.All(a => a.SingleAssignment) ? CountUnit.COUNT_FULL : CountUnit.COUNT_HALF;
            }
            return count;
        }

        public static IList<Assignment> GroupMultipleMatches(IList<Assignment> assignments)
        {
            var result = new List<Assignment>();
            var group = 1;

            var groupedAssignments = assignments
                .DistinctBy(x => x.OwpMatchId)
                .OrderBy(x => x.EventDateTime)
                .GroupBy(x => x.EventDateTime.AddHours(-12).Date)
                .Where(x => x.Any())
                .ToDictionary(x => x.Key, x => x.ToList());

            var groupedAssignmentKey = groupedAssignments.Select(assignment => assignment.Key);
            foreach (var key in groupedAssignmentKey)
            {
                groupedAssignments[key][0].LinkGroup = group;

                for (var i = 1; i < groupedAssignments[key].Count; i++)
                {
                    var isLinked = IsKoTimeDifferentWithin45Mins(groupedAssignments[key][i], groupedAssignments[key][i - 1]);

                    group = isLinked ? group : group + 1;
                    groupedAssignments[key][i].LinkGroup = group;
                }

                group += 1;

                result.AddRange(groupedAssignments[key]);
            }

            return result;
        }

        private static bool IsKoTimeDifferentWithin45Mins(Assignment assignmentA, Assignment assignmentB)
        {
            const int differenceInMinute = 45;
            return Math.Abs(assignmentA.EventDateTime.Subtract(assignmentB.EventDateTime).TotalMinutes) <= differenceInMinute;
        }

    }
}
