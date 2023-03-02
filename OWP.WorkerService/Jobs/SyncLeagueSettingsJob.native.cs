namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using ONEbook.UM.Services;
using Quartz;
using SyncLeagueSettings.Services;

[DisallowConcurrentExecution]
public class SyncLeagueSettingsJob : IJob
{
    private readonly LeagueSettingsService leagueSettingsService;
    private readonly IMaintenanceService maintenanceService;

    public SyncLeagueSettingsJob(LeagueSettingsService leagueSettingsService, IMaintenanceService maintenanceService)
    {
        this.leagueSettingsService = leagueSettingsService;
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
            await leagueSettingsService.InsertNewLeagueSettings(DateTime.Now).ConfigureAwait(false);
        }
    }
}