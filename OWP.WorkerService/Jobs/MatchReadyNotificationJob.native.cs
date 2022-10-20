using Fanex.Logging;
using ONEbook.UM.Services;
using OWP.MatchReadyNotification;
using Quartz;

namespace OWP.WorkerService.Jobs;

[DisallowConcurrentExecution]
public class MatchReadyNotificationJob : IJob
{
    private readonly Handler syncService;
    private readonly IMaintenanceService maintenanceService;

    public MatchReadyNotificationJob(Handler syncService, IMaintenanceService maintenanceService)
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
            await syncService.Handle().ConfigureAwait(false);
        }
    }
}