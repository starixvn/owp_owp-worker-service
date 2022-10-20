using Fanex.Logging;
using ONEbook.UM.Services;
using OWP.ArchiveData.Services;
using Quartz;

namespace OWP.WorkerService.Jobs;

[DisallowConcurrentExecution]
public class ArchiveDataJob : IJob
{
    private readonly IMaintenanceService maintenanceService;
    private readonly ArchiveDataService archiveDataService;

    public ArchiveDataJob(IMaintenanceService maintenanceService, ArchiveDataService archiveDataService)
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

        await LogService.InfoAsync($"Job {this.GetType()} is starting at {DateTime.Now}");
        await archiveDataService.CopyDataToOldTable(DateTime.Today).ConfigureAwait(false);
        await archiveDataService.DeleteOldData(DateTime.Today).ConfigureAwait(false);
        await archiveDataService.DeleteOldNotify(DateTime.Today).ConfigureAwait(false);
    }
}