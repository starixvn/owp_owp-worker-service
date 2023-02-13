namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using Infosite.Services;
using ONEbook.UM.Services;
using Quartz;

[DisallowConcurrentExecution]
internal class SyncEventDeletedJob : IJob
{
    private readonly SyncEventDeletedService syncEventDeletedService;
    private readonly IMaintenanceService maintenanceService;

    public SyncEventDeletedJob(SyncEventDeletedService syncService, IMaintenanceService maintenanceService)
    {
        syncEventDeletedService = syncService;
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
            await syncEventDeletedService.Sync().ConfigureAwait(false);
        }
    }
}