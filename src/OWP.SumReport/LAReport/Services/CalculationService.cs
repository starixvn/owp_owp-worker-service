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

    private IEnumerable<LaReportModel> Calculate(IDictionary<int, List<Assignment>> userIdToAssignmentMap,
        Dictionary<int, string> userSections)
    {
        var userIds = userIdToAssignmentMap.Select(x => x.Key);

        var assignments = userIdToAssignmentMap
            .SelectMany(x => x.Value)
            .ToList();

        var countOwpMatchIdMap = Utils.CountOwpMatchIdMap(assignments);

        foreach (var id in userIds)
        {
            var categorizeAssignments = CategorizeAssignments(userIdToAssignmentMap[id]);
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

    private LaReportModel SumUpAssignments(
        int userId, 
        IList<Assignment> assignments, 
        ImmutableDictionary<int, int> countOwpMatchIdMap)
    {
        var sumUpResult = new LaReportModel { UserId = userId };
        SumUpSpecialAndMixedAndESoccer(assignments, sumUpResult, countOwpMatchIdMap);
        SumUpMultiple(assignments, sumUpResult, countOwpMatchIdMap);
        SumUpSingleAndOthers(assignments, sumUpResult, countOwpMatchIdMap);

        return sumUpResult;
    }

    private static void SumUpSpecialAndMixedAndESoccer(
        IList<Assignment> assignments, 
        LaReportModel sumUpResult, 
        ImmutableDictionary<int, int> countOwpGameIdMap)
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

    private static void SumUpMultiple(
        IList<Assignment> assignments,
        LaReportModel sumUpResult,
        ImmutableDictionary<int, int> countOwpMatchIdMap)
    {
        var othersTypeAssignments = assignments
            .Where(x => x.ReportType == ReportType.Others)
            .ToList();

        var consecutiveMultipleMatchesAssignments = GroupMultipleMatches(othersTypeAssignments);
        
        var matchGroupAssignments = consecutiveMultipleMatchesAssignments
            .GroupBy(i => i.LinkGroup)
            .Select(i => new GroupMatchInfo
            {
                LinkGroup = i,
                MatchCount = i.Count(),
                OwpMatchId = i.Select(x=>x.OwpMatchId).ToList()
            })
            .ToList();

        var multipleMatchGroup = matchGroupAssignments
            .Where(x => x.MatchCount > 1)
            .Select(x => x.LinkGroup)
            .Select(x => x.Key)
            .ToList();

        Utils.UpdateReportType(assignments, multipleMatchGroup);

        var link2= matchGroupAssignments.Where(x => x.MatchCount == 2).ToList();
        var link3= matchGroupAssignments.Where(x => x.MatchCount == 3).ToList();
        var link4= matchGroupAssignments.Where(x => x.MatchCount == 4).ToList();
        var link5= matchGroupAssignments.Where(x => x.MatchCount == 5).ToList();
        var link6= matchGroupAssignments.Where(x => x.MatchCount == 6).ToList();
        var link7= matchGroupAssignments.Where(x => x.MatchCount == 7).ToList();
        var link8Plus = matchGroupAssignments.Where(x => x.MatchCount >= 8).ToList();

        var countLink2 = Utils.CountMatchInEachGroup(link2, countOwpMatchIdMap);
        var countLink3 = Utils.CountMatchInEachGroup(link3, countOwpMatchIdMap);
        var countLink4 = Utils.CountMatchInEachGroup(link4, countOwpMatchIdMap);
        var countLink5 = Utils.CountMatchInEachGroup(link5, countOwpMatchIdMap);
        var countLink6 = Utils.CountMatchInEachGroup(link6, countOwpMatchIdMap);
        var countLink7 = Utils.CountMatchInEachGroup(link7, countOwpMatchIdMap);
        var countLink8 = Utils.CountMatchInEachGroup(link8Plus, countOwpMatchIdMap);

        sumUpResult.Link2 = countLink2.totalAssignmentGroup;
        sumUpResult.Link3 = countLink3.totalAssignmentGroup;
        sumUpResult.Link4 = countLink4.totalAssignmentGroup;
        sumUpResult.Link5 = countLink5.totalAssignmentGroup;
        sumUpResult.Link6 = countLink6.totalAssignmentGroup;
        sumUpResult.Link7 = countLink7.totalAssignmentGroup;
        sumUpResult.Link8Plus = countLink8.totalAssignmentGroup;

        sumUpResult.TotalMatchInGroupMultiple = countLink2.totalAssignment +
                                                countLink3.totalAssignment +
                                                countLink4.totalAssignment +
                                                countLink5.totalAssignment +
                                                countLink6.totalAssignment +
                                                countLink7.totalAssignment +
                                                countLink8.totalAssignment;
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
                    singleMatchesMixedProductCount += Utils.DecidedIncreaseFullOrHalfSingleAndOthers(assignment.OwpMatchId, countOwpMatchIdMap);
                    break;

                case ReportType.OnlyProvider:
                    singleMatchesProviderCount += Utils.DecidedIncreaseFullOrHalfSingleAndOthers(assignment.OwpMatchId, countOwpMatchIdMap);
                    break;

                case ReportType.OnlyTv:
                    singleMatchesTvCount += Utils.DecidedIncreaseFullOrHalfSingleAndOthers(assignment.OwpMatchId, countOwpMatchIdMap);
                    break;

                case ReportType.OnlyReporter:
                    singleMatchesReporterCount += Utils.DecidedIncreaseFullOrHalfSingleAndOthers(assignment.OwpMatchId, countOwpMatchIdMap);
                    break;

                default:
                    othersCounts += Utils.DecidedIncreaseFullOrHalfSingleAndOthers(assignment.OwpMatchId, countOwpMatchIdMap);
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
            .DistinctBy(x=>x.OwpMatchId)
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