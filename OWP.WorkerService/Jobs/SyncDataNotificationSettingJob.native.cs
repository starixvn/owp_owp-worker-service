namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using Infosite.Services;
using ONEbook.UM.Services;
using Quartz;

[DisallowConcurrentExecution]
public class SyncDataNotificationSettingJob : IJob
{
    private readonly NAPUserControlService napUserControlService;
    private readonly IMaintenanceService maintenanceService;

    public SyncDataNotificationSettingJob(NAPUserControlService napUserControlService, IMaintenanceService maintenanceService)
    {
        this.napUserControlService = napUserControlService;
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
            await napUserControlService.SynDataFunctionPermissionAsync().ConfigureAwait(false);
        }
    }
}