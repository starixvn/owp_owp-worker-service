using OWP.Core.Extensions;
using OWP.SumReportData.LAReport.Enum;
using OWP.SumReportData.LAReport.Factory;
using OWP.SumReportData.LAReport.Models;

namespace OWP.SumReportData.LAReport.Services;

public interface CalculationService
{
    Task<IEnumerable<LAReportModel>> Calculate(DateTime eventDate);
}

public class CalculationServiceImp : CalculationService
{
    private readonly LARepository repository;
    private readonly SpecialProductFactory specialProductFactory;
    private readonly MainMatchRuleFactory mainMatchRuleFactory;
    private readonly MultipleAndSingleMatchesFactory multipleAndSingleMatchesFactory;

    private readonly string[] multipleMatchesProviders =
        { "RB2", "IPSD", "RTS", "BG", "BET", "RB2tv", "RB2(tv)", "BG(tv)", "BGtv", "BET(tv)", "BET(s)", "BETtv" };

    public CalculationServiceImp(LARepository repository, SpecialProductFactory specialProductFactory,
        MainMatchRuleFactory mainMatchRuleFactory, MultipleAndSingleMatchesFactory multipleAndSingleMatchesFactory)
    {
        this.repository = repository;
        this.specialProductFactory = specialProductFactory;
        this.mainMatchRuleFactory = mainMatchRuleFactory;
        this.multipleAndSingleMatchesFactory = multipleAndSingleMatchesFactory;
    }

    public async Task<IEnumerable<LAReportModel>> Calculate(DateTime eventDate)
    {
        var assignments = await GetAssignmentDict(eventDate);
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

    private async Task<IDictionary<int, List<Assignment>>> GetAssignmentDict(DateTime eventDate)
    {
        var assignments = await repository.GetLAAssignment(eventDate);

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

        var ftCornerRule = mainMatchRuleFactory.Create(RuleType.FT_CORNER);
        var fantasyRule = mainMatchRuleFactory.Create(RuleType.FANTASY);
        var eSoccerRule = mainMatchRuleFactory.Create(RuleType.ESOCCER);
        var fiveMajorLeagueRule = mainMatchRuleFactory.Create(RuleType.FIVE_MAJOR_LEAGUE);

        ftCornerRule.SetSuccessor(fantasyRule);
        fantasyRule.SetSuccessor(eSoccerRule);
        eSoccerRule.SetSuccessor(fiveMajorLeagueRule);

        foreach (var assignment in assignments)
        {
            var report = assignment;
            assignment.ReportType = assignment.MinorBettypeId.HasValue
                ? notCornerRule.Execute(assignment, assignments)
                : ftCornerRule.Execute(assignment, assignments);

            result.Add(report);
        }

        return result;
    }

    private LAReportModel SumUpAssignments(int userId, IList<Assignment> assignments)
    {
        var emptyResult = new LAReportModel { UserId = userId };
        var specialAndMixedAndESoccerResult = SumUpSpecialAndMixedAndESoccer(assignments, emptyResult);
        var fullResult = SumUpMultipleAndSingle(assignments, specialAndMixedAndESoccerResult);
        fullResult.TotalMatches = fullResult.SumTotal();

        return fullResult;
    }

    private static LAReportModel SumUpSpecialAndMixedAndESoccer(IList<Assignment> assignments, LAReportModel result)
    {
        var sumUpResult = result;

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

        return sumUpResult;
    }

    private LAReportModel SumUpMultipleAndSingle(IList<Assignment> assignments, LAReportModel result)
    {
        var sumUpResult = result;

        //OTHERS
        var othersTypeAssignments = assignments.Where(x => x.ReportType == ReportType.OTHERS);

        var consecutiveMultipleMatchesAssignments = GroupMultipleMatches(othersTypeAssignments);
        var singleMatchesAssignments = othersTypeAssignments.Where(x =>
            !consecutiveMultipleMatchesAssignments.Select(y => y.AssignmentId).Contains(x.AssignmentId));
        var countedMultipleAndSingleReport =
            CountMultipleMatches(consecutiveMultipleMatchesAssignments, singleMatchesAssignments);
        var matchGroupAsignments = consecutiveMultipleMatchesAssignments.GroupBy(i => i.LinkGroup)
            .Select(i => new { LinkGroup = i, MatchCount = i.Count() })
            .ToArray();
        var twoMultipleConsecutiveMatches = matchGroupAsignments.Count(x => x.MatchCount == 2);
        var x3MultipleConsecutiveMatches = matchGroupAsignments.Count(x => x.MatchCount >= 3 && x.MatchCount <= 5);
        var x6MultipleConsecutiveMatches = matchGroupAsignments.Count(x => x.MatchCount >= 6 && x.MatchCount <= 8);
        var x9MultipleConsecutiveMatches = matchGroupAsignments.Count(x => x.MatchCount >= 9);

        sumUpResult.LinkLowerThan3 = countedMultipleAndSingleReport.LinkLowerThan3 + twoMultipleConsecutiveMatches;
        sumUpResult.ProviderTv = countedMultipleAndSingleReport.ProviderTv;
        sumUpResult.Provider = countedMultipleAndSingleReport.Provider;
        sumUpResult.TV = countedMultipleAndSingleReport.TV;
        sumUpResult.Reporter = countedMultipleAndSingleReport.Reporter;
        sumUpResult.Others = countedMultipleAndSingleReport.Others;
        sumUpResult.LinkX3 = x3MultipleConsecutiveMatches;
        sumUpResult.LinkX6 = x6MultipleConsecutiveMatches;
        sumUpResult.LinkX9Plus = x9MultipleConsecutiveMatches;

        return sumUpResult;
    }

    private LAReportModel CountMultipleMatches(IEnumerable<Assignment> multipleMatchesAssignments,
        IEnumerable<Assignment> singleMatchesAssignments)
    {
        var matchGroupAsignments = multipleMatchesAssignments.GroupBy(i => i.LinkGroup)
            .Select(i => new { LinkGroup = i, MatchCount = i.Count() })
            .ToArray();
        var singleLinkGroups = matchGroupAsignments.Where(x => x.MatchCount == 1).Select(x => x.LinkGroup)
            .Select(x => x.Key);
        var singleLinkAssignments = multipleMatchesAssignments.Where(x => singleLinkGroups.Contains(x.LinkGroup));

        var multipleMatchesRule = multipleAndSingleMatchesFactory.Create(RuleType.MULTIPLE);

        var multipleSingleLinkCount = 0;

        var otherAssignments = new List<Assignment>();

        foreach (var assignment in singleLinkAssignments)
        {
            var reportType = multipleMatchesRule.Execute(assignment);

            if (reportType == ReportType.SINGLE_LINK_LOWER_THAN3)
            {
                multipleSingleLinkCount += 1;
            }
            else
            {
                otherAssignments.Add(assignment);
            }
        }

        var singleMatchesAssignmentsAndOthers = singleMatchesAssignments.Concat(otherAssignments);
        var singleAndOthersResult = CountSingleAndOthers(singleMatchesAssignmentsAndOthers);

        return new LAReportModel
        {
            LinkLowerThan3 = multipleSingleLinkCount,
            ProviderTv = singleAndOthersResult.ProviderTv,
            Provider = singleAndOthersResult.Provider,
            TV = singleAndOthersResult.TV,
            Reporter = singleAndOthersResult.Reporter,
            Others = singleAndOthersResult.Others
        };
    }

    private LAReportModel CountSingleAndOthers(IEnumerable<Assignment> singleMatchesAndOthersAssignments)
    {
        var singleMatchesRule = multipleAndSingleMatchesFactory.Create(RuleType.SINGLE);

        var singleMatchesProviderTVCount = 0;
        var singleMatchesProviderCount = 0;
        var singleMatchesTVCount = 0;
        var singleMatchesReporterCount = 0;
        var othersCounts = 0;

        foreach (var assignment in singleMatchesAndOthersAssignments)
        {
            var reportType = singleMatchesRule.Execute(assignment);

            switch (reportType)
            {
                case ReportType.PROVIDER_TV:
                    singleMatchesProviderTVCount += 1;
                    break;

                case ReportType.PROVIDER:
                    singleMatchesProviderCount += 1;
                    break;

                case ReportType.TV:
                    singleMatchesTVCount += 1;
                    break;

                case ReportType.REPORTER:
                    singleMatchesReporterCount += 1;
                    break;

                default:
                    othersCounts += 1;
                    break;
            }
        }

        return new LAReportModel
        {
            ProviderTv = singleMatchesProviderTVCount,
            Provider = singleMatchesProviderCount,
            TV = singleMatchesTVCount,
            Reporter = singleMatchesReporterCount,
            Others = othersCounts
        };
    }

    private IEnumerable<Assignment> GroupMultipleMatches(IEnumerable<Assignment> assignments)
    {
        var result = new List<Assignment>();
        var group = 1;

        var groupedAssignments = assignments
            .Where(x => IsContainsMultipleMatchesProviders(x.ProviderName))
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
                var isLinked = IsKOTimeDifferentWithin5Mins(groupedAssignments[key][i], groupedAssignments[key][i - 1]);

                group = isLinked ? group : group + 1;
                groupedAssignments[key][i].LinkGroup = group;
            }

            group += 1;

            result.AddRange(groupedAssignments[key]);
        }

        return result;
    }

    private static bool IsKOTimeDifferentWithin5Mins(Assignment assignmentA, Assignment assignmentB)
    {
        const int differenceInMinute = 5;
        return Math.Abs(assignmentA.EventDateTime.Subtract(assignmentB.EventDateTime).TotalMinutes) <= differenceInMinute;
    }

    private bool IsContainsMultipleMatchesProviders(string provider)
    {
        var matchProviders = string.IsNullOrEmpty(provider) ? Array.Empty<string>() : provider.Split('/');
        return matchProviders.Any(x => multipleMatchesProviders.Contains(x.Trim(), StringComparer.OrdinalIgnoreCase));
    }
}