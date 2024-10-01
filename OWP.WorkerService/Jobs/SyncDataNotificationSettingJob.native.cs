namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using Infosite.Services;
using Quartz;

[DisallowConcurrentExecution]
public class SyncDataNotificationSettingJob : IJob
{
    private readonly NapUserControlService napUserControlService;

    public SyncDataNotificationSettingJob(NapUserControlService napUserControlService)
    {
        this.napUserControlService = napUserControlService;
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
        await napUserControlService.SynDataFunctionPermissionAsync().ConfigureAwait(false);

    }
}