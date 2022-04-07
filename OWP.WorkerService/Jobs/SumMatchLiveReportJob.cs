using Fanex.Logging;
using ONEbook.UM.Services;
using OWP.SumReportData.Constants;
using OWP.SumReportData.Services;
using OWP.SumReportData.MatchLiveReport.Services;
using Quartz;

namespace OWP.WorkerService.Jobs;

[DisallowConcurrentExecution]
public class SumMatchLiveReportJob : IJob
{
    private readonly ReportService service;
    private readonly CacheManipulator cacheManipulator;
    private readonly IMaintenanceService maintenanceService;

    public SumMatchLiveReportJob(CacheManipulator cacheManipulator, ReportService service, IMaintenanceService maintenanceService)
    {
        this.cacheManipulator = cacheManipulator;
        this.service = service;
        this.maintenanceService = maintenanceService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await ExecuteJob();
        }
        catch (Exception e)
        {
            await LogService.ErrorAsync(e.Message, e);
        }
    }

    private async Task ExecuteJob()
    {
        var underMaintenanceTime = await maintenanceService.GetUnderMaintenanceTimeAsync();
        if (underMaintenanceTime.IsUnderMaintenanceTime)
        {
            return;
        }

        var lastDate = await cacheManipulator.GetLastDateOfSumData(Constant.MatchLiveLastDateCacheKey);

        Console.WriteLine($"Job {this.GetType()} is starting at {DateTime.Now} for {lastDate}-{DateTime.Today}");

        for (var date = lastDate; date <= DateTime.Today; date = date.AddDays(1))
        {
            await service.Calculate(date.AddDays(-1)).ConfigureAwait(false);
        }

        await cacheManipulator.SaveLastDateOfSumData(Constant.MatchLiveLastDateCacheKey, DateTime.Today);

        Console.WriteLine($"Job {this.GetType()} finished at {DateTime.Now}");
    }
}