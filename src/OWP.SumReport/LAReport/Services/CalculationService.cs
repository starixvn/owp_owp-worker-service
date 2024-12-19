namespace OWP.SumReport.LAReport.Services;

using Enum;
using Extensions;
using Factory;
using Models;
using Constants;

public interface CalculationService
{
    Task<IEnumerable<LaReportModel>> Calculate(DateTime eventDate, bool isGenerateData);
}

public class CalculationServiceImp(
    LaRepository repository,
    SpecialProductFactory specialProductFactory,
    MainMatchRuleFactory mainMatchRuleFactory,
    SumUpService sumUpService)
    : CalculationService
{
    public async Task<IEnumerable<LaReportModel>> Calculate(DateTime eventDate, bool isGenerateData)
    {
        var assignments = await GetAssignment(eventDate, isGenerateData);
        var userSections = await repository.GetUserSection();

        return Calculate(assignments, userSections);
    }

    private IEnumerable<LaReportModel> Calculate(IDictionary<int, List<Assignment>> userIdToAssignmentMap,
        Dictionary<int, string> userSections)
    {
        var userIds = userIdToAssignmentMap.Select(x => x.Key);

        foreach (var id in userIds)
        {
            var categorizeAssignments = CategorizeAssignments(userIdToAssignmentMap[id]);
            var reportModel = new LaReportModel
            {
                UserId = id,
                Username = userSections.GetValue(id, string.Empty)
            };
            reportModel.Username = userSections.GetValue(id, string.Empty);
            sumUpService.Execute(categorizeAssignments, reportModel);
           

            yield return reportModel;
        }
    }
    private async Task<IDictionary<int, List<Assignment>>> GetAssignment(DateTime eventDate, bool isGenerateData)
    {
        var assignments = (await repository.GetLaAssignment(eventDate, isGenerateData)).ToList();

        CalculateCountUnit(assignments);

        var assignmentToUserIdMap = assignments.GroupBy(x => x.UserId)
            .Where(x => x.Any())
            .ToDictionary(x => x.Key, x => x.ToList());

        return new Dictionary<int, List<Assignment>>(assignmentToUserIdMap);
    }

    private static void CalculateCountUnit(IList<Assignment> assignments)
    {
        var owpMatchIdToMapCount = Utils.CountOwpMatchIdMap(assignments);
        foreach (var assignment in assignments)
        {
            if (owpMatchIdToMapCount[assignment.OwpMatchId] == CountUnit.NUMBER_MATCH_DECIDED_INCREASE)
            {
                assignment.SingleAssignment = true;
            }
        }
    }

    private IList<Assignment> CategorizeAssignments(IList<Assignment> assignments)
    {
        var result = new List<Assignment>();

        var notCornerRule = specialProductFactory.Create(RuleType.NotCorner);
        var cornerRule = specialProductFactory.Create(RuleType.Corner);
        notCornerRule.SetSuccessor(cornerRule);

        var ftCorner5MlRule = mainMatchRuleFactory.Create(RuleType.FtCornerFiveMajorLeague);
        var ftCornerRule = mainMatchRuleFactory.Create(RuleType.FtCorner);
        var fantasyRule = mainMatchRuleFactory.Create(RuleType.Fantasy);
        var eSoccerRule = mainMatchRuleFactory.Create(RuleType.ESoccer);
        var fiveMajorLeagueRule = mainMatchRuleFactory.Create(RuleType.FiveMajorLeague);

        ftCorner5MlRule.SetSuccessor(ftCornerRule);
        ftCornerRule.SetSuccessor(fantasyRule);
        fantasyRule.SetSuccessor(eSoccerRule);
        eSoccerRule.SetSuccessor(fiveMajorLeagueRule);

        foreach (var assignment in assignments)
        {
            assignment.ReportType = assignment.MinorBettypeId.HasValue
                ? notCornerRule.Execute(assignment, assignments)
                : ftCorner5MlRule.Execute(assignment, assignments);

            result.Add(assignment);
        }

        return result;
    }
}