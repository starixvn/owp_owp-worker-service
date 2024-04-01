namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using ONEbook.UM.Services;
using OtherSport.SyncAdminSiteId.Handlers;
using Quartz;

[DisallowConcurrentExecution]
public class SyncOtherSportAdminSiteMatchJob : IJob
{
    private readonly SyncAdminSiteMatchesHandler syncAdminSiteMatchesHandler;
    private readonly IMaintenanceService maintenanceService;

    public SyncOtherSportAdminSiteMatchJob(SyncAdminSiteMatchesHandler syncAdminSiteMatchesHandler, IMaintenanceService maintenanceService)
    {
        this.syncAdminSiteMatchesHandler = syncAdminSiteMatchesHandler;
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
            var eventDate = DateTime.Now.Date.AddHours(12);
            await syncAdminSiteMatchesHandler.Handle(eventDate).ConfigureAwait(false);
        }
    }
}