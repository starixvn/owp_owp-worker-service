namespace OWP.SumReport.PrematchReport.Services.League;

using Constants;

public interface HDPLeagueService
{
    Task BuildSingleHDPLeagueReport(DateTime eventDate);
}

public class HDPLeagueServiceImp : HDPLeagueService
{
    private readonly DataReportService dataReportService;

    private readonly Repository repository;

    public HDPLeagueServiceImp(
        DataReportService dataReportService,
        Repository repository)
    {
        this.dataReportService = dataReportService;
        this.repository = repository;
    }

    public async Task BuildSingleHDPLeagueReport(DateTime eventDate)
    {
        var leagues = await repository.GetLeagueByDateAndBettype(eventDate, Constant.HDPID);
        var dataReport = dataReportService.BuildSingleLeagueGroupReport(leagues, Constant.HDPID);
        await dataReportService.InsertLeagueReport(dataReport);
    }
}