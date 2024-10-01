namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using Infosite.Services;
using Quartz;

[DisallowConcurrentExecution]
public class Sync180DayInfositeMatchJob : IJob
{
    private readonly Sync180DayInfositeMatchService sync180DayInfositeMatchService;

    public Sync180DayInfositeMatchJob(Sync180DayInfositeMatchService sync180DayInfositeMatchService)
    {
        this.sync180DayInfositeMatchService = sync180DayInfositeMatchService;
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
        await sync180DayInfositeMatchService.Sync().ConfigureAwait(false);
    }
}