namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using Quartz;
using SumReport.Constants;
using SumReport.PrematchReport.Services;
using SumReport.Services;

public class SumPrematchReportJob(
    PrematchReportService reportService,
    CacheManipulator cacheManipulator)
    : IJob
{
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
        var (isGenerateData, lastDate) = await cacheManipulator.GetLastDateForRegenerateData(Constant.PrematchLastDateCacheKey);
        var startDate = isGenerateData ? lastDate : DateTime.Today.AddDays(-3);

        for (var date = startDate; date < DateTime.Today; date = date.AddDays(1))
        {
            await reportService.CalculateDataReport(date).ConfigureAwait(false);
        }

        await cacheManipulator.RemoveLastDateOfSumData(Constant.PrematchLastDateCacheKey);
    }
}