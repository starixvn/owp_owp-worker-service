using OWP.Core.Extensions;
using OWP.SumReport.TraderReport.Enum;
using OWP.SumReport.TraderReport.Models;

namespace OWP.SumReport.TraderReport.Services;

public interface ESoccerService
{
    Task<IEnumerable<ESoccerByUser>> SumESoccerFields(DateTime eventDate, bool isGenerateData);
}

public class ESoccerServiceImp : ESoccerService
{
    private readonly TraderRepository repository;

    public ESoccerServiceImp(TraderRepository repository)
    {
        this.repository = repository;
    }

    public async Task<IEnumerable<ESoccerByUser>> SumESoccerFields(DateTime eventDate, bool isGenerateData)
    {
        var saBaAssignments = await repository.GetSaBaAssignmentByDate(eventDate, isGenerateData);
        saBaAssignments.ForEach(x => x.ReportType = MapReportType(x));

        var saBaAssignmentDict = saBaAssignments
            .GroupBy(x => x.UserId)
            .ToDictionary(x => x.Key, x => x.ToList());

        return SumESoccerByUser(saBaAssignmentDict);
    }

    private static IEnumerable<ESoccerByUser> SumESoccerByUser(Dictionary<int, List<Assignment>> saBaAssignmentDict)
    {
        foreach (var userId in saBaAssignmentDict.Keys)
        {
            var assignments = saBaAssignmentDict[userId];
            var eSoccer5Mins = assignments.Where(x => x.ReportType == ReportType.ESOCCER_5MINS);
            var eSoccer6Mins = assignments.Where(x => x.ReportType == ReportType.ESOCCER_6MINS);
            var eSoccer15Mins = assignments.Where(x => x.ReportType == ReportType.ESOCCER_15MINS);
            var eSoccer16Mins = assignments.Where(x => x.ReportType == ReportType.ESOCCER_16MINS);
            var eSoccer20Mins = assignments.Where(x => x.ReportType == ReportType.ESOCCER_20MINS);

            yield return new ESoccerByUser
            {
                UserId = userId,
                ESoccer5Mins = CountESoccer(eSoccer5Mins),
                ESoccer6Mins = CountESoccer(eSoccer6Mins),
                ESoccer15Mins = CountESoccer(eSoccer15Mins),
                ESoccer16Mins = CountESoccer(eSoccer16Mins),
                ESoccer20Mins = CountESoccer(eSoccer20Mins)
            };
        }
    }

    private static int CountESoccer(IEnumerable<Assignment> eSoccer5Mins)
        => eSoccer5Mins.Select(x => x.OWPMatchId).Distinct().Count();

    private static ReportType MapReportType(Assignment assignment)
    {
        if (assignment.OWPLeagueName.IsContainsIgnoreCase("- 5 Mins Play"))
        {
            return ReportType.ESOCCER_5MINS;
        }

        if (assignment.OWPLeagueName.IsContainsIgnoreCase("- 6 Mins Play"))
        {
            return ReportType.ESOCCER_6MINS;
        }

        if (assignment.OWPLeagueName.IsContainsIgnoreCase("- 15 Mins Play"))
        {
            return ReportType.ESOCCER_15MINS;
        }

        if (assignment.OWPLeagueName.IsContainsIgnoreCase("- 16 Mins Play"))
        {
            return ReportType.ESOCCER_16MINS;
        }

        if (assignment.OWPLeagueName.IsContainsIgnoreCase("- 20 Mins Play"))
        {
            return ReportType.ESOCCER_20MINS;
        }

        return 0;
    }
}