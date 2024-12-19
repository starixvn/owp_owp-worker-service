using AutoMapper;
using OWP.SumReport.LAReport.Enum;
using OWP.SumReport.LAReport.Models;

namespace OWP.SumReport.LAReport.Services;

public interface MultipleProductService
{
    void SumUp(IList<Assignment> assignments, LaReportModel report);
}

public class MultipleProductServiceImp(IMapper mapper) : MultipleProductService
{
    public void SumUp(IList<Assignment> assignments, LaReportModel report)
    {
        SplitSingleAssignment(assignments);

        Execute(assignments, report);
    }

    private static void Execute(IList<Assignment> assignments, LaReportModel sumUpResult)
    {
        var othersAssignments = assignments
            .Where(x => x.ReportType == ReportType.Others)
            .ToList();

        var consecutiveMultipleMatchesAssignments = Utils.GroupMultipleMatches(othersAssignments);

        var matchGroupAssignments = consecutiveMultipleMatchesAssignments
            .GroupBy(i => i.LinkGroup)
            .Select(i => new { LinkGroup = i, MatchCount = i.Count() })
            .ToArray();


        var multipleMatchGroup = matchGroupAssignments
            .Where(x => x.MatchCount > 1)
            .ToList();

        var linkGroup = multipleMatchGroup
            .Select(x => x.LinkGroup)
            .Select(x => x.Key);

        assignments.Where(x => linkGroup.Contains(x.LinkGroup))
            .ToList()
            .ForEach(x => { x.ReportType = ReportType.MultipleMatches; });

        var link2 = matchGroupAssignments.Count(x => x.MatchCount == 2);
        var link3 = matchGroupAssignments.Count(x => x.MatchCount == 3);
        var link4 = matchGroupAssignments.Count(x => x.MatchCount == 4);
        var link5 = matchGroupAssignments.Count(x => x.MatchCount == 5);
        var link6 = matchGroupAssignments.Count(x => x.MatchCount == 6);
        var link7 = matchGroupAssignments.Count(x => x.MatchCount == 7);
        var link8Plus = matchGroupAssignments.Count(x => x.MatchCount == 8);

        if (StopSumUpFirstCondition(link2, link3, link4, link5) && StopSumUpSecondCondition(link6, link7, link8Plus))
        {
            return;
        }

        sumUpResult.Link2 += link2 / 2.0;
        sumUpResult.Link3 += link3 / 2.0;
        sumUpResult.Link4 += link4 / 2.0;
        sumUpResult.Link5 += link5 / 2.0;
        sumUpResult.Link6 += link6 / 2.0;
        sumUpResult.Link7 += link7 / 2.0;
        sumUpResult.Link8Plus += link8Plus / 2.0;

        var totalMatch = multipleMatchGroup
            .Sum(assignment => assignment.MatchCount / 2.0);

        sumUpResult.TotalMatchInGroupMultiple += totalMatch;

        Execute(assignments, sumUpResult);
    }

    private static bool StopSumUpFirstCondition(int link2, int link3, int link4, int link5)
        => link2 == 0 && link3 == 0 && link4 == 0 && link5 == 0;

    private static bool StopSumUpSecondCondition(int link6, int link7, int link8Plus)
        => link6 == 0 && link7 == 0 && link8Plus == 0;

    private void SplitSingleAssignment(ICollection<Assignment> assignments)
    {
        var othersAssignments = assignments
            .Where(x => x.ReportType == ReportType.Others && x.SingleAssignment)
            .ToList();

        foreach (var assignment in othersAssignments)
        {
            assignment.SingleAssignment = false;
            var newAssignmentSimilar = mapper.Map<Assignment>(assignment);
            assignments.Add(newAssignmentSimilar);
        }
    }
}