namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using Provider.AgentConfirm;
using Quartz;

[DisallowConcurrentExecution]
public class SyncAgentConfirmJob(SyncAgentConfirmHandler syncAgentConfirmHandler) : IJob
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
        var eventDate = DateTime.Now.Date.AddDays(-1); // gmt-4 => yesterday
        await syncAgentConfirmHandler.Handle(eventDate).ConfigureAwait(false);
    }
}