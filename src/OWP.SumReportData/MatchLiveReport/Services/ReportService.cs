using System.ComponentModel.DataAnnotations;
using OWP.Core.Extensions;
using OWP.SumReportData.MatchLiveReport.Constants;
using OWP.SumReportData.MatchLiveReport.Models;
using OWP.SumReportData.MatchLiveReport.Repository;
using OWP.SumReportData.MatchLiveReport.Helpers;

namespace OWP.SumReportData.MatchLiveReport.Services;

public interface ReportService
{
    Task Calculate(DateTime eventDate);

    IEnumerable<ReportModel> Calculate(DateTime eventDate, IEnumerable<OWPMatch> matches);
}

public class ReportServiceImp : ReportService
{
    private readonly ReportRepository reportRepository;

    public ReportServiceImp(ReportRepository reportRepository)
    {
        this.reportRepository = reportRepository;
    }

    public async Task Calculate(DateTime eventDate)
    {
        var matches = await reportRepository.QueryMatchesByDate(eventDate);
        
        var result = Calculate(eventDate, matches);

        await reportRepository.InsertData(result);
    }

    public IEnumerable<ReportModel> Calculate(DateTime eventDate, IEnumerable<OWPMatch> matches)
    {
        var minorMatches = MatchHelper.CategorizeMinorMatches(matches);
        var mainMatches = MatchHelper.CategorizeMainMatches(matches);
        var sabaMatches = MatchHelper.CategorizeSabaMatches(matches);

        var minorMatchReportData = MatchHelper.SumMatches(eventDate, minorMatches);
        var mainMatchReportData = MatchHelper.SumMatches(eventDate, mainMatches);
        var sabaMatchReportData = MatchHelper.SumMatches(eventDate, sabaMatches);

        var finalMinorMatchReportData = minorMatchReportData.Select(x => new ReportModel
        {
            BetType = x.BetType,
            EventDate = x.EventDate,
            IsSaba = x.IsSaba,
            LeagueCategory = x.LeagueCategory,
            MatchCount =
            x.BetType.Equals(BetTypeName.SingleTeamOU.GetAttribute<DisplayAttribute>().Name, StringComparison.InvariantCultureIgnoreCase) ||
            x.BetType.Equals(BetTypeName.PenaltyShootOut.GetAttribute<DisplayAttribute>().Name, StringComparison.InvariantCultureIgnoreCase) ?
            x.MatchCount / 2 :
            x.MatchCount
        });

        return mainMatchReportData.Concat(finalMinorMatchReportData).Concat(sabaMatchReportData);
    }
}