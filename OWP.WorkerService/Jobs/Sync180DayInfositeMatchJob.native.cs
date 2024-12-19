namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using Infosite.Services;
using Quartz;

[DisallowConcurrentExecution]
public class Sync180DayInfositeMatchJob(Sync180DayInfositeMatchService sync180DayInfositeMatchService)
    : IJob
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
        await sync180DayInfositeMatchService.Sync().ConfigureAwait(false);
    }
}