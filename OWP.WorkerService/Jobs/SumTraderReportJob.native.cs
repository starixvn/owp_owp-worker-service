namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using ONEbook.UM.Services;
using Quartz;
using SumReport.Constants;
using SumReport.Services;
using SumReport.TraderReport;

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

        var (isGenerateData, lastDate) = await cacheManipulator.GetLastDateForRegenerateData(Constant.TraderLastDateCacheKey);
        var startDate = isGenerateData ? lastDate : DateTime.Today.AddDays(-2);

        for (var date = startDate; date <= DateTime.Today; date = date.AddDays(1))
        {
            await handler.Handle(date, isGenerateData).ConfigureAwait(false);
        }

        await cacheManipulator.RemoveLastDateOfSumData(Constant.TraderLastDateCacheKey);
    }
}