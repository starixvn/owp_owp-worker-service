namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using OWP.Adminsite.Services;
using Quartz;

[DisallowConcurrentExecution]
public class SyncMatchInfoToAdminSiteJob(ISyncDataService syncDataService) : IJob
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
        var nowGmt8 = DateTime.Now.AddHours(12);
        var fromTime = DateTime.Now.Date.AddHours(12);
        if (nowGmt8.Hour < 10)
        {
            nowGmt8 = nowGmt8.AddDays(-1);
        }

        var toTime = nowGmt8.Date.AddHours(36);
        await syncDataService.PushMatchInfoAsync(fromTime, toTime);
    }
}