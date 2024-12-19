using OWP.SumReport.LAReport.Enum;
using OWP.SumReport.LAReport.Factory;
using OWP.SumReport.LAReport.Models;

namespace OWP.SumReport.LAReport.Services;

public interface SingleProductService
{
    void SumUp(IList<Assignment> assignments,
        LaReportModel sumUpResult);
}

public class SingleProductServiceImp(SingleMatchesFactory singleMatchesFactory) : SingleProductService
{
    public void SumUp(IList<Assignment> assignments, LaReportModel sumUpResult)
    {
        var singleMatchesRule = singleMatchesFactory.Create(RuleType.Single);
        var othersAssignments = assignments.Where(x => x.ReportType == ReportType.Others).ToList();
        foreach (var assignment in othersAssignments)
        {
            assignment.ReportType = singleMatchesRule.Execute(assignment);
        }
        sumUpResult.MixedProviderTvReporter = Utils.CountSingleMatch(othersAssignments, ReportType.MixedProviderTvReporter);
        sumUpResult.OnlyProvider = Utils.CountSingleMatch(othersAssignments, ReportType.OnlyProvider);
        sumUpResult.OnlyTv = Utils.CountSingleMatch(othersAssignments, ReportType.OnlyTv);
        sumUpResult.OnlyReporter = Utils.CountSingleMatch(othersAssignments, ReportType.OnlyReporter);
        sumUpResult.Others = Utils.CountSingleMatch(othersAssignments, ReportType.Others);

        var otherMatchIds = othersAssignments.Where(x => x.ReportType == ReportType.Others)
            .Select(a => a.OwpMatchId)
            .Distinct();

        sumUpResult.OtherProductMatchIds = string.Join(",", otherMatchIds);
    }
}