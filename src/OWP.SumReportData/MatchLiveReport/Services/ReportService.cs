using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using OWP.Core.Extensions;
using OWP.SumReportData.MatchLiveReport.Constants;
using OWP.SumReportData.MatchLiveReport.Models;
using OWP.SumReportData.MatchLiveReport.Repository;
using OWP.SumReportData.MatchLiveReport.Helpers;

namespace OWP.SumReportData.MatchLiveReport.Services;

public interface ReportService
{
    Task Calculate(DateTime eventDate, bool isGenerateData);

    IEnumerable<ReportModel> Calculate(DateTime eventDate, IEnumerable<OWPMatch> minorMatches, IEnumerable<OWPMatch> mainMatches, IEnumerable<OWPMatch> sabaMatches);
}

public class ReportServiceImp : ReportService
{
    private readonly ReportRepository reportRepository;

    public ReportServiceImp(ReportRepository reportRepository)
    {
        this.reportRepository = reportRepository;
    }

    public async Task Calculate(DateTime eventDate, bool isGenerateData)
    {
        var (totalMatches, mainMatches) = await reportRepository.QueryMatchesByDate(eventDate, isGenerateData);

        var minorMatchesCategorized = MatchHelper.CategorizeMinorMatches(totalMatches, mainMatches);
        var mainMatchesCategorized = MatchHelper.CategorizeMainMatches(totalMatches);
        var sabaMatchesCategorized = MatchHelper.CategorizeSabaMatches(totalMatches);

        var saba5MinOwpMatchIds = sabaMatchesCategorized.Where(x => x.BetTypeName == BetTypeName.SabaEliteFriendlyVirtualPes215MinsPlay).Select(x=>x.OWPMatchId).ToList();

        var saba5MinOwpMatchIdGrouped = await reportRepository.GetSaba5minOWPMatchId(saba5MinOwpMatchIds);

        var sabaMatchesGrouped = sabaMatchesCategorized.Where(x => x.BetTypeName != BetTypeName.SabaEliteFriendlyVirtualPes215MinsPlay).ToList();

        foreach (var item in saba5MinOwpMatchIdGrouped)
        {
            var temp = sabaMatchesCategorized.Where(x => x.OWPMatchId == item);
            sabaMatchesGrouped.AddRange(temp.ToList());
        }

        var result = Calculate(eventDate, minorMatchesCategorized, mainMatchesCategorized, sabaMatchesGrouped);

        if (result.Any())
        {
            await reportRepository.InsertData(result, eventDate);
        }
    }

    public IEnumerable<ReportModel> Calculate(DateTime eventDate, IEnumerable<OWPMatch> minorMatches, IEnumerable<OWPMatch> mainMatches, IEnumerable<OWPMatch> sabaMatches)
    {
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
            (int) Math.Ceiling(x.MatchCount / 2m ):
            x.MatchCount
        });

        return mainMatchReportData.Concat(finalMinorMatchReportData).Concat(sabaMatchReportData);
    }
}