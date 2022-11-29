using Fanex.Logging;
using ONEbook.UM.Services;
using OWP.Infosite.Services;
using Quartz;

namespace OWP.WorkerService.Jobs;

[DisallowConcurrentExecution]
internal class SyncDeletedChildInfoSiteMatchJob : IJob
{
    private readonly SyncDeleteChildMatchService syncDeletedService;

    private readonly IMaintenanceService maintenanceService;

    public SyncDeletedChildInfoSiteMatchJob(SyncDeleteChildMatchService syncDeletedService, IMaintenanceService maintenanceService)
    {
        this.syncDeletedService = syncDeletedService;
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

        }
    }
}