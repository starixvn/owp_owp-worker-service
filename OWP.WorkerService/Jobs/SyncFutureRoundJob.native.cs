namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using Infosite.Services;
using ONEbook.UM.Services;
using Quartz;

[DisallowConcurrentExecution]
internal class SyncFutureRoundJob : IJob
{
    private readonly FutureSyncRoundService futureSyncRound;

    private readonly IMaintenanceService maintenanceService;

    public SyncFutureRoundJob(FutureSyncRoundService futureSyncRound, IMaintenanceService maintenanceService)
    {
        this.futureSyncRound = futureSyncRound;
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
            await futureSyncRound.Sync().ConfigureAwait(false);
        }
    }
}