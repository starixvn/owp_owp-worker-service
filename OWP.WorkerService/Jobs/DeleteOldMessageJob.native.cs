namespace OWP.WorkerService.Jobs;

using ArchiveData.Services;
using Fanex.Logging;
using ONEbook.UM.Services;
using Quartz;

[DisallowConcurrentExecution]
public class DeleteOldMessageJob : IJob
{
    private readonly IMaintenanceService maintenanceService;

    private readonly ArchiveDataService archiveDataService;

    public DeleteOldMessageJob(
      IMaintenanceService maintenanceService,
      ArchiveDataService archiveDataService)
    {
        this.maintenanceService = maintenanceService;
        this.archiveDataService = archiveDataService;
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
        if (underMaintenanceTime.IsUnderMaintenanceTime)
        {
            return;
        }
        await LogService.InfoAsync($"Job {GetType()} is starting at {DateTime.Now}");
        await archiveDataService.DeleteOldMessage(DateTime.Now).ConfigureAwait(false);
    }
}