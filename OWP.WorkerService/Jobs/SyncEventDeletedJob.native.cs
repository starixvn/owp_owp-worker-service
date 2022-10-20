using Fanex.Logging;
using ONEbook.UM.Services;
using OWP.Infosite.Services;
using Quartz;

namespace OWP.WorkerService.Jobs;

[DisallowConcurrentExecution]
internal class SyncEventDeletedJob : IJob
{
    private readonly SyncEventDeletedService syncEventDeletedService;
    private readonly IMaintenanceService maintenanceService;

    public SyncEventDeletedJob(SyncEventDeletedService syncService, IMaintenanceService maintenanceService)
    {
        this.syncEventDeletedService = syncService;
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