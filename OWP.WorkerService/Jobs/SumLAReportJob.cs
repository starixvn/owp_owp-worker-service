using Fanex.Logging;
using ONEbook.UM.Services;
using OWP.SumReportData.Constants;
using OWP.SumReportData.LAReport;
using OWP.SumReportData.Services;
using Quartz;

namespace OWP.WorkerService.Jobs;

[DisallowConcurrentExecution]
public class SumLAReportJob : IJob
{
    private readonly CacheManipulator cacheManipulator;
    private readonly IMaintenanceService maintenanceService;
    private readonly Handler handler;

    public SumLAReportJob(CacheManipulator cacheManipulator, IMaintenanceService maintenanceService, Handler handler)
    {
        this.cacheManipulator = cacheManipulator;
        this.maintenanceService = maintenanceService;
        this.handler = handler;
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

        var lastDate = await cacheManipulator.GetLastDateOfSumData(Constant.LALastDateCacheKey);

        await LogService.InfoAsync($"Job {this.GetType()} is starting at {DateTime.Now} for {lastDate}-{DateTime.Today}");

        for (var date = lastDate; date <= DateTime.Today; date = date.AddDays(1))
        {
            await handler.Handle(date).ConfigureAwait(false);
        }

        await cacheManipulator.SaveLastDateOfSumData(Constant.LALastDateCacheKey, DateTime.Today.AddDays(-1));

        await LogService.InfoAsync($"Job {this.GetType()} finished at {DateTime.Now}");
    }
}