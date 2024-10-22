namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using Quartz;
using SumReport.Constants;
using SumReport.Services;
using SumReport.TraderReport;

[DisallowConcurrentExecution]
public class SumTraderReportJob : IJob
{
    private readonly Handler handler;
    private readonly CacheManipulator cacheManipulator;

    public SumTraderReportJob(CacheManipulator cacheManipulator, Handler handler)
    {
        this.cacheManipulator = cacheManipulator;
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
        var (isGenerateData, lastDate) = await cacheManipulator.GetLastDateForRegenerateData(Constant.TraderLastDateCacheKey);
        var startDate = isGenerateData ? lastDate : DateTime.Today.AddDays(-2);

        for (var date = startDate; date < DateTime.Today; date = date.AddDays(1))
        {
            await handler.Handle(date, isGenerateData).ConfigureAwait(false);
        }

        await cacheManipulator.RemoveLastDateOfSumData(Constant.TraderLastDateCacheKey);
    }
}