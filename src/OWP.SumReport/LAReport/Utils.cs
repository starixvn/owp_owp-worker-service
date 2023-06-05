using System.Collections.Immutable;
using OWP.SumReport.LAReport.Models;

namespace OWP.SumReport.LAReport
{
    public class Utils
    {
        private const double INCREASE_HALF = 0.5;
        private const double INCREASE_FULL = 1;
        private const double INIT_TOTAL = 0;
        private const int NUMBER_MATCH_DECIDED_INCREASE = 1;

        public static ImmutableDictionary<int, int> CountOwpMatchIdMap(IList<Assignment> assignments)
            => assignments.GroupBy(x => x.OwpMatchId)
                .ToImmutableDictionary(x => x.Key, x => x.ToList().Count);

        public static double SumSpecialProduct(IList<Assignment> assignments,
            ImmutableDictionary<int, int> countOwpMatchIdMap)
        {
            var total = INIT_TOTAL;
            foreach (var assignment in assignments)
            {
                total += countOwpMatchIdMap[assignment.OwpMatchId] == NUMBER_MATCH_DECIDED_INCREASE
                    ? INCREASE_FULL
                    : INCREASE_HALF;
            }
            return total;

        } public static double DecidedIncreaseSingleAndOthers(int owpMatchId,
            ImmutableDictionary<int, int> countOwpMatchIdMap)
        => countOwpMatchIdMap[owpMatchId] == NUMBER_MATCH_DECIDED_INCREASE
            ? INCREASE_FULL
            : INCREASE_HALF;
    }
}
