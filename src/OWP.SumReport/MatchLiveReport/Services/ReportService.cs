namespace OWP.SumReport.MatchLiveReport.Services;

using System.ComponentModel.DataAnnotations;
using Constants;
using Extensions;
using Helpers;
using Models;
using Repository;

public interface ReportService
{
    Task Calculate(DateTime eventDate, bool isGenerateData = false);

    IEnumerable<ReportModel> Calculate(DateTime eventDate, IList<OwpMatch> minorMatches, IList<OwpMatch> mainMatches, IList<OwpMatch> sabaMatches);
}

public class ReportServiceImp : ReportService
{
    private readonly ReportRepository reportRepository;

    public ReportServiceImp(ReportRepository reportRepository)
    {
        this.reportRepository = reportRepository;
    }

    public async Task Calculate(DateTime eventDate, bool isGenerateData = false)
    {
        var matchesByDate = await reportRepository.QueryMatchesByDate(eventDate, isGenerateData);
        var (totalMatches, mainMatches) = (matchesByDate.Item1.ToList(),matchesByDate.Item2.ToList());

        var minorMatchesCategorized = MatchHelper.CategorizeMinorMatches(totalMatches, mainMatches).ToList();
        var mainMatchesCategorized = MatchHelper.CategorizeMainMatches(totalMatches).ToList();
        var sabaMatchesCategorized = MatchHelper.CategorizeSabaMatches(totalMatches).ToList();

        var saba5MinOwpMatchIds = sabaMatchesCategorized.Where(x => x.BetTypeName == BetTypeName.SabaEliteFriendlyVirtualPes215MinsPlay).Select(x => x.OwpMatchId).ToList();

        var saba5MinOwpMatchIdGrouped = await reportRepository.GetSaba5MinOwpMatchId(saba5MinOwpMatchIds);

        var sabaMatchesGrouped = sabaMatchesCategorized.Where(x => x.BetTypeName != BetTypeName.SabaEliteFriendlyVirtualPes215MinsPlay).ToList();

        foreach (var item in saba5MinOwpMatchIdGrouped)
        {
            var temp = sabaMatchesCategorized.Where(x => x.OwpMatchId == item);
            sabaMatchesGrouped.AddRange(temp.ToList());
        }

        var result = Calculate(eventDate, minorMatchesCategorized, mainMatchesCategorized, sabaMatchesGrouped);

        await reportRepository.InsertData(result, eventDate);
    }

    public IEnumerable<ReportModel> Calculate(DateTime eventDate, IList<OwpMatch> minorMatches, IList<OwpMatch> mainMatches, IList<OwpMatch> sabaMatches)
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
            x.BetType.Equals(BetTypeName.SingleTeamOu.GetAttribute<DisplayAttribute>().Name, StringComparison.InvariantCultureIgnoreCase) ||
            x.BetType.Equals(BetTypeName.PenaltyShootOut.GetAttribute<DisplayAttribute>().Name, StringComparison.InvariantCultureIgnoreCase) ?
            (int)Math.Ceiling(x.MatchCount / 2m) :
            x.MatchCount
        });

        return mainMatchReportData.Concat(finalMinorMatchReportData).Concat(sabaMatchReportData);
    }
}