using Fanex.Logging;
using ONEbook.UM.Services;
using OWP.SumReportData.Constants;
using OWP.SumReportData.Services;
using OWP.SumReportData.TraderReport;
using Quartz;

namespace OWP.WorkerService.Jobs;

[DisallowConcurrentExecution]
public class SumTraderReportJob : IJob
{
    private readonly Handler handler;
    private readonly CacheManipulator cacheManipulator;
    private readonly IMaintenanceService maintenanceService;

    public SumTraderReportJob(CacheManipulator cacheManipulator, Handler handler, IMaintenanceService maintenanceService)
    {
        this.cacheManipulator = cacheManipulator;
        this.handler = handler;
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

        var lastDate = await cacheManipulator.GetLastDateOfSumData(Constant.TraderLastDateCacheKey);

        Console.WriteLine($"Job {this.GetType()} is starting at {DateTime.Now} for {lastDate}-{DateTime.Today}");

        for (var date = lastDate; date <= DateTime.Today; date = date.AddDays(1))
        {
            await handler.Handle(date).ConfigureAwait(false);
        }

        await cacheManipulator.SaveLastDateOfSumData(Constant.TraderLastDateCacheKey, DateTime.Today.AddDays(-1));

        Console.WriteLine($"Job {this.GetType()} finished at {DateTime.Now}");
    }
}