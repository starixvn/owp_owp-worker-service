namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using Quartz;
using SumReport.Constants;
using SumReport.OvertimeReport.Services;
using SumReport.Services;

[DisallowConcurrentExecution]
public class SumOvertimeReportJob : IJob
{
    private readonly ReportCalculationService calculationService;
    private readonly CacheManipulator cacheManipulator;

    public SumOvertimeReportJob(ReportCalculationService calculationService, CacheManipulator cacheManipulator)
    {
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
        var (isGenerateData, lastDate) = await cacheManipulator.GetLastDateForRegenerateData(Constant.OvertimeLastDateCacheKey);
        var startDate = isGenerateData ? lastDate : DateTime.Today.AddDays(-3);

        for (var date = startDate; date < DateTime.Today; date = date.AddDays(1))
        {
            await calculationService.SumOvertimeReport(date, isGenerateData).ConfigureAwait(false);
        }

        await cacheManipulator.RemoveLastDateOfSumData(Constant.OvertimeLastDateCacheKey);
    }
}