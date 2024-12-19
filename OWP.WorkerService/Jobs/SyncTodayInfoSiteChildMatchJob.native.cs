namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using Infosite.Services;
using Quartz;

[DisallowConcurrentExecution]
internal class SyncTodayInfoSiteChildMatchJob(SyncChildMatchService todaySyncService) : IJob
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
        var today = DateTime.Today;
        await todaySyncService.Sync(today).ConfigureAwait(false);
    }
}