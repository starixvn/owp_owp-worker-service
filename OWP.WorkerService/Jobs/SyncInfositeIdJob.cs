using Fanex.Logging;
using ONEbook.UM.Services;
using OWP.Infosite.Services;
using OWP.InfositeIdSync.Services;
using Quartz;

namespace OWP.WorkerService.Jobs;

public class SyncInfositeIdJob : IJob
{
    private readonly SyncInfositeIdService synInfositeIdService;
    private readonly TodaySyncService todaySyncService;
    private readonly SyncIsLiveService syncIsLiveService;
    private readonly IMaintenanceService maintenanceService;

    public SyncInfositeIdJob(SyncInfositeIdService synInfositeIdService,
        IMaintenanceService maintenanceService,
        TodaySyncService todaySyncService,
        SyncIsLiveService syncIsLiveService)
    {
        this.synInfositeIdService = synInfositeIdService;
        this.maintenanceService = maintenanceService;
        this.todaySyncService = todaySyncService;
        this.syncIsLiveService = syncIsLiveService;
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
        var underMaintenanceTime = (await maintenanceService.GetUnderMaintenanceTimeAsync());

        if (underMaintenanceTime.IsUnderMaintenanceTime)
        {
            return;
        }

        var fromTime = DateTime.Now.AddHours(9);
        var nowGmt8 = DateTime.Now.AddHours(12);
        var toTime = nowGmt8.Date.AddHours(36);

        await synInfositeIdService.Sync();

        //
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (env == "PROD")
        {
            await syncIsLiveService.Sync(fromTime, toTime).ConfigureAwait(false);
            await todaySyncService.Sync().ConfigureAwait(false);
        }
    }
}