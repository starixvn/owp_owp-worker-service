using Fanex.Logging;
using ONEbook.UM.Services;
using OWP.SumReportData.Constants;
using OWP.SumReportData.OvertimeReport.Services;
using OWP.SumReportData.Services;
using Quartz;

namespace OWP.WorkerService.Jobs;

[DisallowConcurrentExecution]
public class SumOvertimeReportJob : IJob
{
    private readonly ReportCalculationService calculationService;
    private readonly CacheManipulator cacheManipulator;
    private readonly IMaintenanceService maintenanceService;

    public SumOvertimeReportJob(IMaintenanceService maintenanceService, ReportCalculationService calculationService, CacheManipulator cacheManipulator)
    {
        this.maintenanceService = maintenanceService;
        this.calculationService = calculationService;
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

        var (isGenerateData, lastDate) = await cacheManipulator.GetLastDateForRegenerateData(Constant.OvertimeLastDateCacheKey);
        var startDate = isGenerateData ? lastDate : DateTime.Today.AddDays(-3);

        for (var date = startDate; date < DateTime.Today; date = date.AddDays(1))
        {
            await calculationService.SumOvertimeReport(date).ConfigureAwait(false);
        }

        await cacheManipulator.RemoveLastDateOfSumData(Constant.OvertimeLastDateCacheKey);
    }
}