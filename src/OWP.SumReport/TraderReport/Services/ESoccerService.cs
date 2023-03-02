namespace OWP.SumReport.TraderReport.Services;

using Enum;
using Extensions;
using Models;

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
            var eSoccer5Mins = assignments.Where(x => x.ReportType == ReportType.Esoccer5Mins);
            var eSoccer6Mins = assignments.Where(x => x.ReportType == ReportType.Esoccer6Mins);
            var eSoccer15Mins = assignments.Where(x => x.ReportType == ReportType.Esoccer15Mins);
            var eSoccer16Mins = assignments.Where(x => x.ReportType == ReportType.Esoccer16Mins);
            var eSoccer20Mins = assignments.Where(x => x.ReportType == ReportType.Esoccer20Mins);

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
        => eSoccer5Mins.Select(x => x.OwpMatchId).Distinct().Count();

    private static ReportType MapReportType(Assignment assignment)
    {
        if (assignment.OwpLeagueName.IsContainsIgnoreCase("- 5 Mins Play"))
        {
            return ReportType.Esoccer5Mins;
        }

        if (assignment.OwpLeagueName.IsContainsIgnoreCase("- 6 Mins Play"))
        {
            return ReportType.Esoccer6Mins;
        }

        if (assignment.OwpLeagueName.IsContainsIgnoreCase("- 15 Mins Play"))
        {
            return ReportType.Esoccer15Mins;
        }

        if (assignment.OwpLeagueName.IsContainsIgnoreCase("- 16 Mins Play"))
        {
            return ReportType.Esoccer16Mins;
        }

        return assignment.OwpLeagueName.IsContainsIgnoreCase("- 20 Mins Play") 
            ? ReportType.Esoccer20Mins 
            : 0;
    }
}