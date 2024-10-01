namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using Infosite.Services;
using InfositeIdSync.Services;
using Quartz;

[DisallowConcurrentExecution]
public class SyncInfositeIdJob : IJob
{
    private readonly SyncInfositeIdService synInfositeIdService;
    private readonly TodaySyncService todaySyncService;
    private readonly SyncIsLiveService syncIsLiveService;

    private readonly IConfiguration configuration;

    public SyncInfositeIdJob(SyncInfositeIdService synInfositeIdService,
        TodaySyncService todaySyncService,
        SyncIsLiveService syncIsLiveService,
        IConfiguration configuration)
    {
        this.synInfositeIdService = synInfositeIdService;
        this.todaySyncService = todaySyncService;
        this.syncIsLiveService = syncIsLiveService;
        this.configuration = configuration;
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

    public async Task ExecuteJob()
    {
        var fromTime = DateTime.Now.AddHours(9);
        var nowGmt8 = DateTime.Now.AddHours(12);
        var toTime = nowGmt8.Date.AddHours(36);

        await synInfositeIdService.Sync();

        var env = configuration.GetSection("Environment").Value;

        if (string.Equals(env, "PROD"))
        {
            await syncIsLiveService.Sync(fromTime, toTime).ConfigureAwait(false);
            await todaySyncService.Sync().ConfigureAwait(false);
        }
    }
}