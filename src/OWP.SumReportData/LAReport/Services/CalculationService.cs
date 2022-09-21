using OWP.Core.Extensions;
using OWP.SumReportData.LAReport.Enum;
using OWP.SumReportData.LAReport.Factory;
using OWP.SumReportData.LAReport.Models;

namespace OWP.SumReportData.LAReport.Services;

public interface CalculationService
{
    Task<IEnumerable<LAReportModel>> Calculate(DateTime eventDate, bool isGenerateData);
}

public class CalculationServiceImp : CalculationService
{
    private readonly LARepository repository;
    private readonly SpecialProductFactory specialProductFactory;
    private readonly MainMatchRuleFactory mainMatchRuleFactory;
    private readonly SingleMatchesFactory singleMatchesFactory;

    public CalculationServiceImp(LARepository repository, SpecialProductFactory specialProductFactory,
        MainMatchRuleFactory mainMatchRuleFactory, SingleMatchesFactory singleMatchesFactory)
    {
        this.repository = repository;
        this.specialProductFactory = specialProductFactory;
        this.mainMatchRuleFactory = mainMatchRuleFactory;
        this.singleMatchesFactory = singleMatchesFactory;
    }

    public async Task<IEnumerable<LAReportModel>> Calculate(DateTime eventDate, bool isGenerateData)
    {
        var assignments = await GetAssignmentDict(eventDate, isGenerateData);
        var userSections = await repository.GetUserSection();

        return Calculate(assignments, userSections);
    }

    private IEnumerable<LAReportModel> Calculate(IDictionary<int, List<Assignment>> assignments,
        Dictionary<int, string> userSections)
    {
        var userIds = assignments.Select(x => x.Key);
        foreach (var id in userIds)
        {
            var categoriedAssignment = CategorizeAssignments(assignments[id]);
            var reportModel = SumUpAssignments(id, categoriedAssignment);
            reportModel.Username = userSections.GetValue(id, string.Empty);

            yield return reportModel;
        }
    }

    private async Task<IDictionary<int, List<Assignment>>> GetAssignmentDict(DateTime eventDate, bool isGenerateData)
    {
        var assignments = await repository.GetLAAssignment(eventDate, isGenerateData);

        var assignmentsDict = assignments.GroupBy(x => x.UserId)
            .Where(x => x.Any())
            .ToDictionary(x => x.Key, x => x.ToList());

        return new Dictionary<int, List<Assignment>>(assignmentsDict);
    }

    private IList<Assignment> CategorizeAssignments(IList<Assignment> assignments)
    {
        var result = new List<Assignment>();

        var notCornerRule = specialProductFactory.Create(RuleType.NOT_CORNER);
        var cornerRule = specialProductFactory.Create(RuleType.CORNER);
        notCornerRule.SetSuccessor(cornerRule);

        var ftCorner5MLRule = mainMatchRuleFactory.Create(RuleType.FT_CORNER_FIVE_MAJOR_LEAGUE);
        var ftCornerRule = mainMatchRuleFactory.Create(RuleType.FT_CORNER);
        var fantasyRule = mainMatchRuleFactory.Create(RuleType.FANTASY);
        var eSoccerRule = mainMatchRuleFactory.Create(RuleType.ESOCCER);
        var fiveMajorLeagueRule = mainMatchRuleFactory.Create(RuleType.FIVE_MAJOR_LEAGUE);

        ftCorner5MLRule.SetSuccessor(ftCornerRule);
        ftCornerRule.SetSuccessor(fantasyRule);
        fantasyRule.SetSuccessor(eSoccerRule);
        eSoccerRule.SetSuccessor(fiveMajorLeagueRule);

        foreach (var assignment in assignments)
        {
            var report = assignment;
            assignment.ReportType = assignment.MinorBettypeId.HasValue
                ? notCornerRule.Execute(assignment, assignments)
                : ftCorner5MLRule.Execute(assignment, assignments);

            result.Add(report);
        }

        return result;
    }

    private LAReportModel SumUpAssignments(int userId, IList<Assignment> assignments)
    {
        var sumUpResult = new LAReportModel { UserId = userId };
        SumUpSpecialAndMixedAndESoccer(assignments, sumUpResult);
        SumUpMultiple(assignments, sumUpResult);
        SumUpSingleAndOthers(assignments, sumUpResult);

        return sumUpResult;
    }

    private static LAReportModel SumUpSpecialAndMixedAndESoccer(IList<Assignment> assignments, LAReportModel sumUpResult)
    {
        //SPECIAL PRODUCTS
        var tbookingProducts = assignments.Where(x => x.ReportType == ReportType.TOTAL_BOOKING_PLUS)
            .GroupBy(x => x.ParentMatchId).Select(y => y.Key);
        var cornerProducts = assignments.Where(x => x.ReportType == ReportType.CORNER_PLUS)
            .GroupBy(x => x.ParentMatchId).Select(y => y.Key);
        var fantasyProducts = assignments.Where(x => x.ReportType == ReportType.FANTASY);
        var goalKickProducts = assignments.Where(x => x.ReportType == ReportType.GOAL_KICK)
            .GroupBy(x => x.ParentMatchId).Select(y => y.Key);
        var otherProducts = assignments.Where(x => x.ReportType == ReportType.OTHER_SPECIAL_PRODUCTS);

        //MIXED PRODUCTS
        var ftCornerProduct = assignments.Where(x => x.ReportType == ReportType.FT_CORNER && x.ParentMatchId.HasValue)
            .GroupBy(x => x.ParentMatchId).Select(y => y.Key);
        var ftCorner5MLProduct = assignments.Where(x => x.ReportType == ReportType.FT_CORNER_5ML && x.ParentMatchId.HasValue)
            .GroupBy(x => x.ParentMatchId).Select(y => y.Key);

        //ESOCCER
        var esoccer5Mins = assignments.Where(x => x.ReportType == ReportType.ESOCCER_5MINS);
        var esoccer6Mins = assignments.Where(x => x.ReportType == ReportType.ESOCCER_6MINS);
        var esoccer15Mins = assignments.Where(x => x.ReportType == ReportType.ESOCCER_15MINS);
        var esoccer16Mins = assignments.Where(x => x.ReportType == ReportType.ESOCCER_16MINS);
        var esoccer20Mins = assignments.Where(x => x.ReportType == ReportType.ESOCCER_20MINS);

        //5ML
        var fiveML = assignments.Where(x => x.ReportType == ReportType.MAJOR_LEAGUE);

        sumUpResult.TBookingPlus = tbookingProducts.Count();
        sumUpResult.CornerPlus = cornerProducts.Count();
        sumUpResult.Fantasy = fantasyProducts.Count();
        sumUpResult.GoalKick = goalKickProducts.Count();
        sumUpResult.OtherProducts = otherProducts.Count();
        sumUpResult.ESoccer5Mins = esoccer5Mins.Count();
        sumUpResult.ESoccer6Mins = esoccer6Mins.Count();
        sumUpResult.ESoccer15Mins = esoccer15Mins.Count();
        sumUpResult.ESoccer16Mins = esoccer16Mins.Count();
        sumUpResult.ESoccer20Mins = esoccer20Mins.Count();
        sumUpResult.FTCorner = ftCornerProduct.Count();
        sumUpResult.FiveMajorLeague = fiveML.Count();
        sumUpResult.FTCorner5ML = ftCorner5MLProduct.Count();

        return sumUpResult;
    }

    private void SumUpMultiple(IList<Assignment> assignments, LAReportModel sumUpResult)
    {
        var othersTypeAssignments = assignments.Where(x => x.ReportType == ReportType.OTHERS);
        var consecutiveMultipleMatchesAssignments = GroupMultipleMatches(othersTypeAssignments);
        var matchGroupAsignments = consecutiveMultipleMatchesAssignments
            .GroupBy(i => i.LinkGroup)
            .Select(i => new { LinkGroup = i, MatchCount = i.Count() })
            .ToArray();
        var multipleMatchGroup = matchGroupAsignments.Where(x => x.MatchCount > 1).Select(x => x.LinkGroup).Select(x => x.Key);
        var multipleLinkAssignments = assignments.Where(x => multipleMatchGroup.Contains(x.LinkGroup)).ToList();
        multipleLinkAssignments.ToList().ForEach(x => { x.ReportType = ReportType.MULTIPLE_MATCHES; });

        sumUpResult.Link2 = matchGroupAsignments.Count(x => x.MatchCount == 2);
        sumUpResult.Link3 = matchGroupAsignments.Count(x => x.MatchCount == 3);
        sumUpResult.Link4 = matchGroupAsignments.Count(x => x.MatchCount == 4);
        sumUpResult.Link5 = matchGroupAsignments.Count(x => x.MatchCount == 5);
        sumUpResult.Link6 = matchGroupAsignments.Count(x => x.MatchCount == 6);
        sumUpResult.Link7 = matchGroupAsignments.Count(x => x.MatchCount == 7);
        sumUpResult.Link8Plus = matchGroupAsignments.Count(x => x.MatchCount >= 8);
    }

    private void SumUpSingleAndOthers(IEnumerable<Assignment> assignments, LAReportModel sumUpResult)
    {
        var singleMatchesRule = singleMatchesFactory.Create(RuleType.SINGLE);
        var othersTypeAssignments = assignments.Where(x => x.ReportType == ReportType.OTHERS);

        var singleMatchesMixedProductCount = 0;
        var singleMatchesProviderCount = 0;
        var singleMatchesTVCount = 0;
        var singleMatchesReporterCount = 0;
        var othersCounts = 0;
        var otherMatchIds = new List<int>();

        foreach (var assignment in othersTypeAssignments)
        {
            var reportType = singleMatchesRule.Execute(assignment);
            switch (reportType)
            {
                case ReportType.MIXED_PROVIDER_TV_REPORTER:
                    singleMatchesMixedProductCount += 1;
                    break;

                case ReportType.ONLY_PROVIDER:
                    singleMatchesProviderCount += 1;
                    break;

                case ReportType.ONLY_TV:
                    singleMatchesTVCount += 1;
                    break;

                case ReportType.ONLY_REPORTER:
                    singleMatchesReporterCount += 1;
                    break;

                default:
                    othersCounts += 1;
                    otherMatchIds.Add(assignment.OWPMatchId);
                    break;
            }
        }

        sumUpResult.MixedProviderTVReporter = singleMatchesMixedProductCount;
        sumUpResult.OnlyProvider = singleMatchesProviderCount;
        sumUpResult.OnlyTV = singleMatchesTVCount;
        sumUpResult.OnlyReporter = singleMatchesReporterCount;
        sumUpResult.Others = othersCounts;
        sumUpResult.OtherProductMatchIds = string.Join(",", otherMatchIds.Distinct());
    }

    private List<Assignment> GroupMultipleMatches(IEnumerable<Assignment> assignments)
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

            for (int i = 1; i < groupedAssignments[key].Count; i++)
            {
                var isLinked = IsKOTimeDifferentWithin45Mins(groupedAssignments[key][i], groupedAssignments[key][i - 1]);

                group = isLinked ? group : group + 1;
                groupedAssignments[key][i].LinkGroup = group;
            }

            group += 1;

            result.AddRange(groupedAssignments[key]);
        }

        return result;
    }

    private static bool IsKOTimeDifferentWithin45Mins(Assignment assignmentA, Assignment assignmentB)
    {
        const int differenceInMinute = 45;
        return Math.Abs(assignmentA.EventDateTime.Subtract(assignmentB.EventDateTime).TotalMinutes) <= differenceInMinute;
    }
}