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
        if (nowGmt8.Hour < 10)
        {
            nowGmt8 = nowGmt8.AddDays(-1);
        }

        if (nowGmt8.Hour >= 10 && nowGmt8.Hour < 12)
        {
            await syncDataService.PushMatchInfoAsync(nowGmt8);
            await syncDataService.PushMatchInfoAsync(nowGmt8.AddDays(1));
        }
        else
        {
            await syncDataService.PushMatchInfoAsync(nowGmt8);
        }
    }
}