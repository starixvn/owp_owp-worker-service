using Fanex.Logging;
using ONEbook.UM.Services;
using OWP.Infosite.Services;
using Quartz;

namespace OWP.WorkerService.Jobs;

[DisallowConcurrentExecution]
public class Sync180DayInfositeMatchJob : IJob
{
    private readonly Sync180DayInfositeMatchService sync180DayInfositeMatchService;

    private readonly IMaintenanceService maintenanceService;

    public Sync180DayInfositeMatchJob(Sync180DayInfositeMatchService sync180DayInfositeMatchService, IMaintenanceService maintenanceService)
    {
        this.sync180DayInfositeMatchService = sync180DayInfositeMatchService;
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
            await sync180DayInfositeMatchService.Sync().ConfigureAwait(false);
        }
    }
}