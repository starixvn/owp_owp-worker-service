namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using LinkLeagueAndTeam.Services;
using Quartz;

[DisallowConcurrentExecution]
public class LinkLeagueAndTeamJob(LinkLeagueAndTeamService handler) : IJob
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
        await handler
            .Execute()
            .ConfigureAwait(false);
    }
}