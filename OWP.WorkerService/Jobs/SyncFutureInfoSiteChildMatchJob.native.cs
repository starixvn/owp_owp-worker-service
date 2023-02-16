namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using Infosite.Services;
using ONEbook.UM.Services;
using Quartz;

[DisallowConcurrentExecution]
internal class SyncFutureInfoSiteChildMatchJob : IJob
{
    private readonly SyncChildMatchService todaySyncService;

    private readonly IMaintenanceService maintenanceService;

    public SyncFutureInfoSiteChildMatchJob(SyncChildMatchService todaySyncService, IMaintenanceService maintenanceService)
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
            const int syncDays = 10;
            var today = DateTime.Today;
            for (var i = 1; i < syncDays; i++)
            {
                await todaySyncService.Sync(today.AddDays(i)).ConfigureAwait(false);
            }
        }
    }
}