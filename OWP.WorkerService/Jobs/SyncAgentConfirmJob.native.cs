namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using ONEbook.UM.Services;
using OWP.Provider.AgentConfirm;
using Quartz;

[DisallowConcurrentExecution]
public class SyncAgentConfirmJob : IJob
{
    private readonly SyncAgentConfirmHandler syncAgentConfirmHandler;
    private readonly IMaintenanceService maintenanceService;

    public SyncAgentConfirmJob(SyncAgentConfirmHandler syncAgentConfirmHandler, IMaintenanceService maintenanceService)
    {
        this.syncAgentConfirmHandler = syncAgentConfirmHandler;
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
            var eventDate = DateTime.Now.Date.AddDays(-1); // gmt-4 => yesterday
            await syncAgentConfirmHandler.Handle(eventDate).ConfigureAwait(false);
        }
    }
}