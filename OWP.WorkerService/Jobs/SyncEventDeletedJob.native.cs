namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using Infosite.Services;
using Quartz;

[DisallowConcurrentExecution]
internal class SyncEventDeletedJob : IJob
{
    private readonly SyncEventDeletedService syncEventDeletedService;

    public SyncEventDeletedJob(SyncEventDeletedService syncService)
    {
        syncEventDeletedService = syncService;
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
        await syncEventDeletedService.Sync().ConfigureAwait(false);
    }
}