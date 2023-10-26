namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using ONEbook.UM.Services;
using Quartz;
using SumReport.Constants;
using SumReport.LAReport;
using SumReport.Services;

[DisallowConcurrentExecution]
public class SumLaReportJob : IJob
{
    private readonly CacheManipulator cacheManipulator;
    private readonly IMaintenanceService maintenanceService;
    private readonly Handler handler;

    public SumLaReportJob(CacheManipulator cacheManipulator, IMaintenanceService maintenanceService, Handler handler)
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

        var (isGenerateData, lastDate) = await cacheManipulator.GetLastDateForRegenerateData(Constant.LaLastDateCacheKey);
        var startDate = isGenerateData ? lastDate : DateTime.Today.AddDays(-2);

        for (var date = startDate; date < DateTime.Today; date = date.AddDays(1))
        {
            await handler.Handle(date, isGenerateData).ConfigureAwait(false);
        }

        await cacheManipulator.RemoveLastDateOfSumData(Constant.LaLastDateCacheKey);
    }
}