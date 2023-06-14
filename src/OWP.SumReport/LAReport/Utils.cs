using System.Collections.Immutable;
using OWP.SumReport.LAReport.Enum;
using OWP.SumReport.LAReport.Models;

namespace OWP.SumReport.LAReport
{
    public static class Utils
    {
        private const double INCREASE_HALF = 0.5;
        private const double INCREASE_FULL = 1;
        private const int NUMBER_MATCH_DECIDED_INCREASE = 1;

        public static ImmutableDictionary<int, int> CountOwpMatchIdMap(IList<Assignment> assignments)
            => assignments.GroupBy(x => x.OwpMatchId)
                .ToImmutableDictionary(x => x.Key, x => x.ToList().Count);

        public static double SumSpecialProduct(IList<Assignment> assignments,
            ImmutableDictionary<int, int> countOwpMatchIdMap)
        {
            return assignments.Sum(assignment => countOwpMatchIdMap[assignment.OwpMatchId] == NUMBER_MATCH_DECIDED_INCREASE
                ? INCREASE_FULL
                : INCREASE_HALF);
        }
        
        public static double DecidedIncreaseFullOrHalfSingleAndOthers(int owpMatchId,
            ImmutableDictionary<int, int> countOwpMatchIdMap)
        => countOwpMatchIdMap[owpMatchId] == NUMBER_MATCH_DECIDED_INCREASE
            ? INCREASE_FULL
            : INCREASE_HALF;

        public static (double totalAssignmentGroup, double totalAssignment) CountMatchInEachGroup(
            List<GroupMatchInfo> groupMatchInfo,
            ImmutableDictionary<int, int> countOwpMatchIdMap)
        {
            var count = 0.0;
            var total = 0.0;
            var owpMatchId= groupMatchInfo
                .Select(g=>g.OwpMatchId);

            foreach (var ids in owpMatchId)
            {
                count += DecidedIncreaseFullOrHalfInGroup(ids, countOwpMatchIdMap);
                total += ids.Sum(id => DecidedIncreaseFullOrHalfSingleAndOthers(id, countOwpMatchIdMap));
            }

            return (count, total);
        }


            public static double DecidedIncreaseFullOrHalfInGroup(IList<int> matchIdTarget,
            ImmutableDictionary<int, int> countOwpMatchIdMap)
            => matchIdTarget.All(id => countOwpMatchIdMap[id] == 2)
                ? INCREASE_HALF
                : INCREASE_FULL;

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
