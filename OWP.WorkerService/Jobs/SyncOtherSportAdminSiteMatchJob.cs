using Fanex.Logging;
using ONEbook.UM.Services;
using OWP.OtherSport.SyncAdminSiteId.Handlers;
using Quartz;

namespace OWP.WorkerService.Jobs;

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
            var eventDate = DateTime.Now.AddDays(-1).AddHours(12).Date;
            await syncAdminSiteMatchesHandler.Handle(eventDate).ConfigureAwait(false);
        }
    }
}