namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using Quartz;
using SyncLeagueSettings.Services;

[DisallowConcurrentExecution]
public class SyncLeagueSettingsJob : IJob
{
    private readonly LeagueSettingsService leagueSettingsService;

    public SyncLeagueSettingsJob(LeagueSettingsService leagueSettingsService)
    {
        this.leagueSettingsService = leagueSettingsService;
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
        await leagueSettingsService.InsertNewLeagueSettings(DateTime.Now).ConfigureAwait(false);

    }
}