namespace OWP.SumReportData.PrematchReport.Services.Match;

using OWP.SumReportData.PrematchReport.Constants;

public interface HDPMatchService
{
    Task BuildSingleHDPMatchReport(DateTime eventDate, TimeSpan? startNightShift);
}

public class HDPMatchServiceImp : HDPMatchService
{
    private readonly Repository repository;

    private readonly DataMatchReportService dataMatchReportService;

    public HDPMatchServiceImp(Repository repository, DataMatchReportService dataMatchReportService)
    {
        this.repository = repository;
        this.dataMatchReportService = dataMatchReportService;
    }

    public async Task BuildSingleHDPMatchReport(DateTime eventDate, TimeSpan? startNightShift)
    {
        var leagueDetails = await repository.GetLeagueDetailByDate(eventDate, Constant.HDPID);
        var data = await dataMatchReportService.BuildMatchReport(leagueDetails, eventDate, startNightShift, Constant.HDPID);
        await dataMatchReportService.InsertMatchReport(data);
    }
}