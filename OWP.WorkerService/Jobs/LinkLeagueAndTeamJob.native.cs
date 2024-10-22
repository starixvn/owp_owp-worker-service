namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using LinkLeagueAndTeam.Services;
using Quartz;

[DisallowConcurrentExecution]
public class LinkLeagueAndTeamJob : IJob
{
    private readonly LinkLeagueAndTeamService linkLeagueAndTeamService;

    public LinkLeagueAndTeamJob(LinkLeagueAndTeamService handler)
    {
        linkLeagueAndTeamService = handler;
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
        await linkLeagueAndTeamService
            .Execute()
            .ConfigureAwait(false);
    }
}