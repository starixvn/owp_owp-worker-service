namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using Infosite.Services;
using InfositeIdSync.Services;
using Quartz;

[DisallowConcurrentExecution]
public class SyncInfositeIdJob(
    SyncInfositeIdService synInfositeIdService,
    TodaySyncService todaySyncService,
    SyncIsLiveService syncIsLiveService,
    IConfiguration configuration)
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

    public async Task ExecuteJob()
    {
        await synInfositeIdService.Sync();

        var env = configuration.GetSection("Environment").Value;

        if (string.Equals(env, "PROD"))
        {
            var fromTime = DateTime.Now.AddHours(9);
            var nowGmt8 = DateTime.Now.AddHours(12);
            if (nowGmt8.Hour < 10)
            {
                nowGmt8 = nowGmt8.AddDays(-1);
            }
            var toTime = nowGmt8.Date.AddHours(36);

            await syncIsLiveService.Sync(fromTime, toTime).ConfigureAwait(false);
            await todaySyncService.Sync().ConfigureAwait(false);
        }
    }
}