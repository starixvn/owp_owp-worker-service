namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using OtherSport.SyncAdminSiteId.Handlers;
using Quartz;

[DisallowConcurrentExecution]
public class SyncOtherSportAdminSiteMatchJob(SyncAdminSiteMatchesHandler syncAdminSiteMatchesHandler) : IJob
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
        var eventDate = DateTime.Now.Date.AddHours(12);
        await syncAdminSiteMatchesHandler.Handle(eventDate).ConfigureAwait(false);
    }
}