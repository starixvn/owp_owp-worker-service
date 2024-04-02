namespace OWP.WorkerService.Jobs;

using Fanex.Logging;
using LinkLeagueAndTeam.Services;
using ONEbook.UM.Services;
using Quartz;

[DisallowConcurrentExecution]
public class LinkLeagueAndTeamJob : IJob
{
    private readonly LinkLeagueAndTeamService linkLeagueAndTeamService;
    private readonly IMaintenanceService maintenanceService;

    public LinkLeagueAndTeamJob(LinkLeagueAndTeamService handler, IMaintenanceService maintenanceService)
    {
        linkLeagueAndTeamService = handler;
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
        if (underMaintenanceTime.IsUnderMaintenanceTime)
        {
            return;
        }

        await linkLeagueAndTeamService
            .Execute()
            .ConfigureAwait(false);
    }
}