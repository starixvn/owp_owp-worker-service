namespace OWP.WorkerService.Jobs;

using ArchiveData.Services;
using Fanex.Logging;
using Quartz;

[DisallowConcurrentExecution]
public class ArchiveDataJob(ArchiveDataService archiveDataService) : IJob
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
        await archiveDataService.CopyDataToOldTable(DateTime.Today).ConfigureAwait(false);
        await archiveDataService.DeleteOldData(DateTime.Today).ConfigureAwait(false);
        await archiveDataService.DeleteOldNotify(DateTime.Today).ConfigureAwait(false);
        await archiveDataService.DeleteOldNoOfferMatch(DateTime.Today).ConfigureAwait(false);
    }
}