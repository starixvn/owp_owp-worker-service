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
        var assignments = await GetAssignmentPair(eventDate, isGenerateData);
        var userSections = await repository.GetUserSection();

        return Calculate(assignments, userSections);
    }

    private IEnumerable<LaReportModel> Calculate(IDictionary<int, List<Assignment>> assignments,
        Dictionary<int, string> userSections)
    {
        var userIds = assignments.Select(x => x.Key);
        foreach (var id in userIds)
        {
            var categorizeAssignments = CategorizeAssignments(assignments[id]);
            var reportModel = SumUpAssignments(id, categorizeAssignments);
            reportModel.Username = userSections.GetValue(id, string.Empty);

            yield return reportModel;
        }
    }

    private async Task<IDictionary<int, List<Assignment>>> GetAssignmentPair(DateTime eventDate, bool isGenerateData)
    {
        var assignments = await repository.GetLaAssignment(eventDate, isGenerateData);

        var assignmentsDict = assignments.GroupBy(x => x.UserId)
            .Where(x => x.Any())
            .ToDictionary(x => x.Key, x => x.ToList());

        return new Dictionary<int, List<Assignment>>(assignmentsDict);
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

    private LaReportModel SumUpAssignments(int userId, IList<Assignment> assignments)
    {
        var sumUpResult = new LaReportModel { UserId = userId };
        SumUpSpecialAndMixedAndESoccer(assignments, sumUpResult);
        SumUpMultiple(assignments, sumUpResult);
        SumUpSingleAndOthers(assignments, sumUpResult);

        return sumUpResult;
    }

    private static void SumUpSpecialAndMixedAndESoccer(IList<Assignment> assignments, LaReportModel sumUpResult)
    {
        //SPECIAL PRODUCTS
        var totalBookingProducts = assignments.Where(x => x.ReportType == ReportType.TotalBookingPlus)
            .GroupBy(x => x.ParentMatchId).Select(y => y.Key);
        var cornerProducts = assignments.Where(x => x.ReportType == ReportType.CornerPlus)
            .GroupBy(x => x.ParentMatchId).Select(y => y.Key);
        var fantasyProducts = assignments.Where(x => x.ReportType == ReportType.Fantasy);
        var goalKickProducts = assignments.Where(x => x.ReportType == ReportType.GoalKick)
            .GroupBy(x => x.ParentMatchId).Select(y => y.Key);
        var otherProducts = assignments.Where(x => x.ReportType == ReportType.OtherSpecialProducts);

        //MIXED PRODUCTS
        var ftCornerProduct = assignments.Where(x => x.ReportType == ReportType.FtCorner && x.ParentMatchId.HasValue)
            .GroupBy(x => x.ParentMatchId).Select(y => y.Key);
        var ftCorner5MlProduct = assignments.Where(x => x.ReportType == ReportType.FtCorner5Ml && x.ParentMatchId.HasValue)
            .GroupBy(x => x.ParentMatchId).Select(y => y.Key);

        //ESOCCER
        var esoccer5Mins = assignments.Where(x => x.ReportType == ReportType.Esoccer5Mins);
        var esoccer6Mins = assignments.Where(x => x.ReportType == ReportType.Esoccer6Mins);
        var esoccer15Mins = assignments.Where(x => x.ReportType == ReportType.Esoccer15Mins);
        var esoccer16Mins = assignments.Where(x => x.ReportType == ReportType.Esoccer16Mins);
        var esoccer20Mins = assignments.Where(x => x.ReportType == ReportType.Esoccer20Mins);

        //5ML
        var fiveMl = assignments.Where(x => x.ReportType == ReportType.MajorLeague);

        sumUpResult.BookingPlus = totalBookingProducts.Count();
        sumUpResult.CornerPlus = cornerProducts.Count();
        sumUpResult.Fantasy = fantasyProducts.Count();
        sumUpResult.GoalKick = goalKickProducts.Count();
        sumUpResult.OtherProducts = otherProducts.Count();
        sumUpResult.ESoccer5Mins = esoccer5Mins.Count();
        sumUpResult.ESoccer6Mins = esoccer6Mins.Count();
        sumUpResult.ESoccer15Mins = esoccer15Mins.Count();
        sumUpResult.ESoccer16Mins = esoccer16Mins.Count();
        sumUpResult.ESoccer20Mins = esoccer20Mins.Count();
        sumUpResult.FtCorner = ftCornerProduct.Count();
        sumUpResult.FiveMajorLeague = fiveMl.Count();
        sumUpResult.FtCorner5Ml = ftCorner5MlProduct.Count();
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
        var totalMatch=0;
      
        foreach (var assignment in matchGroupAssignments)
        {
            if (assignment.MatchCount > 1)
            {
                totalMatch+=assignment.MatchCount;
            }
        }

        sumUpResult.TotalMatchInGroupMultiple = totalMatch;
    }

    private void SumUpSingleAndOthers(IEnumerable<Assignment> assignments, LaReportModel sumUpResult)
    {
        var singleMatchesRule = singleMatchesFactory.Create(RuleType.Single);
        var othersTypeAssignments = assignments.Where(x => x.ReportType == ReportType.Others);

        var singleMatchesMixedProductCount = 0;
        var singleMatchesProviderCount = 0;
        var singleMatchesTvCount = 0;
        var singleMatchesReporterCount = 0;
        var othersCounts = 0;
        var otherMatchIds = new List<int>();

        foreach (var assignment in othersTypeAssignments)
        {
            var reportType = singleMatchesRule.Execute(assignment);
            switch (reportType)
            {
                case ReportType.MixedProviderTvReporter:
                    singleMatchesMixedProductCount += 1;
                    break;

                case ReportType.OnlyProvider:
                    singleMatchesProviderCount += 1;
                    break;

                case ReportType.OnlyTv:
                    singleMatchesTvCount += 1;
                    break;

                case ReportType.OnlyReporter:
                    singleMatchesReporterCount += 1;
                    break;

                default:
                    othersCounts += 1;
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