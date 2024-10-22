namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using Infosite.Services;
using Quartz;

[DisallowConcurrentExecution]
internal class SyncPreMatchLeagueJob : IJob
{
    private readonly SyncPreMatchLeagueService syncService;

    public SyncPreMatchLeagueJob(SyncPreMatchLeagueService syncService)
    {
        this.syncService = syncService;
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
        await syncService.Sync().ConfigureAwait(false);
    }
}