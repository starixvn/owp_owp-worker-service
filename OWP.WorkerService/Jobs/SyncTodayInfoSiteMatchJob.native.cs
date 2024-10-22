namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using Infosite.Services;
using Quartz;

[DisallowConcurrentExecution]
internal class SyncTodayInfoSiteMatchJob : IJob
{
    private readonly TodaySyncService todaySyncService;

    public SyncTodayInfoSiteMatchJob(TodaySyncService todaySyncService)
    {
        this.todaySyncService = todaySyncService;
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
        await todaySyncService.Sync().ConfigureAwait(false);
    }
}