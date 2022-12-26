namespace OWP.SumReport.PrematchReport.Services.Match;

using System;
using System.Threading.Tasks;
using OWP.SumReport.PrematchReport.Models;

public interface MatchService
{
    Task BuildMatchReportAsync(DateTime eventDate);
}

public class MatchServiceImp : MatchService
{
    private readonly Repository repository;
    private readonly HDPMatchService hdpMatchService;
    private readonly OUMatchService OuMatchService;

    public MatchServiceImp(Repository repository, HDPMatchService hdpMatchService, OUMatchService OuMatchService)
    {
        this.repository = repository;
        this.OuMatchService = OuMatchService;
        this.hdpMatchService = hdpMatchService;
    }

    public async Task BuildMatchReportAsync(DateTime eventDate)
    {
        var startNightShift = await repository.QueryExistedStartTime(eventDate.Date);
        await hdpMatchService.BuildSingleHDPMatchReport(eventDate.Date, startNightShift);
        await OuMatchService.BuildSingleOUMatchReport(eventDate.Date, startNightShift);
    }
}