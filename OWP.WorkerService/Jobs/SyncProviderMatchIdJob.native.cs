using OWP.Adminsite.Services;

namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using Quartz;


[DisallowConcurrentExecution]
public class SyncProviderMatchIdJob(SyncMatchProviderIdService handler) : IJob
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

    private async Task ExecuteJob(){
        await LogService.InfoAsync($"[SyncProviderMatchIdJob]=> Job start at: {DateTimeOffset.Now} GMT-4" );

        await handler.Execute(DateTime.Today).ConfigureAwait(false);
    }
}