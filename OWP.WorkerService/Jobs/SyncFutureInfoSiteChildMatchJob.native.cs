using Fanex.Logging;
using ONEbook.UM.Services;
using OWP.Infosite.Services;
using Quartz;

namespace OWP.WorkerService.Jobs;

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
            var today = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Unspecified);
            for (var i = 0; i <= 10; i++)
            {
                await todaySyncService.Sync(today).ConfigureAwait(false);
            }
        }
    }
}