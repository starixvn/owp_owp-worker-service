using Fanex.Logging;
using ONEbook.UM.Services;
using OWP.Infosite.Services;
using Quartz;

namespace OWP.WorkerService.Jobs;

[DisallowConcurrentExecution]
internal class SyncPreMatchLeagueJob : IJob
{
    private readonly SyncPreMatchLeagueService syncService;
    private readonly IMaintenanceService maintenanceService;

    public SyncPreMatchLeagueJob(SyncPreMatchLeagueService syncService, IMaintenanceService maintenanceService)
    {
        this.syncService = syncService;
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
            await syncService.Sync().ConfigureAwait(false);
        }
    }
}