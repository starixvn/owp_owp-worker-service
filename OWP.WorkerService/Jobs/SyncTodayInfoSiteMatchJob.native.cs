namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using Infosite.Services;
using ONEbook.UM.Services;
using Quartz;

[DisallowConcurrentExecution]
internal class SyncTodayInfoSiteMatchJob : IJob
{
    private readonly TodaySyncService todaySyncService;

    private readonly IMaintenanceService maintenanceService;

    public SyncTodayInfoSiteMatchJob(TodaySyncService todaySyncService, IMaintenanceService maintenanceService)
    {
        this.todaySyncService = todaySyncService;
        this.maintenanceService = maintenanceService;
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
        var underMaintenanceTime = await maintenanceService.GetUnderMaintenanceTimeAsync();

        if (!underMaintenanceTime.IsUnderMaintenanceTime)
        {
            await todaySyncService.Sync().ConfigureAwait(false);
        }
    }
}