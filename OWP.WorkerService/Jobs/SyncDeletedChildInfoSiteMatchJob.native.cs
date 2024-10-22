namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using Infosite.Services;
using Quartz;

[DisallowConcurrentExecution]
internal class SyncDeletedChildInfoSiteMatchJob : IJob
{
    private readonly SyncDeleteChildMatchService syncDeletedService;

    public SyncDeletedChildInfoSiteMatchJob(SyncDeleteChildMatchService syncDeletedService)
    {
        this.syncDeletedService = syncDeletedService;
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
        await syncDeletedService.Sync().ConfigureAwait(false);
    }
}