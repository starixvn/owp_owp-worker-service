namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using MatchReadyNotification;
using Quartz;

[DisallowConcurrentExecution]
public class MatchReadyNotificationJob : IJob
{
    private readonly Handler syncService;

    public MatchReadyNotificationJob(Handler syncService)
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
        await syncService.Handle().ConfigureAwait(false);
    }
}