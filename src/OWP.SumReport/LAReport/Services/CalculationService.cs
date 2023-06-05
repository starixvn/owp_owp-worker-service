using System.Collections.Immutable;

namespace OWP.SumReport.LAReport.Services;

using Enum;
using Extensions;
using Factory;
using Models;

public interface CalculationService
{
    Task<IEnumerable<LaReportModel>> Calculate(DateTime eventDate, bool isGenerateData);
}

public class CalculationServiceImp : CalculationService
{
    private readonly LaRepository repository;
    private readonly SpecialProductFactory specialProductFactory;
    private readonly MainMatchRuleFactory mainMatchRuleFactory;
    private readonly SingleMatchesFactory singleMatchesFactory;

    public CalculationServiceImp(LaRepository repository, SpecialProductFactory specialProductFactory,
        MainMatchRuleFactory mainMatchRuleFactory, SingleMatchesFactory singleMatchesFactory)
    {
        this.repository = repository;
        this.specialProductFactory = specialProductFactory;
        this.mainMatchRuleFactory = mainMatchRuleFactory;
        this.singleMatchesFactory = singleMatchesFactory;
    }

    public async Task<IEnumerable<LaReportModel>> Calculate(DateTime eventDate, bool isGenerateData)
    {
        var assignments = await GetAssignment(eventDate, isGenerateData);
        var userSections = await repository.GetUserSection();

        return Calculate(assignments, userSections);
    }

    private IEnumerable<LaReportModel> Calculate(IDictionary<int, List<Assignment>> assignments,
        Dictionary<int, string> userSections)
    {
        var userIds = assignments.Select(x => x.Key);

        var owpMatchId = assignments
            .SelectMany(x => x.Value)
            .ToList();

        var countOwpMatchIdMap = Utils.CountOwpMatchIdMap(owpMatchId);

        foreach (var id in userIds)
        {
            var categorizeAssignments = CategorizeAssignments(assignments[id]);
            var reportModel = SumUpAssignments(id, categorizeAssignments, countOwpMatchIdMap);
            reportModel.Username = userSections.GetValue(id, string.Empty);

            yield return reportModel;
        }
    }

    private async Task<IDictionary<int, List<Assignment>>> GetAssignment(DateTime eventDate, bool isGenerateData)
    {
        var assignments = await repository.GetLaAssignment(eventDate, isGenerateData);

        var assignmentToUserIdMap = assignments.GroupBy(x => x.UserId)
            .Where(x => x.Any())
            .ToDictionary(x => x.Key, x => x.ToList());

        return new Dictionary<int, List<Assignment>>(assignmentToUserIdMap);
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

    private LaReportModel SumUpAssignments(int userId, IList<Assignment> assignments, ImmutableDictionary<int, int> countOwpMatchIdMap)
    {
        var sumUpResult = new LaReportModel { UserId = userId };
        SumUpSpecialAndMixedAndESoccer(assignments, sumUpResult, countOwpMatchIdMap);
        SumUpMultiple(assignments, sumUpResult);
        SumUpSingleAndOthers(assignments, sumUpResult, countOwpMatchIdMap);

        return sumUpResult;
    }

    private static void SumUpSpecialAndMixedAndESoccer(IList<Assignment> assignments, LaReportModel sumUpResult, ImmutableDictionary<int, int> countOwpGameIdMap)
    {
        //SPECIAL PRODUCTS
        var totalBookingProducts = assignments.Where(x => x.ReportType == ReportType.TotalBookingPlus)
            .ToList();

        var cornerProducts = assignments.Where(x => x.ReportType == ReportType.CornerPlus)
            .ToList();
           

        var fantasyProducts = assignments.Where(x => x.ReportType == ReportType.Fantasy).ToList(); ;

        var goalKickProducts = assignments.Where(x => x.ReportType == ReportType.GoalKick).ToList(); ;
            
        var otherProducts = assignments.Where(x => x.ReportType == ReportType.OtherSpecialProducts).ToList();

        //MIXED PRODUCTS
        var ftCornerProduct = assignments.Where(x => x.ReportType == ReportType.FtCorner && x.ParentMatchId.HasValue)
            .ToList();

        var ftCorner5MlProduct = assignments.Where(x => x.ReportType == ReportType.FtCorner5Ml && x.ParentMatchId.HasValue)
            .ToList();

        //ESOCCER
        var esoccer5Mins = assignments.Where(x => x.ReportType == ReportType.Esoccer5Mins).ToList();
        var esoccer6Mins = assignments.Where(x => x.ReportType == ReportType.Esoccer6Mins).ToList();
        var esoccer15Mins = assignments.Where(x => x.ReportType == ReportType.Esoccer15Mins).ToList();
        var esoccer16Mins = assignments.Where(x => x.ReportType == ReportType.Esoccer16Mins).ToList();
        var esoccer20Mins = assignments.Where(x => x.ReportType == ReportType.Esoccer20Mins).ToList();

        //5ML
        var fiveMl = assignments.Where(x => x.ReportType == ReportType.MajorLeague).ToList();

        sumUpResult.BookingPlus = Utils.SumSpecialProduct(totalBookingProducts, countOwpGameIdMap);
        sumUpResult.CornerPlus = Utils.SumSpecialProduct(cornerProducts, countOwpGameIdMap);
        sumUpResult.Fantasy = Utils.SumSpecialProduct(fantasyProducts, countOwpGameIdMap);
        sumUpResult.GoalKick = Utils.SumSpecialProduct(goalKickProducts, countOwpGameIdMap);
        sumUpResult.OtherProducts = Utils.SumSpecialProduct(otherProducts, countOwpGameIdMap);
        sumUpResult.ESoccer5Mins = Utils.SumSpecialProduct(esoccer5Mins, countOwpGameIdMap);
        sumUpResult.ESoccer6Mins = Utils.SumSpecialProduct(esoccer6Mins, countOwpGameIdMap);
        sumUpResult.ESoccer15Mins = Utils.SumSpecialProduct(esoccer15Mins, countOwpGameIdMap);
        sumUpResult.ESoccer16Mins = Utils.SumSpecialProduct(esoccer16Mins, countOwpGameIdMap);
        sumUpResult.ESoccer20Mins = Utils.SumSpecialProduct(esoccer20Mins, countOwpGameIdMap);
        sumUpResult.FtCorner = Utils.SumSpecialProduct(ftCornerProduct, countOwpGameIdMap);
        sumUpResult.FiveMajorLeague = Utils.SumSpecialProduct(fiveMl, countOwpGameIdMap);
        sumUpResult.FtCorner5Ml = Utils.SumSpecialProduct(ftCorner5MlProduct, countOwpGameIdMap);
    }

    private static void SumUpMultiple(IList<Assignment> assignments, LaReportModel sumUpResult)
    {
        var othersTypeAssignments = assignments.Where(x => x.ReportType == ReportType.Others);
        var consecutiveMultipleMatchesAssignments = GroupMultipleMatches(othersTypeAssignments);
        var matchGroupAssignments = consecutiveMultipleMatchesAssignments
            .GroupBy(i => i.LinkGroup)
            .Select(i => new { LinkGroup = i, MatchCount = i.Count() })
            .ToArray();
        var multipleMatchGroup = matchGroupAssignments.Where(x => x.MatchCount > 1).Select(x => x.LinkGroup).Select(x => x.Key);
        var multipleLinkAssignments = assignments.Where(x => multipleMatchGroup.Contains(x.LinkGroup)).ToList();
        multipleLinkAssignments.ToList().ForEach(x => { x.ReportType = ReportType.MultipleMatches; });

        sumUpResult.Link2 = matchGroupAssignments.Count(x => x.MatchCount == 2);
        sumUpResult.Link3 = matchGroupAssignments.Count(x => x.MatchCount == 3);
        sumUpResult.Link4 = matchGroupAssignments.Count(x => x.MatchCount == 4);
        sumUpResult.Link5 = matchGroupAssignments.Count(x => x.MatchCount == 5);
        sumUpResult.Link6 = matchGroupAssignments.Count(x => x.MatchCount == 6);
        sumUpResult.Link7 = matchGroupAssignments.Count(x => x.MatchCount == 7);
        sumUpResult.Link8Plus = matchGroupAssignments.Count(x => x.MatchCount >= 8);
        
        sumUpResult.TotalMatchInGroupMultiple = matchGroupAssignments
            .Where(assignment => assignment.MatchCount > 1)
            .Sum(assignment => assignment.MatchCount);
    }

    private void SumUpSingleAndOthers(IEnumerable<Assignment> assignments, LaReportModel sumUpResult, ImmutableDictionary<int,int> countOwpMatchIdMap)
    {
        var singleMatchesRule = singleMatchesFactory.Create(RuleType.Single);
        var othersTypeAssignments = assignments.Where(x => x.ReportType == ReportType.Others).ToList();

        var singleMatchesMixedProductCount = 0.0;
        var singleMatchesProviderCount = 0.0;
        var singleMatchesTvCount = 0.0;
        var singleMatchesReporterCount = 0.0;
        var othersCounts = 0.0;
        var otherMatchIds = new List<int>();

        foreach (var assignment in othersTypeAssignments)
        {
            var reportType = singleMatchesRule.Execute(assignment);
            switch (reportType)
            {
                case ReportType.MixedProviderTvReporter:
                    singleMatchesMixedProductCount += Utils.DecidedIncreaseSingleAndOthers(assignment.OwpMatchId, countOwpMatchIdMap);
                    break;

                case ReportType.OnlyProvider:
                    singleMatchesProviderCount += Utils.DecidedIncreaseSingleAndOthers(assignment.OwpMatchId, countOwpMatchIdMap);
                    break;

                case ReportType.OnlyTv:
                    singleMatchesTvCount += Utils.DecidedIncreaseSingleAndOthers(assignment.OwpMatchId, countOwpMatchIdMap);
                    break;

                case ReportType.OnlyReporter:
                    singleMatchesReporterCount += Utils.DecidedIncreaseSingleAndOthers(assignment.OwpMatchId, countOwpMatchIdMap);
                    break;

                default:
                    othersCounts += Utils.DecidedIncreaseSingleAndOthers(assignment.OwpMatchId, countOwpMatchIdMap);
                    otherMatchIds.Add(assignment.OwpMatchId);
                    break;
            }
        }

        sumUpResult.MixedProviderTvReporter = singleMatchesMixedProductCount;
        sumUpResult.OnlyProvider = singleMatchesProviderCount;
        sumUpResult.OnlyTv = singleMatchesTvCount;
        sumUpResult.OnlyReporter = singleMatchesReporterCount;
        sumUpResult.Others = othersCounts;
        sumUpResult.OtherProductMatchIds = string.Join(",", otherMatchIds.Distinct());
    }

    private static IEnumerable<Assignment> GroupMultipleMatches(IEnumerable<Assignment> assignments)
    {
        var result = new List<Assignment>();
        var group = 1;

        var groupedAssignments = assignments
            .OrderBy(x => x.EventDateTime)
            .GroupBy(x => x.EventDateTime.AddHours(-12).Date)
            .Where(x => x.Any())
            .ToDictionary(x => x.Key, x => x.ToList());

        foreach (var dateAssignments in groupedAssignments)
        {
            var key = dateAssignments.Key;
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