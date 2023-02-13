using Fanex.Logging;
using ONEbook.UM.Services;
using OWP.SumReport.Constants;
using OWP.SumReport.PrematchReport.Services;
using OWP.SumReport.Services;
using Quartz;

namespace OWP.WorkerService.Jobs;

public class SumPrematchReportJob : IJob
{
    private readonly IMaintenanceService maintenanceService;

    private readonly PrematchReportService reportService;

    private readonly CacheManipulator cacheManipulator;

    public SumPrematchReportJob(
        IMaintenanceService maintenanceService,
        PrematchReportService reportService, 
        CacheManipulator cacheManipulator)
    {
        this.maintenanceService = maintenanceService;
        this.reportService = reportService;
        this.cacheManipulator = cacheManipulator;
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

        var (isGenerateData, lastDate) = await cacheManipulator.GetLastDateForRegenerateData(Constant.PrematchLastDateCacheKey);
        var startDate = isGenerateData ? lastDate : DateTime.Today.AddDays(-3);

        for (var date = startDate; date < DateTime.Today; date = date.AddDays(1))
        {
            await reportService.CalculateDataReport(date).ConfigureAwait(false);
        }

        await cacheManipulator.RemoveLastDateOfSumData(Constant.PrematchLastDateCacheKey);            
    }
}