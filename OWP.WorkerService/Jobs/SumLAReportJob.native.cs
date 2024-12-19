namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using Quartz;
using SumReport.Constants;
using SumReport.LAReport;
using SumReport.Services;

[DisallowConcurrentExecution]
public class SumLaReportJob(CacheManipulator cacheManipulator, Handler handler) : IJob
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
        await LogService.InfoAsync("[LAReport]=> Job start");

        var (isGenerateData, lastDate) = await cacheManipulator.GetLastDateForRegenerateData(Constant.LaLastDateCacheKey);

        var startDate = isGenerateData ? lastDate : DateTime.Today.AddDays(-2);

        for (var date = startDate; date < DateTime.Today; date = date.AddDays(1))
        {
            await handler.Handle(date, isGenerateData).ConfigureAwait(false);
        }

        await cacheManipulator.RemoveLastDateOfSumData(Constant.LaLastDateCacheKey);
    }
}