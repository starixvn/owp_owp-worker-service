namespace OWP.WorkerService.Jobs;

using ArchiveData.Services;
using Fanex.Logging;
using Quartz;

[DisallowConcurrentExecution]
public class ArchiveOldMessageJob(ArchiveDataService archiveDataService) : IJob
{
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
        await LogService.InfoAsync($"Job {GetType()} is starting at {DateTime.Now}");
        await archiveDataService.ArchiveOldMessage(DateTime.Now).ConfigureAwait(false);
    }
}